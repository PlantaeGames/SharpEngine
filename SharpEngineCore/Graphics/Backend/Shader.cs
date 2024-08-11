namespace SharpEngineCore.Graphics;

internal abstract class Shader
{
    private readonly Blob _blob;
    private readonly Device _device;
    internal Blob GetBlob() => _blob;

    protected Shader(Blob blob, Device device)
    {
        _blob = blob;
        _device = device;
    }
}