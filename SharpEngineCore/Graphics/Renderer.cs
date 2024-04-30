namespace SharpEngineCore.Graphics;

internal abstract class Renderer
{
    protected Device _device;

    protected Renderer(Device device)
    {
        _device = device;
    }
}
