using System.Diagnostics;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Input;

public static class Input
{
    private static InputManager _manager;
    private static bool _initialized;

    private static Mouse _mouse;
    private static Keyboard _keyboard;

    public static Point GetCursorPosition()
    {
        return _mouse.CursorPosition;
    }

    public static bool GetKeyDown(Key key)
    {
        return (_mouse.GetKeyDown(key) ||
                _keyboard.GetKeyDown(key));
    }

    public static bool GetKeyUp(Key key)
    {
        return (_mouse.GetKeyUp(key) ||
                _keyboard.GetKeyUp(key));
    }

    public static bool GetKey(Key key)
    {
        return (_mouse.GetKey(key) ||
                _keyboard.GetKey(key));
    }

    internal static void Feed(MSG msg)
    {
        Debug.Assert(_initialized);

        _manager.Feed(msg);
    }

    internal static void Initialize(InputManager inputManager)
    {
        Debug.Assert(_initialized == false);

        _manager = inputManager;

        _mouse = _manager.GetDevice<Mouse>(0);
        _keyboard = _manager.GetDevice<Keyboard>(0);

        _initialized = true;
    }
}
