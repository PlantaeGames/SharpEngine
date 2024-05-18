using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;
internal sealed class ForwardPass : Pass
{
    private const float DEPTH_CLEAR_VALUE = 1f;

    public Texture2D OutputTexture { get; private set; }
    private RenderTargetView _outputView;

    private Texture2D _depthTexture;
    private DepthStencilView _depthView;
    private DepthStencilState _depthState;

    private ShaderResourceView _lightsSRV;

    private ConstantBuffer _currentCameraCBuffer;

    private PipelineVariation _staticVariation;

    private readonly Size _resolution;

    private readonly Queue<(Guid id, Material material, Mesh mesh)> _queue = new();

    private readonly List<(ConstantBuffer dataBuffer, CameraObject camera)> _cameraDataBuffers;


    public Guid CreateNewGraphicsObject(Material material, Mesh mesh)
    {
        var id = Guid.NewGuid();
        _queue.Enqueue((id, material, mesh));

        return id;
    }

    public ForwardPass(Size resolution,
        ShaderResourceView lightsResourceView,
        List<(ConstantBuffer dataBuffer, CameraObject camera)> cameraDataBuffers) :
        base()
    {
        _resolution = resolution;
        _lightsSRV = lightsResourceView;
        _cameraDataBuffers = cameraDataBuffers;
    }

    public override void OnGo(Device device, DeviceContext context)
    {
        foreach (var cameraData in _cameraDataBuffers)
        {
            if (cameraData.camera.State == State.Paused)
                continue;

            context.CopyResource(cameraData.dataBuffer, _currentCameraCBuffer);

            var dynamicVariation = new ForwardDynamicVariation(cameraData.camera.Viewport);
            dynamicVariation.Bind(context);

            foreach (var graphicsObject in GraphicsObjects)
            {
                graphicsObject._variation.Bind(context);
                if(graphicsObject._variation.UseIndexRendering)
                    context.DrawIndexed(graphicsObject._variation.IndexCount, 0);
                else
                    context.Draw(graphicsObject._variation.VertexCount, 0);
            }
        }
    }

    public override void OnInitialize(Device device, DeviceContext context)
    {
        OutputTexture = device.CreateTexture2D(
            new FSurface(_resolution),
            new ResourceUsageInfo()
            {
                Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT,
                BindFlags = D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET
            },
            DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM);

        _outputView = device.CreateRenderTargetView(
            OutputTexture,
            new ViewCreationInfo()
            {
                Format = OutputTexture.Info.Format,
                Size = OutputTexture.Info.Size
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
                Format = _depthTexture.Info.Format,
                Size = _depthTexture.Info.Size
            });

        _depthState = device.CreateDepthStencilState(
            new DepthStencilStateInfo()
            {

                DepthEnabled = true,
                DepthWriteMask = D3D11_DEPTH_WRITE_MASK.D3D11_DEPTH_WRITE_MASK_ALL,
                DepthComparisionFunc = D3D11_COMPARISON_FUNC.D3D11_COMPARISON_GREATER
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
    }

    public override void OnReady(Device device, DeviceContext context)
    {
        context.ClearRenderTargetView(_outputView, new());
        context.ClearDepthStencilView(_depthView, new DepthStencilClearInfo()
        {
            ClearFlags = D3D11_CLEAR_FLAG.D3D11_CLEAR_DEPTH,
            Depth = DEPTH_CLEAR_VALUE,
        });

        ClearQueue(device, context);
        _staticVariation.Bind(context);
    }

    private void ClearQueue(Device device, DeviceContext context)
    {  
        for(var i = 0; i < _queue.Count; i++)
        {
            var details = _queue.Dequeue();

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
            for(var x = 1; x < details.material.PixelBuffers.Length; x++)
            {
                pixelResourceViews[x] = device.CreateShaderResourceView(
                    details.material.PixelBuffers[i],
                    new ViewCreationInfo()
                    { 
                        Format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN,
                        Size = details.material.PixelBuffers[i].Info.Size
                    });
            }

            var vertexResourceViews = new List<ShaderResourceView>();
            for (var x = 1; x < details.material.VertexBuffers.Length; x++)
            {
                vertexResourceViews[x] = device.CreateShaderResourceView(
                    details.material.VertexBuffers[i],
                    new ViewCreationInfo()
                    {
                        Format = DXGI_FORMAT.DXGI_FORMAT_UNKNOWN,
                        Size = details.material.VertexBuffers[i].Info.Size
                    });
            }

            var transformConstantBuffer = Buffer.CreateConstantBuffer(
                device.CreateBuffer(
                    new TransformConstantData().ToSurface(),
                    typeof(TransformConstantData),
                    new ResourceUsageInfo()
                    {
                        Usage = D3D11_USAGE.D3D11_USAGE_DYNAMIC,
                        BindFlags = D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
                        CPUAccessFlags = D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE
                    }));

            var vertexConstantBuffers = new List<ConstantBuffer>
            {
                transformConstantBuffer,
                _currentCameraCBuffer
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
                details.material.PixelSamplers,
                details.material.PixelConstantBuffers,
                pixelResourceViews.ToArray(),
                details.material.UseIndexedRendering
                );

            var graphicsObject = new GraphicsObject(
                details.id, transformConstantBuffer, variation);

            _toAdd.Add(graphicsObject);
        }
    }
}