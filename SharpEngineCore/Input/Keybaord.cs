﻿using SharpEngineCore.Graphics;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Input;

public sealed class Keyboard : InputDevice
{
    public Keyboard(Window window)
        : base(window, DeviceType.Keyboard)
    { }

    public override void Feed(MSG msg)
    {
        var key = (Key)((int)msg.wParam);

        if (msg.message == WM.WM_KEYDOWN)
        {
            UpdateKey(key, false);
        }
        else if (msg.message == WM.WM_KEYUP)
        {
            UpdateKey(key, true);
        }
    }
}
