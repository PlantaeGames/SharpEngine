using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class SkyboxPass : Pass
{
    private const string SKYBOX_VERTEX_SHADER_NAME = "Shaders\\SkyboxVertexShader.hlsl";
    private const string SKYBOX_PIXEL_SHADER_NAME = "Shaders\\SkyboxPixelShader.hlsl";

    private readonly Texture2D _outputTexture;
    private Texture2D _depthTexture;

    private RenderTargetView _renderTargetView;
    private DepthStencilView _depthView;

    private ConstantBuffer _transformBuffer;

    private readonly List<CameraObject> _cameras;

    private PipelineVariation _staticVariation;

    public SkyboxPass(Texture2D outputTexture,
        List<CameraObject> cameras)
    {
        _outputTexture = outputTexture;
        _cameras = cameras;
    }

    public override void OnGo(Device device, DeviceContext context)
    {
        foreach (var camera in _cameras)
        {
            _transformBuffer.Update(
                new SkyboxTransformConstantData()
                {
                    Rotation = camera.Data.Rotation
                });

            var dynamicVariation = new SkyboxDynamicVariation(camera.Viewport);
            dynamicVariation.Bind(context);

            if (_staticVariation.UseIndexRendering)
                context.DrawIndexed(_staticVariation.IndexCount, 0);
            else
                context.Draw(_staticVariation.VertexCount, 0);
        }
    }

    public override void OnInitialize(Device device, DeviceContext context)
    {
        _depthTexture = device.CreateTexture2D(
            [new FSurface(_outputTexture.Info.Size)],
            new TextureCreationInfo()
            {
                Format = DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT,
                Usage = new ResourceUsageInfo()
                {
                    Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                    BindFlags = D3D11_BIND_FLAG.D3D11_BIND_DEPTH_STENCIL
                }

            });
        _depthView = device.CreateDepthStencilView(_depthTexture,
            new ViewCreationInfo()
            {
                Format = _depthTexture.Info.Format,
                TextureMipLevels = _depthTexture.Info.MipLevels,
            });

        _renderTargetView = device.CreateRenderTargetView(_outputTexture,
            new ViewCreationInfo()
            {
                Format = _outputTexture.Info.Format,
                TextureMipLevels = _outputTexture.Info.MipLevels
            });

        var vertexShader = device.CreateVertexShader(
            new ShaderModule(SKYBOX_VERTEX_SHADER_NAME));
        var pixelShader = device.CreatePixelShader(
            new ShaderModule(SKYBOX_PIXEL_SHADER_NAME));

        var inputLayout = device.CreateInputLayout(
            new InputLayoutInfo()
            {
                Layout = new SkyboxVertex(),
                Topology = Topology.TriangleList,
                VertexShader = vertexShader
            });

        var cubeMesh = Mesh.Cube();
        var vertexFragments = new List<Fragment>();
        for(var i = 0; i < cubeMesh.Vertices.Length; i++)
        {
            vertexFragments.AddRange(new SkyboxVertex()
            {
                Position = cubeMesh.Vertices[i].Position
            }.ToFragments());
        }

        var indexUnits = cubeMesh.ToIndexUnits();
        var vertexSurface = new FSurface(new(vertexFragments.Count, 1));
        vertexSurface.SetLinearFragments(vertexFragments.ToArray());

        var indexSurface = new USurface(new(indexUnits.Length, 1));
        indexSurface.SetLinearUnits(indexUnits);

        var vertexBuffer = Buffer.CreateVertexBuffer(
            device.CreateBuffer(
                vertexSurface, typeof(SkyboxVertex),
                new ResourceUsageInfo()
                { 
                    Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                    BindFlags = D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER,
                }));

        var indexBuffer = Buffer.CreateIndexBuffer(
            device.CreateBuffer(
                indexSurface, typeof(Index),
                new ResourceUsageInfo()
                {
                    Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                    BindFlags = D3D11_BIND_FLAG.D3D11_BIND_INDEX_BUFFER
                    
                }));

        _transformBuffer = Buffer.CreateConstantBuffer(
            device.CreateBuffer(new SkyboxTransformConstantData().ToSurface(),
            typeof(SkyboxTransformConstantData),
            new ResourceUsageInfo()
            {
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
                CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE
            }));

        var cubeMapSurfaceSize = new Size(1024, 1024);
        var _0 = new FSurface(cubeMapSurfaceSize, Channels.Quad);
        _0.Clear(new FColor4(1, 0, 0, 0));

        var _1 = new FSurface(cubeMapSurfaceSize, Channels.Quad);
        _1.Clear(new FColor4(0, 1, 0, 0));

        var _2 = new FSurface(cubeMapSurfaceSize, Channels.Quad);
        _2.Clear(new FColor4(0, 0, 1, 0));

        var _3 = new FSurface(cubeMapSurfaceSize, Channels.Quad);
        _3.Clear(new FColor4(1, 1, 0, 0));

        var _4 = new FSurface(cubeMapSurfaceSize, Channels.Quad);
        _4.Clear(new FColor4(1, 1, 1, 0));

        var _5 = new FSurface(cubeMapSurfaceSize, Channels.Quad);
        _5.Clear(new FColor4(1, 0, 1, 0));

        FSurface[] cubeMapSurfaces = 
            [
                _0,
                _1,
                _2,
                _3,
                _4,
                _5
            ];

        var cubeMapTexture = device.CreateTexture2D(
            cubeMapSurfaces,
            new TextureCreationInfo()
            { 
                Usage = new ResourceUsageInfo()
                {
                    Usage  = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                    BindFlags = D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE,
                    MiscFlags = D3D11_RESOURCE_MISC_FLAG.D3D11_RESOURCE_MISC_TEXTURECUBE
                }
            });

        var cubeMapView = device.CreateShaderResourceView(cubeMapTexture,
            new ViewCreationInfo()
            {
                Format = cubeMapTexture.Info.Format,
                TextureMipLevels = cubeMapTexture.Info.MipLevels,
                ViewResourceType = ViewResourceType.CubeMap
            });

        var cubeMapSampler = device.CreateSampler(
            new SamplerInfo()
            {
                AddressMode = D3D11_TEXTURE_ADDRESS_MODE.D3D11_TEXTURE_ADDRESS_CLAMP,
                Filter = D3D11_FILTER.D3D11_FILTER_MIN_MAG_MIP_POINT
            });

        _staticVariation = new SkyboxVariation(
            inputLayout,
            vertexBuffer,
            indexBuffer,
            vertexShader,
            _transformBuffer,
            pixelShader,
            cubeMapView,
            cubeMapSampler,
            _renderTargetView,
            _depthView
            );
    }

    public override void OnReady(Device device, DeviceContext context)
    {
        context.ClearRenderTargetView(_renderTargetView, new());
        context.ClearDepthStencilView(_depthView, new DepthStencilClearInfo()
        {
            ClearFlags = D3D11_CLEAR_FLAG.D3D11_CLEAR_DEPTH,
            Depth = 1f
        });

        _staticVariation.Bind(context);
    }
}