namespace SharpEngineCore.Graphics;

public abstract class Object
{
    public State State { get; private set; }
    public readonly Guid Id = Guid.NewGuid();

    protected abstract void OnPause();
    protected abstract void OnResume();
    protected abstract void OnRemove();

    public void Pause()
    {
        OnPause();
        State = State.Paused;
    }

    public void Resume()
    {
        OnResume();
        State = State.Active;
    }

    public void Remove()
    {
        OnRemove();
        State = State.Expired;
    }
}
