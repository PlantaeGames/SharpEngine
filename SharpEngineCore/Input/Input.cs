using System.Diagnostics;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Input;

public static class Input
{
    private static List<InputManager> _managers = new();
    private static bool _initialized;

    internal static void Add(InputManager manager)
    {
        _managers.Add(manager);
    }

    public static Point GetCursorPosition()
    {
        throw new NotImplementedException();
    }

    public static bool GetKeyDown(Key key)
    {
        var result = _managers
            .Any(x => 
            x.Any(d => d.GetKeyDown(key)));

        return result;
    }

    public static bool GetKeyUp(Key key)
    {
        var result = _managers
            .Any(x =>
            x.Any(d => d.GetKeyUp(key)));

        return result;
    }

    public static bool GetKey(Key key)
    {
        var result = _managers
            .Any(x =>
            x.Any(d => d.GetKey(key)));

        return result;
    }

    internal static void Feed(MSG msg)
    {
        Debug.Assert(_initialized);

        foreach (var manager in _managers)
        {
            manager.Feed(msg);
        }
    }
}
