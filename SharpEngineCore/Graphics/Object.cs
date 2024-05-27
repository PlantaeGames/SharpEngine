using System.Diagnostics;
using TerraFX.Interop.WinRT;

namespace SharpEngineCore.Graphics;

public abstract class Object
{
    public State State { get; private set; }
    public readonly Guid Id = Guid.NewGuid();

    internal void SetState(State state)
    {
        switch (state)
        {
            case State.Active:
                State = State.Active;
                OnRemove();
                break;
            case State.Paused:
                State = State.Paused;
                OnPause();
                break;
            case State.Expired:
                State = State.Expired;
                OnRemove();
                break;
            default:
                Debug.Assert(false,
                    "Unknown State of Object");
                break;
        }
    }

    protected abstract void OnPause();
    protected abstract void OnResume();
    protected abstract void OnRemove();
}
