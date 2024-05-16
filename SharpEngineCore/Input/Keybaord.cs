using TerraFX.Interop.Windows;

namespace SharpEngineCore.Input;

public sealed class Keybaord : InputDevice
{
    public override void Feed(Key key)
    {

    }

    public Keybaord(HANDLE handle)
        : base(handle, DeviceType.Keyboard)
    { }
}
