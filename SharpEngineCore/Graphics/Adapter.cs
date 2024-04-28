using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

using SharpEngineCore.Exceptions;

namespace SharpEngineCore.Graphics;

internal sealed class Adapter
{
    private readonly ComPtr<IDXGIAdapter> _pAdapter;

    public Adapter(ComPtr<IDXGIAdapter> pAdapter)
    {
        _pAdapter = pAdapter;
    }

    public DXGI_ADAPTER_DESC GetDescription()
    {
        return NativeGetDescription();

        unsafe DXGI_ADAPTER_DESC NativeGetDescription()
        {
            fixed (IDXGIAdapter** ppAdatper = _pAdapter)
            {
                DXGI_ADAPTER_DESC description = new DXGI_ADAPTER_DESC();

                GraphicsSharpException.SetInfoQueue();
                var result = (*ppAdatper)->GetDesc(&description);

                if(Errors.CheckHResult(result) == false)
                {
                    // error here
                    throw GraphicsSharpException.GetLastGraphicsException(
                        new GraphicsSharpException($"Failed to get adapter description.\nError Code: {result}"));
                }

                return description;
            }
        }
    }
}
