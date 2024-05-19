namespace SharpEngineCore.Graphics;

public sealed class LightObject : Object
{
    internal LightData _lastUpdatedData;

    public void Update(LightData data)
    {
        _lastUpdatedData = data;
    }

    protected override void OnPause() { }
    protected override void OnResume() { }
    protected override void OnRemove() { }

    internal LightObject(LightData data) :
        base()
    {
        _lastUpdatedData = data;
    }
}
