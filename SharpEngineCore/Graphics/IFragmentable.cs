namespace SharpEngineCore.Graphics;

public interface IFragmentable
{
    public Fragment[] ToFragments();
    public int GetFragmentsCount();
}
