using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class Swapchain
{
    private ComPtr<IDXGISwapChain> _pSwapchain;

    private readonly (float r, float g, float b, float a) CLEAR_COLOR = (0f, 0f, 0f, 0f);

    public void Present()
    {
        NativePresent();

        unsafe void NativePresent()
        {
            fixed(IDXGISwapChain** ppSwapchain = _pSwapchain)
            {
                (*ppSwapchain)->Present(1u, 0u);
            }
        }
    }

    public Swapchain(ComPtr<IDXGISwapChain> pSwapchain)
    {
        _pSwapchain = pSwapchain;
    }
}
