using System.Runtime.InteropServices;

namespace SharpEngineCore.Graphics;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 16)]
internal struct PixelShaderPassSwitchData : IFragmentable, ISurfaceable
{
    public FColor4 LightingSwitch;

    public int GetFragmentsCount()
    {
        return ToFragments().Length;
    }

    public int GetSize()
    {
        unsafe
        {
            return sizeof(PixelShaderPassSwitchData);
        }
    }

    public Fragment[] ToFragments()
    {
        return
            [
                LightingSwitch.r,
                LightingSwitch.g,
                LightingSwitch.b,
                LightingSwitch.a
            ];
    }

    public Surface ToSurface()
    {
        var surface = new FSurface(new(GetFragmentsCount(), 1));
        surface.SetLinearFragments(ToFragments());
        return surface;
    }
}
