using TerraFX.Interop.Windows;
using System.Diagnostics;
using SharpEngineCore.Graphics;


namespace SharpEngineCore.Input;

public abstract class InputDevice
{
    public Window Handle { get; init; }
    public DeviceType Type { get; init; }

    protected List<Key> _downKeys = new();
    protected List<Key> _upKeys = new();
    protected List<Key> _holdKeys = new();

    public abstract void Feed(MSG msg);

    protected virtual void UpdateKey(Key key, bool up)
    {
        if (up)
        {
            if (GetKey(key))
                return;

            _downKeys.Add(key);
            _holdKeys.Add(key);
        }
        else
        {
            if (GetKeyUp(key) && !GetKey(key))
                return;

            _upKeys.Add(key);
        }
    }

    public virtual KeyState GetState(Key key)
    {
        if(_upKeys.Contains(key))
            return KeyState.Up;
        else if (_downKeys.Contains(key))
            return KeyState.Down;

        if(_holdKeys.Contains(key))
            return KeyState.Hold;

        return KeyState.Up;
    }

    public virtual void Clear()
    {
        _downKeys.Clear();
        _upKeys.Clear();
    }

    public virtual bool GetKeyDown(Key key)
    {
        return _downKeys.Contains(key);
    }

    public virtual bool GetKey(Key key)
    {
        return _holdKeys.Contains(key);
    }

    public virtual bool GetKeyUp(Key key)
    {
        return _upKeys.Contains(key);
    }

    protected InputDevice(Window window, DeviceType type)
    {
        Handle = window;
        Type = type;
    }
}
