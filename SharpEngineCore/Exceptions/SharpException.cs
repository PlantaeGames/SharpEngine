using System.Text;

namespace SharpEngineCore.Exceptions;

/// <summary>
/// A base for all exceptions of sharp engine.
/// </summary>
public class SharpException : Exception
{
    private const string ERROR_LABEL = "Error";

    public SharpException() : base() { }

    public SharpException(string message) :
        base(message)
    { }

    public SharpException(string message, Exception inner) :
        base(message, inner)
    { }

    public virtual void Show()
    {
       //TODO: Add Windowing System MessageBox here. or native.
       // MessageBox.Show($"{this}", ERROR_LABEL, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        foreach (var exception in this.GetExceptions())
        {
            result.AppendLine(exception.Message);
        }

        return result.ToString();
    }
}