using System.Runtime.InteropServices;

namespace SharpEngineEditor.ImGui.Backend;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 8)]
public struct FColor2 : IFragmentable
{
    public Fragment r = 0f;
    public Fragment g = 0f;

    public FColor2(Fragment r, Fragment g)
    {
        this.r = r;
        this.g = g;
    }
    
    public FColor2()
    { }

    /// <summary>
    /// Size of this structure in bytes.
    /// </summary>
    /// <returns>Size.</returns>
    public int GetSize()
    {
        unsafe
        {
            return sizeof(FColor2);
        }
    }

    public Fragment[] ToFragments()
    {
        return [r, g];
    }

    public int GetFragmentsCount()
    {
        return ToFragments().Length;
    }
}