using System.Diagnostics;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

internal sealed class ComUtilities
{
    public static ComPtr<ID3D11Resource> ToResourceNativePtr<T>(ComPtr<T> nativePtr)
        where T : unmanaged, IUnknown.Interface
    {
        return NativeToResource();

        unsafe ComPtr<ID3D11Resource> NativeToResource()
        {
            var ptr = new ComPtr<ID3D11Resource>();
            var result = nativePtr.As(&ptr);

            Debug.Assert(result.FAILED == false,
                $"Failed to query {nameof(ID3D11Resource)}" +
                $" Interface from {nameof(ID3D11Texture2D)}.");

            return ptr;
        }
    }
}
