using System.Diagnostics;

using TerraFX.Interop.DirectX;

using SharpEngineCore.Utilities;

namespace SharpEngineCore.Graphics;
internal sealed class ForwardPass : Pass
{
    private const float DEPTH_CLEAR_VALUE = 1f;

    public Texture2D OutputTexture { get; private set; }
    private RenderTargetView _outputView;

    private Texture2D _depthTexture;
    private DepthStencilView _depthView;
    private DepthStencilState _depthState;

    private readonly int _maxPerVariationLightsCount;
    private readonly List<LightObject> _lightObjects;
    private Buffer _lightsBuffer;
    private ShaderResourceView _lightsSRV;

    private readonly List<CameraObject> _cameraObjects;
    private ConstantBuffer _currentCameraCBuffer;

    private readonly List<Texture2D> _shadowDepthTextures;
    private readonly List<ShaderResourceView> _shadowSRVS = new();
    private readonly List<Sampler> _depthSamplers = new();

    private PipelineVariation _staticVariation;
    private readonly Size _resolution;

    public PipelineVariation CreateSubVariation(
        Device device, Material material, Mesh mesh)
    {
        return AddNewSubVariation(device, material, mesh);
    }

    public ForwardPass(Size resolution,
        int maxPerVariationLightsCount,
        List<LightObject> lightObjects,
        List<CameraObject> cameraObjects,
        List<Texture2D> shadowDepthTextures) :
        base()
    {
        _resolution = resolution;
        _maxPerVariationLightsCount = maxPerVariationLightsCount;
        _lightObjects = lightObjects;
        _cameraObjects = cameraObjects;
        _shadowDepthTextures = shadowDepthTextures;
    }

    public override void OnGo(Device device, DeviceContext context)
    {
        foreach (var cameraData in _cameraObjects)
        {
            _currentCameraCBuffer.Update(cameraData.Data);

            var dynamicVariation = new ForwardDynamicVariation(cameraData.Viewport);
            dynamicVariation.Bind(context);

            foreach (var variation in _subVariations)
            {
                variation.Bind(context);

                // updating affecting lights
                UpdateLightsBuffer(_lightObjects);

                // updating depthTextures to affecting textures
                var dynamicSubVariation = new ForwardDynamicSubVariation(
                    variation.PixelShaderStage,
                    _shadowSRVS.ToArray(), _depthSamplers.ToArray());
                dynamicSubVariation.Bind(context);


                if(variation.UseIndexRendering)
                    context.DrawIndexed(variation.IndexCount, 0);
                else
                    context.Draw(variation.VertexCount, 0);

                variation.Unbind(context);
            }
        }
    }

    public override void OnInitialize(Device device, DeviceContext context)
    {
        OutputTexture = device.CreateTexture2D(
            new FSurface(_resolution, Channels.Quad),
            new ResourceUsageInfo()
            {
                Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET
            },
            DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM);

        _outputView = device.CreateRenderTargetView(
            OutputTexture,
            new ViewCreationInfo()
            {
                Format = OutputTexture.Info.Format
            });

        _depthTexture = device.CreateTexture2D(
            new FSurface(_resolution),
            new ResourceUsageInfo()
            {
                Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_DEPTH_STENCIL
            },
            DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT);

        _depthView = device.CreateDepthStencilView(
            _depthTexture,
            new ViewCreationInfo()
            {
                Format = _depthTexture.Info.Format
            });

        _depthState = device.CreateDepthStencilState(
            new DepthStencilStateInfo()
            {
                DepthEnabled = true,
                DepthWriteMask = D3D11_DEPTH_WRITE_MASK.D3D11_DEPTH_WRITE_MASK_ALL,
                DepthComparisionFunc = D3D11_COMPARISON_FUNC.D3D11_COMPARISON_LESS
            });

        _staticVariation = new ForwardVariation(_outputView, _depthState, _depthView);

        _currentCameraCBuffer = Buffer.CreateConstantBuffer(
            device.CreateBuffer(new CameraConstantData().ToSurface(), typeof(CameraConstantData),
            new ResourceUsageInfo()
            {
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
                CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE
            }));

        var length = new LightData().GetFragmentsCount() * _maxPerVariationLightsCount;
        _lightsBuffer = device.CreateBuffer(
            new FSurface(new(length, 1)), typeof(LightData),
            new ResourceUsageInfo()
            {
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE,
                CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE,
                MiscFlags = D3D11_RESOURCE_MISC_FLAG.D3D11_RESOURCE_MISC_BUFFER_STRUCTURED
            });

        _lightsSRV = device.CreateShaderResourceView(_lightsBuffer,
            new ViewCreationInfo()
            {
                Format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN,
                BufferByteStride = _lightsBuffer.Info.ByteStride,
                BufferBytesSize = _lightsBuffer.Info.BytesSize,
                ViewResourceType = ViewResourceType.Buffer
            });

        AddShadowSRVS(device, _maxPerVariationLightsCount);
    }

    public override void OnReady(Device device, DeviceContext context)
    {
        context.ClearRenderTargetView(_outputView, new());
        context.ClearDepthStencilView(_depthView, new DepthStencilClearInfo()
        {
            ClearFlags = D3D11_CLEAR_FLAG.D3D11_CLEAR_DEPTH,
            Depth = DEPTH_CLEAR_VALUE,
        });

        NormalizeDepthTextures(device);

        _staticVariation.Bind(context);
    }

    private void NormalizeDepthTextures(Device device)
    {
        // if we r in lights limit 
        if (_shadowDepthTextures.Count > _maxPerVariationLightsCount == false)
            return;

        if (_shadowSRVS.Count == _shadowDepthTextures.Count)
            return;

        // need to add more textures
        if (_shadowSRVS.Count < _shadowDepthTextures.Count)
        {
            var addCount = _shadowDepthTextures.Count - _shadowSRVS.Count;
            AddShadowSRVS(device, addCount);

            return;
        }

        var removeCount = _shadowSRVS.Count - _shadowDepthTextures.Count;
        var newDepthCount = _shadowSRVS.Count - removeCount;
        var stableCount = newDepthCount < _maxPerVariationLightsCount ?
                           _maxPerVariationLightsCount - newDepthCount : 0;
        removeCount -= stableCount;

        RemoveShadowSRVS(device, removeCount);
    }

    private void AddShadowSRVS(Device device, int count)
    {
        var startIndex = _shadowSRVS.Count;
        for (var i = 0; i < count; i++)
        {
            _shadowSRVS.Add(device.CreateShaderResourceView(
                _shadowDepthTextures[startIndex + i],
                new ViewCreationInfo()
                {
                    Format = DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT,
                    ViewResourceType = ViewResourceType.Texture2D
                }));

            _depthSamplers.Add(device.CreateSampler(
                new SamplerInfo()
                {
                    Filter = D3D11_FILTER.D3D11_FILTER_MIN_MAG_MIP_POINT,
                    AddressMode = D3D11_TEXTURE_ADDRESS_MODE.D3D11_TEXTURE_ADDRESS_CLAMP
                }));
        }
    }

    private void RemoveShadowSRVS(Device device, int removeCount)
    {
        _shadowSRVS.RemoveRange(_shadowSRVS.Count - removeCount, removeCount);
        _depthSamplers.RemoveRange(_depthSamplers.Count - removeCount, removeCount);
    }

    private void UpdateLightsBuffer(List<LightObject> lightObjects)
    {
        Debug.Assert(lightObjects.Count <= _maxPerVariationLightsCount,
            "Ah, more lights have been sent.");

        var lightsData = new List<LightData>();

        for (var i = 0; i < _maxPerVariationLightsCount; i++)
        {
            if(i >= lightObjects.Count)
            {
                lightsData.Add(new());
                continue;
            }

            lightsData.Add(lightObjects[i].Data);
        }

        var length = _lightsBuffer.Info.SurfaceSize.ToArea();
        var surface = new FSurface(new(length, 1));

        var fragments = new List<Fragment>();
        for (var i = 0; i < lightsData.Count; i++)
        {
            fragments.AddRange(lightsData[i].ToFragments());
        }

        surface.SetLinearFragments(fragments.ToArray());

        _lightsBuffer.Update(surface);
    }

    private PipelineVariation AddNewSubVariation(
        Device device, Material material, Mesh mesh)
    {
        var details = (material, mesh);

        var vertexFragments = details.mesh.ToVertexFragments();
        var verticesSurface = new FSurface(new(vertexFragments.Length, 1));
        verticesSurface.SetLinearFragments(vertexFragments);

        var indexUnits = details.mesh.ToIndexUnits();
        var indicesSurface = new USurface(new(indexUnits.Length, 1));
        indicesSurface.SetLinearUnits(indexUnits);

        var vertexBuffer = Buffer.CreateVertexBuffer(
            device.CreateBuffer(verticesSurface, typeof(Vertex),
            new ResourceUsageInfo()
            {
                Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER
            }));

        var indexBuffer = Buffer.CreateIndexBuffer(
            device.CreateBuffer(indicesSurface, typeof(Index),
            new ResourceUsageInfo()
            {
                Usage = D3D11_USAGE.D3D11_USAGE_IMMUTABLE,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_INDEX_BUFFER
            }));

        var vertexShader = device.CreateVertexShader(
            details.material.VertexShader);
        var pixelShader = device.CreatePixelShader(
            details.material.PixelShader
            );

        var inputLayout = device.CreateInputLayout(
            new InputLayoutInfo()
            {
                Topology = details.material.Topology,
                Layout = new Vertex(),
                VertexShader = vertexShader
            });

        var pixelResourceViews = new List<ShaderResourceView>();
        pixelResourceViews.Add(_lightsSRV);
        pixelResourceViews.AddRange(_shadowSRVS);
        for (var x = 0; x < details.material.PixelTextures.Length; x++)
        {
            pixelResourceViews[x] = device.CreateShaderResourceView(
                details.material.PixelTextures[x],
                new ViewCreationInfo()
                {
                    Format = details.material.PixelTextures[x].Info.Format,
                });
        }
        for (var x = 0; x < details.material.PixelBuffers.Length; x++)
        {
            pixelResourceViews[x] = device.CreateShaderResourceView(
                details.material.PixelBuffers[x],
                new ViewCreationInfo()
                {
                    Format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN,
                    BufferByteStride = details.material.PixelBuffers[x].Info.ByteStride,
                    BufferBytesSize = details.material.PixelBuffers[x].Info.BytesSize,
                    ViewResourceType = ViewResourceType.Buffer
                });
        }

        var pixelSamplers = new List<Sampler>();
        pixelSamplers.AddRange(_depthSamplers);
        pixelSamplers.AddRange(material.PixelSamplers);

        var vertexResourceViews = new List<ShaderResourceView>();
        vertexResourceViews.Add(_lightsSRV);
        for (var x = 0; x < details.material.VertexTextures.Length; x++)
        {
            vertexResourceViews[x] = device.CreateShaderResourceView(
                details.material.VertexTextures[x],
                new ViewCreationInfo()
                {
                    Format = details.material.VertexTextures[x].Info.Format,
                });
        }
        for (var x = 0; x < details.material.VertexBuffers.Length; x++)
        {
            vertexResourceViews[x] = device.CreateShaderResourceView(
                details.material.VertexBuffers[x],
                new ViewCreationInfo()
                {
                    Format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN,
                    BufferByteStride = details.material.VertexBuffers[x].Info.ByteStride,
                    BufferBytesSize = details.material.VertexBuffers[x].Info.BytesSize,
                    ViewResourceType = ViewResourceType.Buffer
                });
        }

        var transformBuffer = Buffer.CreateConstantBuffer(
            device.CreateBuffer(new TransformConstantData().ToSurface(),
            typeof(TransformConstantData),
            new ResourceUsageInfo()
            {
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
                CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE
            }));

        var vertexConstantBuffers = new List<ConstantBuffer>
        {
            _currentCameraCBuffer,
            transformBuffer
        };

        vertexConstantBuffers.AddRange(details.material.VertexConstantBuffers);

        var variation = new ForwardSubVariation(
            inputLayout,
            vertexBuffer,
            indexBuffer,
            vertexShader,
            details.material.VertexSamplers,
            vertexConstantBuffers.ToArray(),
            vertexResourceViews.ToArray(),
            pixelShader,
            pixelSamplers.ToArray(),
            details.material.PixelConstantBuffers,
            pixelResourceViews.ToArray(),
            details.material.UseIndexedRendering
            );

        _subVariations.Add(variation);
        return variation;
    }
}