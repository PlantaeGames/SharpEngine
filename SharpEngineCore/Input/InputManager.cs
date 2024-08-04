using System.Collections;
using System.Diagnostics;
using TerraFX.Interop.Windows;

namespace SharpEngineCore.Input;

internal sealed class InputManager : IEnumerable<InputDevice>
{
    private const int MAX_DEVICE_COUNT = 32;

    public int DeviceCount => _devices.Count;

    private List<InputDevice> _devices = new();

    public T GetDevice<T>(int index = 0)
        where T : InputDevice
    {
        var device = _devices
                        .Where(x => x as T != null)
                        .ToArray()[index];

        return device as T;
    }

    public void RemoveDevice<T>(int index = 0)
        where T : InputDevice
    {
        var device = GetDevice<T>(index);
        _devices.Remove(device);
    }

    public T AddDevice<T>(T device)
        where T : InputDevice
    {
        Debug.Assert(DeviceCount <= MAX_DEVICE_COUNT,
            "Input device limit reached, can't add more devices.");

        _devices.Add(device);
        return device;
    }

    public void Feed(MSG msg)
    {
        foreach (var device in _devices)
        {
            device.Feed(msg);
        }
    }

    public void Feed(MSG msg, DeviceType type)
    {
        var devices = _devices.
                        Where(x => x.Type == type);

        foreach(var device in devices)
        {
            device.Feed(msg);
        }
    }

    public IEnumerator<InputDevice> GetEnumerator()
    {
        return new Enumerator(_devices.ToArray());
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator(_devices.ToArray());
    }

    public InputManager()
    { }

    internal sealed class Enumerator : IEnumerator<InputDevice>
    {
        private InputDevice[] _devices;
        private int _index = -1;

        public Enumerator(InputDevice[] devices)
        {
            Debug.Assert(devices != null);

            _devices = devices;
        }

        public InputDevice Current => _devices[_index];

        object IEnumerator.Current => _devices[_index];

        public void Dispose()
        {}

        public bool MoveNext()
        {
            _index++;

            return _index < _devices.Length;
        }

        public void Reset()
        {
            _index = -1;
        }
    }
}