namespace SharpEngineCore.Graphics;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 16)]
public struct SkyboxTransformConstantData : IFragmentable, ISurfaceable
{
    public FColor4 Rotation;

    public int GetFragmentsCount()
    {
        return ToFragments().Length;
    }

    public int GetSize()
    {
        unsafe
        {
            return sizeof(SkyboxTransformConstantData);
        }
    }

    public Fragment[] ToFragments()
    {
        return
            [
                Rotation.r,
                Rotation.g,
                Rotation.b,
                Rotation.a
            ];
    }

    public Surface ToSurface()
    {
        var surface = new FSurface(new(GetFragmentsCount(), 1));
        surface.SetLinearFragments(ToFragments());
        return surface;
    }
}
