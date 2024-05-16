using TerraFX.Interop.Windows;

namespace SharpEngineCore.Input;

public sealed class Mouse : InputDevice
{
    public override void Feed(Key key)
    {
        
    }

    public Mouse(HANDLE handle)
        : base(handle, DeviceType.Mouse)
    { }
}
