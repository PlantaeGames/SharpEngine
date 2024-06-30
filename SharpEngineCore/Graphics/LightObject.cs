namespace SharpEngineCore.Graphics;

public sealed class LightObject : Object
{
    public LightConstantData Data { get; private set; }

    public void Update(LightConstantData data)
    {
        Data = data;
    }

    protected override void OnPause() { }
    protected override void OnResume() { }
    protected override void OnRemove() { }

    internal LightObject(LightConstantData data) :
        base()
    {
        Data = data;
    }
}
