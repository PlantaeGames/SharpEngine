using TerraFX.Interop.Windows;

namespace SharpEngineCore.Graphics;

public abstract class Shader
{
    private readonly Blob _blob;
    internal Blob GetBlob() => _blob;

    protected Shader(Blob blob)
    {
        _blob = blob;
    }
}