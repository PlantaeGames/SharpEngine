using System.Diagnostics;

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class Swapchain
{
    private Device _device;
    private Window _window;
    private Texture2D _backTexture;

    private ComPtr<IDXGISwapChain> _pSwapchain;

    public Texture2D GetBackTexture() => _backTexture;

    private void CreateBackTexture()
    {
        NativeCreateBackTexture();

        unsafe void NativeCreateBackTexture()
        {
            fixed(IDXGISwapChain** ppSwapchain = _pSwapchain)
            {
                var result = new HRESULT();

                var uuid = typeof(IDXGIResource).GUID;
                var pBackTextureResource = new ComPtr<ID3D11Resource>();
                fixed (ID3D11Resource** ppResource = pBackTextureResource)
                {
                    GraphicsException.SetInfoQueue();
                    result = (*ppSwapchain)->GetBuffer(0u, &uuid, (void**)ppResource);

                    if(result.FAILED)
                    {
                        // error here
                        GraphicsException.ThrowLastGraphicsException(
                            $"Failed to obtain back buffer of swapchain\nError Code: {result}");
                    }
                }

                var pBackTexture = new ComPtr<ID3D11Texture2D>();
                result = pBackTextureResource.As(&pBackTexture);

                Debug.Assert(result.FAILED == false,
                    "Failed to query ID311Texture2D from ID3D11Resource.");

                _backTexture = new Texture2D(
                    pBackTexture,
                    new() 
                    { 
                        Size = _window.GetSize()
                    },
                    _device);
            }
        }
    }

    public void Present(uint syncInterval = 1u)
    {
        NativePresent();

        unsafe void NativePresent()
        {
            fixed(IDXGISwapChain** ppSwapchain = _pSwapchain)
            {
                GraphicsException.SetInfoQueue();
                var result = (*ppSwapchain)->Present(syncInterval, 0u);

                if(result.FAILED)
                {
                    GraphicsException.ThrowLastGraphicsException(
                        $"Failed to present swapchain\nError Code: {result}");
                }
            }
        }
    }

    public Swapchain(ComPtr<IDXGISwapChain> pSwapchain, Window window, Device device)
    {
        _pSwapchain = pSwapchain;
        _device = device;
        _window = window;

        CreateBackTexture();
    }
}