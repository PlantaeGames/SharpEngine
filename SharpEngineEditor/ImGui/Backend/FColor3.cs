using System.Runtime.InteropServices;

namespace SharpEngineEditor.ImGui.Backend;

[StructLayout(LayoutKind.Sequential, Pack = 0, Size = 12)]
public struct FColor3 : IFragmentable
{
    public Fragment r = 0f;
    public Fragment g = 0f;
    public Fragment b = 0f;

    public FColor3(Fragment r, Fragment g, Fragment b)
    {
        this.r = r;
        this.g = g;
        this.b = b;
    }

    public FColor3()
    { }

    /// <summary>
    /// Size of this structure in bytes.
    /// </summary>
    /// <returns>Size.</returns>
    public int GetSize()
    {
        unsafe
        {
            return sizeof(FColor3);
        }
    }

    public Fragment[] ToFragments()
    {
        return [r, g, b];
    }

    public int GetFragmentsCount()
    {
        return ToFragments().Length;
    }
}