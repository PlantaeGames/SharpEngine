﻿namespace SharpEngineCore.Graphics;
internal sealed class ForwardPass : Pass
{
    private Texture2D _output;

    private PipelineVariation _staticVariation;
    private PipelineVariation _dynamicVariation;

    private Queue<ForwardSubVariationCreateInfo> _installment;

    public void AddSubVariation(ForwardSubVariationCreateInfo info)
    {
        _installment.Enqueue(info);
    }

    public ForwardPass(Texture2D output)
        : base()
    {
        _installment = new();
        _subVariations = new();

        _output = output;
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
            "Shaders\\VertexShader.cso",
            true
            ));
        var pixelShader = device.CreatePixelShader(new ShaderModule(
            "Shaders\\PixelShader.cso",
            true
            ));

        var layout = device.CreateInputLayout(
            new InputLayoutInfo
            {
                Layout = new Vertex(),
                Topology = Topology.TriangleList,
                VertexShader = vertexShader
            });

        var viewport = new Viewport()
        {
            Info = new TerraFX.Interop.DirectX.D3D11_VIEWPORT()
            {
                TopLeftX = 1f,
                TopLeftY = 1f,
                Width = _output.Info.Size.Width,
                Height = _output.Info.Size.Height,
                MinDepth = 0f,
                MaxDepth = 1f
            }
        };

        var targetView = device.CreateRenderTargetView(_output);

        _staticVariation = new ForwardVariation(
            layout,
            vertexShader, pixelShader,
            viewport, targetView);
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

            var variation = new ForwardSubVariation(vertexBuffer, indexBuffer);
            _subVariations.Enqueue(variation);
        }
    }
}