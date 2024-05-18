using System.Diagnostics;

using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

using SharpEngineCore.Utilities;

namespace SharpEngineCore.Graphics;

public abstract class Resource
{
    public readonly ResourceInfo ResourceInfo;

    private readonly ComPtr<ID3D11Resource> _pResource;
    private protected readonly Device _device;

    private protected void Write(Surface surface, int subIndex)
    {
        Debug.Assert(ResourceInfo.UsageInfo.Usage == D3D11_USAGE.D3D11_USAGE_DYNAMIC ||
                     ResourceInfo.UsageInfo.Usage == D3D11_USAGE.D3D11_USAGE_STAGING,
            "Updating buffers from cpu is only available through Dynamic and staging resources.");
        Debug.Assert(
            ResourceInfo.UsageInfo.CPUAccessFlags.HasFlag(D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_WRITE),
            "Does not have CPU Write access on this buffer.");
        Debug.Assert(surface.Size.ToArea() == ResourceInfo.Size.ToArea(),
            "Buffer and sent surface to update does not match in size.");

        var context = _device.GetContext();

        var info = new MapInfo()
        {
            MapType = D3D11_MAP.D3D11_MAP_WRITE_DISCARD,
            SubResourceIndex = subIndex
        };

        var map = context.Map(this, info);

        // writting here.
        unsafe
        {
            var pDest = map.Map.pData;
            var pSource = surface.GetNativePointer();

            for(var i = 0; i < surface.ToArea() * surface.GetPeiceSize(); i++)
            {
                *((byte*)pDest + i) = surface.Get(i);
            }

        }

        context.Unmap(this, info);
    }

    private protected void Read(Surface surface, int subIndex)
    {
        Debug.Assert(ResourceInfo.UsageInfo.Usage == D3D11_USAGE.D3D11_USAGE_DYNAMIC ||
                     ResourceInfo.UsageInfo.Usage == D3D11_USAGE.D3D11_USAGE_STAGING,
            "Updating buffers from cpu is only available through Dynamic and staging buffers.");
        Debug.Assert(
            ResourceInfo.UsageInfo.CPUAccessFlags.HasFlag(D3D11_CPU_ACCESS_FLAG.D3D11_CPU_ACCESS_READ),
            "Does not have CPU Read access on this buffer.");
        Debug.Assert(surface.Size.ToArea() == ResourceInfo.Size.ToArea(),
            "Buffer and sent surface to update does not match in size.");

        var context = _device.GetContext();

        var info = new MapInfo()
        {
            MapType = D3D11_MAP.D3D11_MAP_READ,
            SubResourceIndex = subIndex
        };

        var map = context.Map(this, info);

        // writting here.
        unsafe
        {
            var pSource = map.Map.pData;
            var pDst = surface.GetNativePointer();

            for (var i = 0; i < surface.ToArea() * surface.GetPeiceSize(); i++)
            {
                 surface.Set(*((byte*)pSource + i), i);
            }
        }

        context.Unmap(this, info);
    }

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

    internal Resource(ComPtr<ID3D11Resource> pResource,
        ResourceInfo info,
        Device device)
    {
        ResourceInfo = info;
        _pResource = new(pResource);
        _device = device;
    }
}
