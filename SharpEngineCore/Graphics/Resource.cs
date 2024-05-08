using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

public abstract class Resource
{
    private readonly ComPtr<ID3D11Resource> _pResource;

    public D3D11_RESOURCE_DIMENSION GetResourceType()
    {
        return NativeGetType();

        unsafe D3D11_RESOURCE_DIMENSION NativeGetType()
        {
            var type = new D3D11_RESOURCE_DIMENSION();

            fixed (ID3D11Resource** ppResource = _pResource)
            {
                (*ppResource)->GetType(&type);
            }

            return type;
        }
    }

    internal ComPtr<ID3D11Resource> GetNativePtrAsResource() => new(_pResource);

    protected Resource(ComPtr<ID3D11Resource> pResource)
    { 
        _pResource = new(pResource);
    }
}
