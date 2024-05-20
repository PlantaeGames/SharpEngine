using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class DepthPass : Pass
{
    public Texture2D OutputTexture { get; private set; }
    private RenderTargetView _outputView;

    private const int SHADOW_MAP_WIDTH = 2048;
    private const int SHADOOW_MAP_HEIGHT = 2048;
    private const float CLEAR_DEPTH_VALUE = 1f;

    private const string DEPTH_VERTEX_SHADER_NAME = "Shaders\\DepthVertexShader.hlsl";
    private const string DEPTH_PIXEL_SHADER_NAME = "Shaders\\DepthPixelShader.hlsl";

    private ConstantBuffer _lightPerspectiveBuffer;

    public readonly List<Texture2D> DepthTextures = new();
    private readonly List<DepthStencilView> _depthViews = new();

    private readonly List<LightObject> _lights;
    private readonly int _maxLightsCount;

    private PipelineVariation _staticVariation;

    private DepthStencilState _depthState;

    public DepthPass(List<LightObject> lights, int maxLightsCount)
    {
        _lights = lights;
        _maxLightsCount = maxLightsCount;
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
                _outputView,
                _depthState,
                _depthViews[i]);
            dynamicVariation.Bind(context);

            _lightPerspectiveBuffer.Update(
                new LightTransformConstantData()
                {
                    Position = _lights[i]._lastUpdatedData.Position,
                    Rotation = _lights[i]._lastUpdatedData.Rotation,
                    LightType = _lights[i]._lastUpdatedData.LightType,
                    Attributes = _lights[i]._lastUpdatedData.Attributes
                });

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
                new LightTransformConstantData().ToSurface(), typeof(LightTransformConstantData),
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

        AddDepthTextures(device, _maxLightsCount);

        OutputTexture = device.CreateTexture2D(
            new FSurface(new(SHADOW_MAP_WIDTH, SHADOOW_MAP_HEIGHT), Channels.Quad),
            new ResourceUsageInfo()
            {
                Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET
            }, DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM);

        _outputView = device.CreateRenderTargetView(OutputTexture,
            new ViewCreationInfo()
            {
                Format = OutputTexture.Info.Format,
                ViewResourceType = ViewResourceType.Texture2D
            });
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

        NormalizeDepthTextures(device);

        _staticVariation.Bind(context);
    }

    private void NormalizeDepthTextures(Device device)
    {
        // if we r in lights limit 
        if(_lights.Count > _maxLightsCount == false)
            return;

        if (DepthTextures.Count == _lights.Count)
            return;

        // need to add more textures
        if (DepthTextures.Count < _lights.Count)
        {
            var addCount = _lights.Count - DepthTextures.Count;
            AddDepthTextures(device, addCount);

            return;
        }

        var removeCount = DepthTextures.Count - _lights.Count;
        var newDepthCount = DepthTextures.Count - removeCount;
        var stableCount = newDepthCount < _maxLightsCount ?
                           _maxLightsCount - newDepthCount : 0;
        removeCount -= stableCount;

        RemoveDepthTextures(device, removeCount);
    }

    private void AddDepthTextures(Device device, int count)
    {
        for (var i = 0; i < count; i++)
        {
            var depthTexture = device.CreateTexture2D(
                new FSurface(new(SHADOW_MAP_WIDTH, SHADOOW_MAP_HEIGHT)),
                new ResourceUsageInfo()
                {
                    Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                    BindFlags = D3D11_BIND_FLAG.D3D11_BIND_DEPTH_STENCIL |
                                D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE
                },
                DXGI_FORMAT.DXGI_FORMAT_R32_TYPELESS);

            DepthTextures.Add(depthTexture);

            var depthView = device.CreateDepthStencilView(
                depthTexture,
                new ViewCreationInfo()
                {
                    Format = DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT
                });

            _depthViews.Add(depthView);
        }
    }

    private void RemoveDepthTextures(Device device, int removeCount)
    {
        DepthTextures.RemoveRange(DepthTextures.Count - removeCount, removeCount);
        _depthViews.RemoveRange(_depthViews.Count - removeCount, removeCount);
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
            indexBuffer,
            material.UseIndexedRendering);

        _subVariations.Add(variation);
        return variation;
    }
}