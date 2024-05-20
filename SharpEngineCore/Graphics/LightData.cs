using System.Runtime.InteropServices;

namespace SharpEngineCore.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 64)]
public struct LightData : IFragmentable, ISurfaceable
{
    public FColor4 Position;
    public FColor4 Rotation;
    public FColor4 Scale;
    public FColor4 Color;
    public FColor4 AmbientColor;
    public FColor4 Intensity;
    public FColor4 LightType;

    public FColor4 Attributes;

    public int GetFragmentsCount()
    {
        return ToFragments().Length;
    }

    public int GetSize()
    {
        unsafe
        {
            return sizeof(LightData);
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

                Color.r,
                Color.g,
                Color.b,
                Color.a,

                AmbientColor.r,
                AmbientColor.g,
                AmbientColor.b,
                AmbientColor.a,

                Intensity.r,
                Intensity.g,
                Intensity.b,
                Intensity.a,

                LightType.r,
                LightType.g,
                LightType.b,
                LightType.a,

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
