namespace SharpEngineCore.Graphics;

public sealed class LightObject : Object
{
    public LightData Data { get; private set; }

    public void Update(LightData data)
    {
        Data = data;
    }

    protected override void OnPause() { }
    protected override void OnResume() { }
    protected override void OnRemove() { }

    internal LightObject(LightData data) :
        base()
    {
        Data = data;
    }
}
