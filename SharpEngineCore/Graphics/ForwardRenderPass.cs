using TerraFX.Interop.DirectX;

namespace SharpEngineCore.Graphics;

internal sealed class ForwardRenderPass : RenderPass
{
    private Texture2D _outputTexture;
    private Texture2D _depthTexture;

    private RenderTargetView _outputView;

    private DepthStencilState _depthState;
    private DepthStencilView _depthView;

    public ForwardRenderPass(Texture2D outputTexture) :
        base()
    {
        _outputTexture = outputTexture;
    }

    public override void OnGo(Device device, DeviceContext context)
    {
    }

    public override void OnInitialize(Device device, DeviceContext context)
    {
        _outputView = device.CreateRenderTargetView(_outputTexture, new ViewCreationInfo()
        {
            Size = _outputTexture.Info.Size,
            Format = _outputTexture.Info.Format
        });

        _depthTexture = device.CreateTexture2D(
            new(_outputTexture.Info.Size, _outputTexture.Info.Channels),
            new ResourceUsageInfo()
            {
                Usage = TerraFX.Interop.DirectX.D3D11_USAGE.D3D11_USAGE_DEFAULT,
                BindFlags = TerraFX.Interop.DirectX.D3D11_BIND_FLAG.D3D11_BIND_DEPTH_STENCIL
            },
            DXGI_FORMAT.DXGI_FORMAT_D32_FLOAT);

        _depthState = device.CreateDepthStencilState(new DepthStencilStateInfo()
        {
            DepthEnabled = true,
            DepthWriteMask = TerraFX.Interop.DirectX.D3D11_DEPTH_WRITE_MASK.D3D11_DEPTH_WRITE_MASK_ALL,
            DepthComparisionFunc = TerraFX.Interop.DirectX.D3D11_COMPARISON_FUNC.D3D11_COMPARISON_GREATER

        });
        _depthView = device.CreateDepthStencilView(_depthTexture, new ViewCreationInfo()
        {
            Size = _depthTexture.Info.Size,
            Format = _depthTexture.Info.Format
        });

        var forwardPass = new ForwardPass(_outputView, _depthState, _depthView);
        _passes = [forwardPass];
    }

    public override void OnReady(Device device, DeviceContext context)
    {
        context.ClearRenderTargetView(_outputView, new());
        context.ClearDepthStencilView(_depthView, new DepthStencilClearInfo()
        {
            Depth = 1f,
            ClearFlags = TerraFX.Interop.DirectX.D3D11_CLEAR_FLAG.D3D11_CLEAR_DEPTH
        });
    }
}
