namespace SharpEngineEditor.ImGui.Backend;

internal abstract class ResourceView
{
    public readonly ResourceViewInfo ResourceViewInfo;
    
    private readonly Resource _resource;
    private readonly Device _device;

    public bool IsValid => _resource.IsValid();

    protected ResourceView(Resource resource,
        ResourceViewInfo resourceViewInfo,
        Device device)
    {
        _resource = resource;
        ResourceViewInfo = resourceViewInfo;

        _device = device;
    }
}