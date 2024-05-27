namespace SharpEngineCore.Graphics;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 80)]
public struct CameraConstantData : IFragmentable, ISurfaceable
{
    public FColor4 Position;
    public FColor4 Rotation;
    public FColor4 Scale;
    public FColor4 Projection;
    public FColor4 Attributes;

    public int GetFragmentsCount()
    {
        return ToFragments().Length;
    }

    public int GetSize()
    {
        unsafe
        {
            return sizeof(CameraConstantData);
        }
    }

    public Fragment[] ToFragments()
    {
        return
            [
                Position.r,
                Position.g,
                Position.b,
                Position.a,

                Rotation.r,
                Rotation.g,
                Rotation.b,
                Rotation.a,

                Scale.r,
                Scale.g,
                Scale.b,
                Scale.a,

                Projection.r,
                Projection.g,
                Projection.b,
                Projection.a,

                Attributes.r,
                Attributes.g,
                Attributes.b,
                Attributes.a,

            ];
    }

    public Surface ToSurface()
    {
        var surface = new FSurface(new(GetFragmentsCount(), 1));
        surface.SetLinearFragments(ToFragments());
        return surface;
    }
}
