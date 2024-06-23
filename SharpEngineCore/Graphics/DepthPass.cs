using System.Diagnostics;
using TerraFX.Interop.DirectX;

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

    private ConstantBuffer _lightTransformCBuffer;

    private DepthStencilView _depthView;
    private Sampler _depthSampler;
    private Texture2D _depthTexture;

    public Texture2D DepthTexture => _depthTexture;
    public Sampler DepthSampler => _depthSampler;

    private readonly List<LightObject> _lights;
    private readonly int _maxLightsCount;

    private PipelineVariation _staticVariation;

    private DepthStencilState _depthState;

    public DepthPass(List<LightObject> lights, int maxLightsCount)
    {
        _lights = lights;
        _maxLightsCount = maxLightsCount;
    }

    public void TakePass(Device device, DeviceContext context,
                         int lightIndex, int rotationIndex = -1)
    {
        Debug.Assert(lightIndex < _lights.Count,
                     "The light index must correspond to the lights in the pipeline.");

        context.ClearState();
        OnReady(device, context);

        Pass();

        context.ClearState();

        void Pass()
        {
            var light = _lights[lightIndex];

            var rotation = rotationIndex < 0 ? 
                             light.Data.Rotation :   
                             Utilities.Utilities.CubeFaceIndexToAngle(rotationIndex);

            _lightTransformCBuffer.Update(
                new LightTransformConstantData()
                {
                    Position = light.Data.Position,
                    Rotation = rotation,
                    Scale = light.Data.Scale,
                    LightType = light.Data.LightType,
                    Attributes = light.Data.Attributes
                });

            foreach (var variation in _subVariations)
            {
                variation.Bind(context);
                if (variation.UseIndexRendering)
                    context.DrawIndexed(variation.IndexCount, 0);
                else
                    context.Draw(variation.VertexCount, 0);

                variation.Unbind(context);
            }
        }
    }

    public override void OnGo(Device device, DeviceContext context)
    {
    }

    public override void OnInitialize(Device device, DeviceContext context)
    {
        _lightTransformCBuffer = Buffer.CreateConstantBuffer(
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
                DepthComparisionFunc = D3D11_COMPARISON_FUNC.D3D11_COMPARISON_LESS_EQUAL
            });

        OutputTexture = device.CreateTexture2D(
            [new FSurface(new(SHADOW_MAP_WIDTH, SHADOOW_MAP_HEIGHT), Channels.Quad)],
            new TextureCreationInfo()
            {
                Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM,
                UsageInfo = new ResourceUsageInfo()
                {
                    Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                    BindFlags = D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET
                }
            });

        _outputView = device.CreateRenderTargetView(OutputTexture,
            new ViewCreationInfo()
            {
                Format = OutputTexture.Info.Format,
                ViewResourceType = ViewResourceType.Texture2D
            });

        _depthSampler = device.CreateSampler(new()
        {
            AddressMode = D3D11_TEXTURE_ADDRESS_MODE.D3D11_TEXTURE_ADDRESS_CLAMP,
            Filter = D3D11_FILTER.D3D11_FILTER_MIN_MAG_MIP_POINT
        });

        _depthTexture = device.CreateTexture2D(
            [new FSurface(new(SHADOW_MAP_WIDTH, SHADOOW_MAP_HEIGHT))],
            new TextureCreationInfo()
            {
                UsageInfo = new ResourceUsageInfo()
                {
                    Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                    BindFlags = D3D11_BIND_FLAG.D3D11_BIND_DEPTH_STENCIL |
                                D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE
                },
                Format = DXGI_FORMAT.DXGI_FORMAT_R32_TYPELESS
            });

        _depthView = device.CreateDepthStencilView(_depthTexture,
            new ViewCreationInfo()
            {
                Format = DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT,
                ViewResourceType = ViewResourceType.Texture2D
            });

        var rastrizer = device.CreateRasterizerState(
            new RasterizerStateInfo()
            {
                CullMode = D3D11_CULL_MODE.D3D11_CULL_FRONT,
                FillMode = D3D11_FILL_MODE.D3D11_FILL_SOLID
            });


        _staticVariation = new DepthVariation(
                vertexShader, pixelShader, viewport,
                _depthState, _depthView, _outputView, rastrizer);
    }

    public override void OnReady(Device device, DeviceContext context)
    {
        context.ClearDepthStencilView(_depthView,
            new DepthStencilClearInfo()
            {
                ClearFlags = D3D11_CLEAR_FLAG.D3D11_CLEAR_DEPTH,
                Depth = CLEAR_DEPTH_VALUE,
            });
        context.ClearRenderTargetView(_outputView, new());

        _staticVariation.Bind(context);
    }
    private PipelineVariation AddNewSunVariation(
        Device device, Material material, Mesh mesh, ConstantBuffer transformBuffer)
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
            [_lightTransformCBuffer, transformBuffer],
            vertexBuffer,
            indexBuffer,
            material.UseIndexedRendering);

        _subVariations.Add(variation);
        return variation;
    }

    public override void OnLightAdd(LightObject light, Device device)
    {
    }

    public override void OnLightRemove(LightObject light, Device device)
    { }

    public override void OnLightPause(LightObject light, Device device)
    { }

    public override void OnLightResume(LightObject light, Device device)
    { }

    public override void OnCameraAdd(CameraObject camera, Device device)
    {
    }

    public override void OnCameraPause(CameraObject camera, Device device)
    {
    }

    public override void OnCameraResume(CameraObject camera, Device device)
    {
    }

    public override void OnCameraRemove(CameraObject camera, Device device)
    {
    }

    public override void OnGraphicsAdd(GraphicsObject graphics, Device device)
    {
        var variation = AddNewSunVariation(
            device, graphics.Info.material, graphics.Info.mesh, graphics.GetTransformBuffer());

        graphics.AddVariation(variation);
    }

    public override void OnGraphicsRemove(GraphicsObject graphics, Device device)
    {
    }

    public override void OnGraphicsPause(GraphicsObject graphics, Device device)
    {
    }

    public override void OnGraphicsResume(GraphicsObject graphics, Device device)
    {
    }

    public override void OnSkyboxSet(CubemapInfo info, Device device)
    {
    }
}