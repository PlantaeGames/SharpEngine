﻿using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal sealed class ForwardPass : Pass
{
    private const float DEPTH_CLEAR_VALUE = 1f;

    private readonly int _maxPerVariationLightsCount;

    private readonly List<LightObject> _lightObjects;
    private ConstantBuffer _lightDataCBuffer;

    private readonly List<CameraObject> _cameraObjects;
    private ConstantBuffer _currentCameraCBuffer;

    private DepthPass _depthPass;
    private ShaderResourceView _lightDepthView;

    private PipelineVariation _staticVariation;

    private ConstantBuffer _lightingSwitchCBuffer;

    private Dictionary<CameraObject, (DepthStencilView depthView, RenderTargetView outputView)> _outputViews = new();

    public ForwardPass(
        int maxPerVariationLightsCount,
        List<LightObject> lightObjects,
        List<CameraObject> cameraObjects,
        DepthPass depthPass) :
        base()
    {
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

            var views = _outputViews[cameraData];
            var dynamicVariation = new ForwardDynamicVariation(
                views.depthView,
                views.outputView,
                cameraData.Viewport);

            for (var i = 0; i < _lightObjects.Count; i++)
            {
                // TODO: NEED CULLING HERE
                // updating affecting lights
                //                       //

                var light = _lightObjects[i];
                var lightPasses = light.Data.LightType == LightInfo.Point ?
                                   6 : 1;

                //_lightDataCBuffer.Update(light.Data);

                for(var j = 0; j < lightPasses; j++)
                {
                    var rotation = Utilities.Utilities.CubeFaceIndexToAngle(j);
                    _lightDataCBuffer.Update(new LightConstantData()
                    {
                        Position = light.Data.Position,
                        Rotation = rotation,
                        Scale = light.Data.Scale,
                        LightType = light.Data.LightType,
                        Attributes = light.Data.Attributes,
                        AmbientColor = light.Data.AmbientColor,
                        Color = light.Data.Color,
                        Intensity = light.Data.Intensity
                    });

                    _depthPass.TakePass(device, context, i, j);

                    var outputMerger = _staticVariation.OutputMerger;
                    if (i < 1)
                    {
                        outputMerger.ToggleDepthStencilState(true);
                        outputMerger.ToggleBlendState(false);
                    }
                    else
                    {
                        outputMerger.ToggleDepthStencilState(false);
                        outputMerger.ToggleBlendState(true);
                    }

                    if(j < 1)
                    {
                        _lightingSwitchCBuffer.Update(
                            new PixelShaderPassSwitchData()
                            {
                                LightingSwitch = new(1, 0, 0, 0)
                            });
                    }
                    else
                    {
                        _lightingSwitchCBuffer.Update(
                            new PixelShaderPassSwitchData()
                            {
                                LightingSwitch = new(0, 0, 0, 0)
                            });
                    }

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
        var depthInfoOn = new DepthStencilStateInfo()
        {
            DepthEnabled = true,
            DepthWriteMask = D3D11_DEPTH_WRITE_MASK.D3D11_DEPTH_WRITE_MASK_ALL,
            DepthComparisionFunc = D3D11_COMPARISON_FUNC.D3D11_COMPARISON_LESS
        };
        var depthInfoOff = new DepthStencilStateInfo()
        {
            DepthEnabled = true,
            DepthWriteMask = D3D11_DEPTH_WRITE_MASK.D3D11_DEPTH_WRITE_MASK_ZERO,
            DepthComparisionFunc = D3D11_COMPARISON_FUNC.D3D11_COMPARISON_LESS_EQUAL
        };
        var depthOn = device.CreateDepthStencilState(depthInfoOn);
        var depthOff = device.CreateDepthStencilState(depthInfoOff);

        var blendInfo = new BlendStateInfo()
        {
            BlendFactor = new(1, 1, 1, 1)
        };
        blendInfo.RenderTargetBlendDescs[0] = new D3D11_RENDER_TARGET_BLEND_DESC
        {
            BlendEnable = true,
            RenderTargetWriteMask = (byte)D3D11_COLOR_WRITE_ENABLE.D3D11_COLOR_WRITE_ENABLE_ALL,
            
            SrcBlend = D3D11_BLEND.D3D11_BLEND_SRC_COLOR,
            DestBlend = D3D11_BLEND.D3D11_BLEND_DEST_COLOR,
            BlendOp = D3D11_BLEND_OP.D3D11_BLEND_OP_ADD,

            SrcBlendAlpha = D3D11_BLEND.D3D11_BLEND_SRC_ALPHA,
            DestBlendAlpha = D3D11_BLEND.D3D11_BLEND_DEST_ALPHA,
            BlendOpAlpha = D3D11_BLEND_OP.D3D11_BLEND_OP_ADD
        };

        var blendOn = device.CreateBlendState(blendInfo);
        blendInfo.RenderTargetBlendDescs[0].BlendEnable = false;
        var blendOff = device.CreateBlendState(blendInfo);

        _staticVariation = new ForwardVariation(depthOn, depthOff,
                                                blendOn, blendOff);

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

        _lightingSwitchCBuffer = Buffer.CreateConstantBuffer(
            device.CreateBuffer(
                new PixelShaderPassSwitchData().ToSurface(), typeof(PixelShaderPassSwitchData),
                new ResourceUsageInfo()
                {
                    Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC,
                    BindFlags = D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
                    CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE
                }));
    }

    public override void OnReady(Device device, DeviceContext context)
    {
        foreach (var view in _outputViews)
        {
            context.ClearDepthStencilView(view.Value.depthView, new DepthStencilClearInfo()
            {
                ClearFlags = D3D11_CLEAR_FLAG.D3D11_CLEAR_DEPTH,
                Depth = DEPTH_CLEAR_VALUE,
            });
        }

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
        vertexConstantBuffers.Add(transformBuffer);
        vertexConstantBuffers.Add(_currentCameraCBuffer);
        vertexConstantBuffers.AddRange(material.VertexConstantBuffers);

        var pixelConstantBuffers = new List<ConstantBuffer>();
        pixelConstantBuffers.Add(_lightDataCBuffer);
        pixelConstantBuffers.Add(_lightingSwitchCBuffer);
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

        var rasterizer = device.CreateRasterizerState(
            new RasterizerStateInfo()
            {
                CullMode = material.CullMode,
                FillMode = material.FillMode
            });

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
            rasterizer,
            details.material.UseIndexedRendering            
            );

        _subVariations.Add(variation);
        return variation;
    }

    public override void OnCameraAdd(CameraObject camera, Device device)
    {
        var depthTexture = device.CreateTexture2D(
                [new FSurface(camera.RenderTexture.Info.Size)],
                new TextureCreationInfo()
                {
                    Format = DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT,
                    UsageInfo = new ResourceUsageInfo()
                    {
                        Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                        BindFlags = D3D11_BIND_FLAG.D3D11_BIND_DEPTH_STENCIL
                    }

                });
        var depthView = device.CreateDepthStencilView(depthTexture,
                new ViewCreationInfo()
                {
                    Format = depthTexture.Info.Format,
                    TextureMipLevels = depthTexture.Info.MipLevels,
                });

        var renderTargetView = device.CreateRenderTargetView(camera.RenderTexture,
                new ViewCreationInfo()
                {
                    Format = camera.RenderTexture.Info.Format,
                    TextureMipLevels = camera.RenderTexture.Info.MipLevels
                });

        _outputViews.Add(camera, (depthView, renderTargetView));
    }

    public override void OnCameraPause(CameraObject camera, Device device)
    {
    }

    public override void OnCameraRemove(CameraObject camera, Device device)
    {
        _outputViews.Remove(camera);
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