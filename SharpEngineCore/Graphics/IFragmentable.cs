using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

public interface IFragmentable
{
    public Fragment[] ToFragments();
    public int GetFragmentsCount();

    public int GetSize();

    /// <summary>
    /// Gets the size of the Fragment in bytes.
    /// </summary>
    /// <returns>Size of fragment.</returns>
    public int GetUnitSize()
    {
        return Fragment.GetSize();
    }
}
