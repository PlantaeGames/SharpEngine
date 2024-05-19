using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class DepthPass : Pass
{
    private const int SHADOW_MAP_WIDTH = 2048;
    private const int SHADOOW_MAP_HEIGHT = 2048;
    private const float CLEAR_DEPTH_VALUE = 1f;

    private const string DEPTH_VERTEX_SHADER_NAME = "Shaders\\DepthVertexShader.hlsl";
    private const string DEPTH_PIXEL_SHADER_NAME = "Shaders\\DepthPixelShader.hlsl";

    private ConstantBuffer _lightPerspectiveBuffer;

    public readonly Texture2D[] DepthTextures;
    private readonly DepthStencilView[] _depthViews;

    private readonly List<LightObject> _lights;
    private readonly int _maxLightsCount;

    private readonly List<GraphicsObject> _graphicsObjects;

    private PipelineVariation _staticVariation;

    private DepthStencilState _depthState;

    public DepthPass(List<LightObject> lights,
        int maxLightsCount,
        List<GraphicsObject> graphicsObjects)
    {
        _lights = lights;
        _graphicsObjects = graphicsObjects;
        _maxLightsCount = maxLightsCount;

        _depthViews = new DepthStencilView[maxLightsCount];
        DepthTextures = new Texture2D[maxLightsCount];
    }

    public PipelineVariation CreateSubVariation(
        Device device, Material material, Mesh mesh)
    {
        return AddNewSunVariation(device, material, mesh);
    }

    public override void OnGo(Device device, DeviceContext context)
    {
        for (var i = 0; i < _lights.Count; i++)
        {
            var dynamicVariation = new DepthDynamicVariation(
                _depthState,
                _depthViews[i]);
            dynamicVariation.Bind(context);

            _lightPerspectiveBuffer.Update(_lights[i]._lastUpdatedData);
            foreach (var variation in _subVariations)
            {
                variation.Bind(context);
                if (variation.UseIndexRendering)
                    context.DrawIndexed(variation.IndexCount, 0);
                else
                    context.Draw(variation.VertexCount, 0);
            }
        }
    }

    public override void OnInitialize(Device device, DeviceContext context)
    {
        _lightPerspectiveBuffer = Buffer.CreateConstantBuffer(
            device.CreateBuffer(
                new TransformConstantData().ToSurface(), typeof(TransformConstantData),
            new ResourceUsageInfo()
            {
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
                CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE
            }));


        var vertexShader = device.CreateVertexShader(
            new ShaderModule(DEPTH_VERTEX_SHADER_NAME));

        var pixelShader = device.CreatePixelShader(
            new ShaderModule(DEPTH_PIXEL_SHADER_NAME));

        var viewport = new Viewport(
            new D3D11_VIEWPORT()
            {
                Width = SHADOW_MAP_WIDTH,
                Height = SHADOOW_MAP_HEIGHT,
                MinDepth = 0f,
                MaxDepth = 1f
            });

        _depthState = device.CreateDepthStencilState(
            new DepthStencilStateInfo()
            {
                DepthEnabled = true,
                DepthWriteMask = D3D11_DEPTH_WRITE_MASK.D3D11_DEPTH_WRITE_MASK_ALL,
                DepthComparisionFunc = D3D11_COMPARISON_FUNC.D3D11_COMPARISON_GREATER
            });

        _staticVariation = new DepthVariation(
            vertexShader, _lightPerspectiveBuffer, pixelShader, viewport);

        for(var i = 0; i < _maxLightsCount; i++)
        {
            var depthTexture = device.CreateTexture2D(
                new FSurface(new(SHADOW_MAP_WIDTH, SHADOOW_MAP_HEIGHT)),
                new ResourceUsageInfo()
                {
                    Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                    BindFlags = D3D11_BIND_FLAG.D3D11_BIND_DEPTH_STENCIL
                },
                DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT);

            DepthTextures[i] = depthTexture;

            var depthView = device.CreateDepthStencilView(
                depthTexture,
                new ViewCreationInfo()
                {
                    Format = depthTexture.Info.Format,
                    Size = depthTexture.Info.Size
                });

            _depthViews[i] = depthView;
        }
    }

    public override void OnReady(Device device, DeviceContext context)
    {
        foreach (var view in _depthViews)
        {
            context.ClearDepthStencilView(view,
                new DepthStencilClearInfo()
                {
                    ClearFlags = D3D11_CLEAR_FLAG.D3D11_CLEAR_DEPTH,
                    Depth = CLEAR_DEPTH_VALUE,
                });
        }

        _staticVariation.Bind(context);
    }

    private PipelineVariation AddNewSunVariation(
        Device device, Material material, Mesh mesh)
    {
        var vertexShader = _staticVariation.VertexShaderStage.VertexShader;

        var inputLayout = device.CreateInputLayout(
            new InputLayoutInfo()
            {
                Layout = new Vertex(),
                Topology = material.Topology,
                VertexShader = vertexShader
            });

        var vertexFragments = mesh.ToVertexFragments();
        var vertexSurface = new FSurface(new(vertexFragments.Length, 1));
        vertexSurface.SetLinearFragments(vertexFragments);
        var vertexBuffer = Buffer.CreateVertexBuffer(
            device.CreateBuffer(vertexSurface, typeof(Vertex),
            new ResourceUsageInfo()
            {
                Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER
            }));

        var indexUnits = mesh.ToIndexUnits();
        var indicesSurface = new USurface(new(indexUnits.Length, 1));
        indicesSurface.SetLinearUnits(indexUnits);
        var indexBuffer = Buffer.CreateIndexBuffer(
            device.CreateBuffer(indicesSurface, typeof(Index),
            new ResourceUsageInfo()
            {
                Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_INDEX_BUFFER
            }));

        var variation = new DepthSubVariation(
            inputLayout,
            vertexBuffer,
            indexBuffer
            );

        _subVariations.Add(variation);
        return variation;
    }
}