using System.Runtime.InteropServices;

namespace SharpEngineCore.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 64)]
public struct TransformConstantData : IFragmentable, ISurfaceable
{
    public FColor4 Position;
    public FColor4 Rotation;
    public FColor4 Scale;
    public FColor4 W;

    public int GetFragmentsCount()
    {
        return ToFragments().Length;
    }

    public int GetSize()
    {
        unsafe
        {
            return sizeof(TransformConstantData);
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

                W.r,
                W.g,
                W.b,
                W.a
            ];
    }

    public Surface ToSurface()
    {
        var surface = new FSurface(new(GetFragmentsCount(), 1));
        surface.SetLinearFragments(ToFragments());
        return surface;
    }
}
