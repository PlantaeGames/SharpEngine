namespace SharpEngineCore.Graphics;

public interface IUnitable
{
    public int GetUnitCount();
    public Unit[] ToUnits();
    public int GetSize();

    /// <summary>
    /// Size of single unit of this structure in bytes.
    /// </summary>
    /// <returns>Size.</returns>
    public int GetUnitSize()
    {
        return Unit.GetSize();
    }
}