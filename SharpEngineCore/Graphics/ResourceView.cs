namespace SharpEngineCore.Graphics;

internal abstract class ResourceView
{
    public readonly ResourceViewInfo ResourceViewInfo;

    private readonly Resource _resource;
    private readonly Device _device;

    protected ResourceView(Resource resource,
        ResourceViewInfo resourceViewInfo,
        Device device)
    {
        _resource = resource;
        ResourceViewInfo = resourceViewInfo;

        _device = device;
    }
}