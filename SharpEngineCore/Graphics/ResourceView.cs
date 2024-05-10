namespace SharpEngineCore.Graphics;

internal abstract class ResourceView
{
    public readonly ResourceViewInfo Info;
    private readonly Device _device;

    protected ResourceView(ResourceViewInfo info, Device device)
    { 
        Info = info;
        _device = device;
    }
}
