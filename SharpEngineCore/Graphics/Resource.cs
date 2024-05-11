using SharpEngineCore.Utilities;
using System.Diagnostics;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

public sealed class PerspectivePoint
{
    public TransformConstantData Transform;

    public PerspectivePoint()
    { }
}

public sealed class GraphicsObject
{
    public TransformConstantData Transform { get; }
    public Mesh Mesh;

    public GraphicsObject()
    {

    }
}


internal abstract class Resource
{
    public readonly ResourceInfo ResourceInfo;

    private readonly ComPtr<ID3D11Resource> _pResource;
    protected readonly Device _device;

    private protected void Write(Surface surface, int subIndex)
    {
        Debug.Assert(ResourceInfo.UsageInfo.Usage == D3D11_USAGE.D3D11_USAGE_DYNAMIC,
            "Updating buffers from cpu is only available through Dynamic buffers.");
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
                *((byte*)pDest) = surface.Get(i);
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

    protected Resource(ComPtr<ID3D11Resource> pResource,
        ResourceInfo info,
        Device device)
    {
        ResourceInfo = info;
        _pResource = new(pResource);
        _device = device;
    }
}
