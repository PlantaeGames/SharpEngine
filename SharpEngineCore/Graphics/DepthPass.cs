using System;
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

    public List<Texture2D> DepthTextures => _data.DepthTextures;
    public List<ShaderResourceView> DepthShaderViews => _data.ShaderViews;
    public List<Sampler> DepthSamplers => _data.DepthSamplers;
    private readonly DepthPassData _data;

    private readonly List<LightObject> _lights;
    private readonly int _maxLightsCount;

    private PipelineVariation _staticVariation;

    private DepthStencilState _depthState;

    public DepthPass(List<LightObject> lights, int maxLightsCount)
    {
        _lights = lights;
        _maxLightsCount = maxLightsCount;

        _data = new(maxLightsCount);
    }

    public override void OnGo(Device device, DeviceContext context)
    {
        for (var i = 0; i < _lights.Count; i++)
        {
            var data = _data.Get(_lights[i].Id);
            if (data.active == false)
                continue;

            for (var x = 0; x < data.texture.Info.SubtexturesCount; x++)
            {
                var dynamicVariation = new DepthDynamicVariation(
                    _outputView,
                    data.views[x]);
                dynamicVariation.Bind(context);

                var rotation = _lights[i].Data.Rotation;

                if (_lights[i].Data.LightType == Light.Point)
                {
                    // change rotation
                    rotation = Utilities.Utilities.CubeFaceIndexToAngle(x);
                }

                _lightPerspectiveBuffer.Update(
                    new LightTransformConstantData()
                    {
                        Position = _lights[i].Data.Position,
                        Rotation = rotation,
                        Scale = _lights[i].Data.Scale,
                        LightType = _lights[i].Data.LightType,
                        Attributes = _lights[i].Data.Attributes
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
                DepthComparisionFunc = D3D11_COMPARISON_FUNC.D3D11_COMPARISON_LESS_EQUAL
            });

        _staticVariation = new DepthVariation(
            vertexShader, pixelShader, viewport, _depthState);

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

        var samplers = new Sampler[_maxLightsCount];
        for (var i = 0; i < _maxLightsCount; i++)
        {
            var sampler = device.CreateSampler(new()
            {
                AddressMode = D3D11_TEXTURE_ADDRESS_MODE.D3D11_TEXTURE_ADDRESS_CLAMP,
                Filter = D3D11_FILTER.D3D11_FILTER_MIN_MAG_MIP_POINT
            });

            samplers[i] = sampler;
        }

        _data.SetSamplers(samplers);
    }

    public override void OnReady(Device device, DeviceContext context)
    {
        foreach (var value in _data)
        {
            if (value.texture.IsValid() == false)
                continue;

            foreach (var view in value.views)
            {
                context.ClearDepthStencilView(view,
                    new DepthStencilClearInfo()
                    {
                        ClearFlags = D3D11_CLEAR_FLAG.D3D11_CLEAR_DEPTH,
                        Depth = CLEAR_DEPTH_VALUE,
                    });
            }
        }

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
            [_lightPerspectiveBuffer, transformBuffer],
            vertexBuffer,
            indexBuffer,
            material.UseIndexedRendering);

        _subVariations.Add(variation);
        return variation;
    }

    public override void OnLightAdd(LightObject light, Device device)
    {
        var surfaces = new List<FSurface>();
        if(light.Data.LightType == Light.Point)
        {
            for(var i = 0; i < 6; i ++)
            {
                surfaces.Add(new FSurface(new(SHADOW_MAP_WIDTH, SHADOOW_MAP_HEIGHT)));
            }
        }
        if(light.Data.LightType == Light.Directional)
        {
            surfaces.Add(new FSurface(new(SHADOW_MAP_WIDTH, SHADOOW_MAP_HEIGHT)));
        }

        var texture = device.CreateTexture2D(
            surfaces.ToArray(),
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

        var views = new List<DepthStencilView>();
        for(var i = 0; i < texture.Info.SubtexturesCount; i++)
        {
            var view = device.CreateDepthStencilView(texture,
                new ViewCreationInfo()
                {
                    Format = DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT,

                    TextureSliceIndex = i,
                    TextureSliceCount = 1,
                    ViewResourceType = ViewResourceType.Texture2DArray
                }); ;

            views.Add(view);
        }

        var srv = device.CreateShaderResourceView(
            texture,
            new ViewCreationInfo()
            {
                Format = DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT,

                TextureSliceCount = texture.Info.SubtexturesCount,
                ViewResourceType = ViewResourceType.Texture2DArray
            });

        _data.Add(light.Id, texture, views, srv);
    }

    public override void OnLightRemove(LightObject light, Device device)
    {
        _data.Remove(light.Id);
    }

    public override void OnLightPause(LightObject light, Device device)
    {
        _data.Pause(light.Id);
    }

    public override void OnLightResume(LightObject light, Device device)
    {
        _data.Resume(light.Id);
    }

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