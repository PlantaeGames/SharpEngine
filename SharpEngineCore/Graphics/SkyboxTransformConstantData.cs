namespace SharpEngineCore.Graphics;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 32)]
public struct SkyboxTransformConstantData : IFragmentable, ISurfaceable
{
    public FColor4 Rotation;
    public FColor4 Attributes;

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
                Rotation.a,

                Attributes.r,
                Attributes.g,
                Attributes.b,
                Attributes.a
            ];
    }

    public Surface ToSurface()
    {
        var surface = new FSurface(new(GetFragmentsCount(), 1));
        surface.SetLinearFragments(ToFragments());
        return surface;
    }
}
