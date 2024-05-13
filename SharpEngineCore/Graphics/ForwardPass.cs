namespace SharpEngineCore.Graphics;
internal sealed class ForwardPass : Pass
{
    private RenderTargetView _outputView;

    private DepthStencilState _depthState;
    private DepthStencilView _depthView;

    private PipelineVariation _staticVariation;
    private Queue<ForwardSubVariationCreateInfo> _installment;

    public List<GraphicsObject> GraphicsObjects { get; private set; }
    public PerspectiveCamera PerspectiveCamera { get; private set; }

    public Guid AddSubVariation(ForwardSubVariationCreateInfo info)
    {
        _installment.Enqueue(info);
        return info.Id;
    }

    public ForwardPass(RenderTargetView outputView, 
        DepthStencilState depthState, DepthStencilView depthView) :
        base()
    {
        _installment = new();
        _subVariations = new();
        GraphicsObjects = new();
        PerspectiveCamera = new (outputView.Info.ResourceViewInfo.Size);

        _outputView = outputView;
        _depthState = depthState;
        _depthView = depthView;
    }

    public override void OnGo(Device device, DeviceContext context)
    {
        foreach (var varitation in _subVariations)
        {
            varitation.Bind(context);

            context.Draw(varitation.VertexCount, 0);
        }
    }

    public override void OnInitialize(Device device, DeviceContext context)
    {
        var vertexShader = device.CreateVertexShader(new ShaderModule(
            "Shaders\\VertexShader.hlsl"
            ));
        var pixelShader = device.CreatePixelShader(new ShaderModule(
            "Shaders\\PixelShader.hlsl"
            ));

        var layout = device.CreateInputLayout(
            new InputLayoutInfo
            {
                Layout = new Vertex(),
                Topology = Topology.TriangleList,
                VertexShader = vertexShader
            });

        var viewport = PerspectiveCamera.Viewport;

        var lightData = new LightConstantData();
        lightData.Color = new(1, 1, 1, 1);
        lightData.AmbientColor = new(0.15f, 0.15f, 0.15f, 0.15f);
        lightData.Position = new(0, 100, 0, 0);
        lightData.Rotation = new(0, 0, 0, 0);

        var lightSurface = lightData.ToSurface();
        var lightConstantBuffer = Buffer.CreateConstantBuffer(
            device.CreateBuffer(lightSurface, typeof(LightConstantData),
            new ResourceUsageInfo()
            {
                BindFlags = TerraFX.Interop.DirectX.D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
                Usage = TerraFX.Interop.DirectX.D3D11_USAGE.D3D11_USAGE_IMMUTABLE,
            }));

        _staticVariation = new ForwardVariation(
            layout,
            vertexShader, pixelShader, lightConstantBuffer,
            viewport, _outputView, _depthState, _depthView);
    }

    public override void OnReady(Device device, DeviceContext context)
    {
        ClearInstallments(device);

        _staticVariation.Bind(context);
    }

    private void ClearInstallments(Device device)
    {
        if (_installment.Count == 0)
            return;

        for (var i = 0; i < _installment.Count; i++)
        {
            var info = _installment.Dequeue();

            // getting vertices
            var vertexFragments = info.Mesh.ToVertexFragments();
            var vertices = new FSurface(new(vertexFragments.Length, 1));
            vertices.SetLinearFragments(vertexFragments);

            // getting indices
            var indexUnits = info.Mesh.ToIndexUnits();
            var indices = new USurface(new(indexUnits.Length, 1));
            indices.SetLinearUnits(indexUnits);

            // creating buffers
            var vertexBuffer = Buffer.CreateVertexBuffer(
                device.CreateBuffer(vertices, typeof(Vertex), new ResourceUsageInfo()
                {
                    BindFlags = TerraFX.Interop.DirectX.D3D11_BIND_FLAG.D3D11_BIND_VERTEX_BUFFER,
                    Usage = TerraFX.Interop.DirectX.D3D11_USAGE.D3D11_USAGE_IMMUTABLE
                }));

            var indexBuffer = Buffer.CreateIndexBuffer(
                device.CreateBuffer(indices, typeof(Index), new ResourceUsageInfo()
                {
                    BindFlags = TerraFX.Interop.DirectX.D3D11_BIND_FLAG.D3D11_BIND_INDEX_BUFFER,
                    Usage = TerraFX.Interop.DirectX.D3D11_USAGE.D3D11_USAGE_IMMUTABLE
                }));

            var transform = new TransformConstantData();
            transform.Scale = new FColor4(1f, 1f, 1f, 1f);
            transform.W = new FColor4((float)_outputView.Info.ResourceViewInfo.Size.Height /
                                      (float)_outputView.Info.ResourceViewInfo.Size.Width,
                                       70f,
                                       0.1f,
                                       1000f);

            var transformSurface = transform.ToSurface();
            var transformConstantBuffer = Buffer.CreateConstantBuffer(device.CreateBuffer(
                transformSurface, typeof(TransformConstantData), new ResourceUsageInfo()
                {
                    BindFlags = TerraFX.Interop.DirectX.D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
                    Usage = TerraFX.Interop.DirectX.D3D11_USAGE.D3D11_USAGE_DYNAMIC,
                    CPUAccessFlags = TerraFX.Interop.DirectX.D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE
                }
                ));

            var camTransformConstantBuffer = Buffer.CreateConstantBuffer(device.CreateBuffer(
                transformSurface, typeof(TransformConstantData), new ResourceUsageInfo()
                {
                    BindFlags = TerraFX.Interop.DirectX.D3D11_BIND_FLAG.D3D11_BIND_CONSTANT_BUFFER,
                    Usage = TerraFX.Interop.DirectX.D3D11_USAGE.D3D11_USAGE_DYNAMIC,
                    CPUAccessFlags = TerraFX.Interop.DirectX.D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE
                }
                ));

            var graphicsObject = new GraphicsObject(transformConstantBuffer, info.Id);
            GraphicsObjects.Add(graphicsObject);

            var variation = new ForwardSubVariation(vertexBuffer, indexBuffer,
                transformConstantBuffer, camTransformConstantBuffer);
            _subVariations.Enqueue(variation);
        }
    }
}