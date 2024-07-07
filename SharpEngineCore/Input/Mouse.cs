using SharpEngineCore.Graphics;
using System.Reflection.Metadata;
using System.Windows.Forms;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Input;

public sealed class Mouse : InputDevice
{
    public Point CursorPosition { get; private set; } = new();

    private void UpdateCursorPosition(Point cursorPos)
    {
        // TODO: CONVERT SCREEN COORDS TO WINDOW COORDS.
    }

    public override void Feed(MSG msg)
    {
        if(msg.message == WM.WM_LBUTTONDOWN)
        {
            UpdateKey(Key.LButton, false);
        }
        else if(msg.message == WM.WM_LBUTTONUP)
        {
            UpdateKey(Key.LButton, true);
        }
        else if(msg.message == WM.WM_RBUTTONDOWN)
        {
            UpdateKey(Key.RButton, false);
        }
        else if(msg.message == WM.WM_RBUTTONUP)
        {
            UpdateKey(Key.RButton, true);
        }
        else if(msg.message == WM.WM_MBUTTONDOWN)
        {
            UpdateKey(Key.MButton, false);
        }
        else if(msg.message == WM.WM_MOUSEMOVE)
        {
            UpdateCursorPosition(new(msg.pt.x, msg.pt.y));
        }
    }

    public Mouse(Window window)
        : base(window, DeviceType.Mouse)
    { }
}