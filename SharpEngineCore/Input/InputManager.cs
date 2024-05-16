using System.Diagnostics;

namespace SharpEngineCore.Input;

public sealed class InputManager
{
    private const int MAX_DEVICE_COUNT = 32;

    public int DeviceCount => _devices.Count;

    private List<InputDevice> _devices = new();

    public T GetDevice<T>(int index)
        where T : InputDevice, new()
    {
        var device = _devices
                        .Where(x => x as T != null)
                        .ToArray()[index];

        return device as T;
    }

    public void RemoveDevice<T>(int index)
        where T : InputDevice, new()
    {
        var device = GetDevice<T>(index);
        _devices.Remove(device);
    }

    public T AddDevice<T>(T device)
        where T : InputDevice, new()
    {
        Debug.Assert(DeviceCount <= MAX_DEVICE_COUNT,
            "Input device limit reached, can't add more devices.");

        _devices.Add(device);
        return device;
    }

    public void Feed(Key key, DeviceType type)
    {
        var devices = _devices.
                        Where(x => x.Type == type);

        foreach(var device in devices)
        {
            device.Feed(key);
        }
    }

    public InputManager()
    { }
}