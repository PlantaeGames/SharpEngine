using System.Diagnostics;

using TerraFX.Interop.DirectX;

using SharpEngineCore.Utilities;

namespace SharpEngineCore.Graphics;

internal sealed class ForwardPass : Pass
{
    private const float DEPTH_CLEAR_VALUE = 1f;

    private readonly Texture2D _outputTexture;
    private RenderTargetView _outputView;

    private Texture2D _depthTexture;
    private DepthStencilView _depthView;
    private DepthStencilState _depthState;

    private readonly int _maxPerVariationLightsCount;

    private readonly List<LightObject> _lightObjects;
    private ConstantBuffer _lightDataCBuffer;

    private readonly List<CameraObject> _cameraObjects;
    private ConstantBuffer _currentCameraCBuffer;

    private DepthPass _depthPass;
    private ShaderResourceView _lightDepthView;

    private PipelineVariation _staticVariation;

    public ForwardPass(Texture2D outputTexture,
        int maxPerVariationLightsCount,
        List<LightObject> lightObjects,
        List<CameraObject> cameraObjects,
        DepthPass depthPass) :
        base()
    {
        _outputTexture = outputTexture;
        _maxPerVariationLightsCount = maxPerVariationLightsCount;
        _lightObjects = lightObjects;
        _cameraObjects = cameraObjects;
        _depthPass = depthPass;
    }

    public override void OnGo(Device device, DeviceContext context)
    {
        foreach (var cameraData in _cameraObjects)
        {
            _currentCameraCBuffer.Update(cameraData.Data);

            var dynamicVariation = new ForwardDynamicVariation(cameraData.Viewport);

            for (var i = 0; i < _lightObjects.Count; i++)
            {
                // TODO: NEED CULLING HERE
                // updating affecting lights
                //                       //

                var light = _lightObjects[i];
                var lightPasses = light.Data.LightType == Light.Point ?
                                   6 : 1;

                _lightDataCBuffer.Update(light.Data);

                for(var j = 0; j < lightPasses; j++)
                {
                    _depthPass.TakePass(device, context, i, j);
                    _staticVariation.Bind(context);
                    dynamicVariation.Bind(context);

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
    }

    public override void OnInitialize(Device device, DeviceContext context)
    {
        _outputView = device.CreateRenderTargetView(
            _outputTexture,
            new ViewCreationInfo()
            {
                Format = _outputTexture.Info.Format
            });

        _depthTexture = device.CreateTexture2D(
            [new FSurface(_outputTexture.Info.Size)],
            new TextureCreationInfo()
            {
                Format = DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT,
                UsageInfo = new ResourceUsageInfo()
                {
                    Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                    BindFlags = D3D11_BIND_FLAG.D3D11_BIND_DEPTH_STENCIL
                }
            });

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

        var blendInfo = new BlendStateInfo();
        blendInfo.RenderTargetBlendDescs[0] = new D3D11_RENDER_TARGET_BLEND_DESC
        {
            BlendEnable = true,
            RenderTargetWriteMask = 0xff,
            
            SrcBlend = D3D11_BLEND.D3D11_BLEND_ONE,
            DestBlend = D3D11_BLEND.D3D11_BLEND_ONE,
            BlendOp = D3D11_BLEND_OP.D3D11_BLEND_OP_ADD
        };
        var blendState = device.CreateBlendState(blendInfo);

        _staticVariation = new ForwardVariation(_outputView, _depthState, _depthView,
                                                blendState);

        _currentCameraCBuffer = Buffer.CreateConstantBuffer(
            device.CreateBuffer(new CameraConstantData().ToSurface(), typeof(CameraConstantData),
            new ResourceUsageInfo()
            {
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
                CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE
            }));

        _lightDataCBuffer = Buffer.CreateConstantBuffer(
            device.CreateBuffer(
                new LightConstantData().ToSurface(), typeof(LightConstantData),
            new ResourceUsageInfo()
            {
                Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
                CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE
            }));

        _lightDepthView = device.CreateShaderResourceView(_depthPass.DepthTexture,
            new ViewCreationInfo()
            {
                Format = DXGI_FORMAT.DXGI_FORMAT_R32_FLOAT,
                ViewResourceType = ViewResourceType.Texture2D
            });
    }

    public override void OnReady(Device device, DeviceContext context)
    {
        context.ClearDepthStencilView(_depthView, new DepthStencilClearInfo()
        {
            ClearFlags = D3D11_CLEAR_FLAG.D3D11_CLEAR_DEPTH,
            Depth = DEPTH_CLEAR_VALUE,
        });

/*        context.ClearRenderTargetView(_outputView,
            new(1,1,0.4f,1));*/
    }

    private PipelineVariation AddNewSubVariation(
        Device device, Material material, Mesh mesh, ConstantBuffer transformBuffer)
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
        pixelResourceViews.Add(_lightDepthView);

        for (var x = 0; x < details.material.PixelTextures.Length; x++)
        {
            pixelResourceViews.Add(device.CreateShaderResourceView(
                details.material.PixelTextures[x],
                new ViewCreationInfo()
                {
                    Format = details.material.PixelTextures[x].Info.Format,
                }));
        }
        for (var x = 0; x < details.material.PixelBuffers.Length; x++)
        {
            pixelResourceViews.Add(device.CreateShaderResourceView(
                details.material.PixelBuffers[x],
                new ViewCreationInfo()
                {
                    Format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN,
                    BufferByteStride = details.material.PixelBuffers[x].Info.ByteStride,
                    BufferBytesSize = details.material.PixelBuffers[x].Info.BytesSize,
                    ViewResourceType = ViewResourceType.Buffer
                }));
        }

        var pixelSamplers = new List<Sampler>();
        pixelSamplers.Add(_depthPass.DepthSampler);
        pixelSamplers.AddRange(material.PixelSamplers);

        var vertexResourceViews = new List<ShaderResourceView>();

        for (var x = 0; x < details.material.VertexTextures.Length; x++)
        {
            vertexResourceViews.Add(device.CreateShaderResourceView(
                details.material.VertexTextures[x],
                new ViewCreationInfo()
                {
                    Format = details.material.VertexTextures[x].Info.Format,
                }));
        }
        for (var x = 0; x < details.material.VertexBuffers.Length; x++)
        {
            vertexResourceViews.Add(device.CreateShaderResourceView(
                details.material.VertexBuffers[x],
                new ViewCreationInfo()
                {
                    Format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN,
                    BufferByteStride = details.material.VertexBuffers[x].Info.ByteStride,
                    BufferBytesSize = details.material.VertexBuffers[x].Info.BytesSize,
                    ViewResourceType = ViewResourceType.Buffer
                }));
        }

        var vertexConstantBuffers = new List<ConstantBuffer>();
        vertexConstantBuffers.Add(_lightDataCBuffer);
        vertexConstantBuffers.Add(_currentCameraCBuffer);
        vertexConstantBuffers.Add(transformBuffer);
        vertexConstantBuffers.AddRange(material.VertexConstantBuffers);

        var pixelConstantBuffers = new List<ConstantBuffer>();
        pixelConstantBuffers.Add(_lightDataCBuffer);
        pixelConstantBuffers.AddRange(material.PixelConstantBuffers);

        //var transformBuffer = Buffer.CreateConstantBuffer(
        //    device.CreateBuffer(new TransformConstantData().ToSurface(),
        //    typeof(TransformConstantData),
        //    new ResourceUsageInfo()
        //    {
        //        Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC,
        //        BindFlags = D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
        //        CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE
        //    }));

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
            pixelConstantBuffers.ToArray(),
            pixelResourceViews.ToArray(),
            details.material.UseIndexedRendering
            );

        _subVariations.Add(variation);
        return variation;
    }

    public override void OnCameraAdd(CameraObject camera, Device device)
    {
    }

    public override void OnCameraPause(CameraObject camera, Device device)
    {
    }

    public override void OnCameraRemove(CameraObject camera, Device device)
    {
    }

    public override void OnCameraResume(CameraObject camera, Device device)
    {
    }

    public override void OnGraphicsAdd(GraphicsObject graphics, Device device)
    {
        var variation = AddNewSubVariation(
            device, graphics.Info.material, graphics.Info.mesh, graphics.GetTransformBuffer());
        graphics.AddVariation(variation);
    }

    public override void OnGraphicsPause(GraphicsObject graphics, Device device)
    {
    }

    public override void OnGraphicsRemove(GraphicsObject graphics, Device device)
    {
    }

    public override void OnGraphicsResume(GraphicsObject graphics, Device device)
    {
    }

    public override void OnLightAdd(LightObject light, Device device)
    {
    }

    public override void OnLightPause(LightObject light, Device device)
    {
    }

    public override void OnLightRemove(LightObject light, Device device)
    {
    }

    public override void OnLightResume(LightObject light, Device device)
    {
    }
    public override void OnSkyboxSet(CubemapInfo info, Device device)
    {
    }
}