using TerraFX.Interop.Windows;

namespace SharpEngineCore.Input;

public abstract class InputDevice
{
    public HANDLE Handle { get; init; }
    public DeviceType Type { get; init; }

    public abstract void Feed(Key key);

    protected InputDevice(HANDLE handle, DeviceType type)
    { 
        Handle = handle;
        Type = type;
    }
}
