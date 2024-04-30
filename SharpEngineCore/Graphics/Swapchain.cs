using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class Swapchain
{
    private ComPtr<IDXGISwapChain> _pSwapchain;
    private Window _window;
    private Texture _backTexture;

    private readonly (float r, float g, float b, float a) CLEAR_COLOR = (0f, 0f, 0f, 0f);

    private void GetBackTexture()
    {
        NativeGetBackTexture();

        unsafe void NativeGetBackTexture()
        {
            fixed(IDXGISwapChain** ppSwapchain = _pSwapchain)
            {
                var uuid = typeof(IDXGIResource).GUID;

                var pBackTexture = new ComPtr<ID3D11Texture2D>();
                fixed (ID3D11Texture2D** ppTexture = pBackTexture)
                {
                    GraphicsException.SetInfoQueue();
                    var result = (*ppSwapchain)->GetBuffer(0u, &uuid, (void**)ppTexture);

                    if(result.FAILED)
                    {
                        // error here
                        throw GraphicsException.GetLastGraphicsException(
                            new GraphicsException($"Failed to obtain back buffer of swapchain\nError Code: {result}"));
                    }
                }

                _backTexture = new Texture2D(pBackTexture,
                    new TextureInfo() { Size = _window.GetSize() });
            }
        }
    }

    public void Present()
    {
        NativePresent();

        unsafe void NativePresent()
        {
            fixed(IDXGISwapChain** ppSwapchain = _pSwapchain)
            {
                GraphicsException.SetInfoQueue();
                var result = (*ppSwapchain)->Present(1u, 0u);

                if(result.FAILED)
                {
                    throw GraphicsException.GetLastGraphicsException(
                        new GraphicsException($"Failed to present swapchain\nError Code: {result}"));
                }
            }
        }
    }

    public Swapchain(ComPtr<IDXGISwapChain> pSwapchain, Window window)
    {
        _pSwapchain = pSwapchain;
        _window = window;
    }
}
