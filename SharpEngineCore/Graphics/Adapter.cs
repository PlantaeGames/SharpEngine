﻿using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class Adapter
{
    private readonly ComPtr<IDXGIAdapter> _pAdapter;

    public ComPtr<IDXGIAdapter> GetNativePtr() => _pAdapter;

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

                GraphicsException.SetInfoQueue();
                var result = (*ppAdatper)->GetDesc(&description);

                if (result.FAILED)
                {
                    // error here
                    throw GraphicsException.GetLastGraphicsException(
                        new GraphicsException($"Failed to get adapter description.\nError Code: {result}"));
                }

                return description;
            }
        }
    }
}
