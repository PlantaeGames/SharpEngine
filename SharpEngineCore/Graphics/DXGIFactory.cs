using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;

using SharpEngineCore.Exceptions;

namespace SharpEngineCore.Graphics;

internal sealed class DXGIFactory
{
    private ComPtr<IDXGIFactory> _pFactory;

    private static DXGIFactory? _instance;
    private static object _instanceLock = new();

    private const uint MAX_ENUMERATE_DEVICE_COUNT = 12u;

    public Adapter[] GetAdpters()
    {
        var pAdapters = NativeEnumerate();

        var adapters = new Adapter[pAdapters.Length];
        for (var i = 0; i < adapters.Length; i++)
        {
            adapters[i] = new Adapter(pAdapters[i]);
        }

        return adapters;

        unsafe ComPtr<IDXGIAdapter>[] NativeEnumerate()
        {
            var foundList = new List<uint>((int)MAX_ENUMERATE_DEVICE_COUNT);

            fixed (IDXGIFactory** ppFactory = _pFactory)
            {
                for (var i = 0u; i < MAX_ENUMERATE_DEVICE_COUNT; i++)
                {
                    IDXGIAdapter* pTemp = (IDXGIAdapter*)IntPtr.Zero;
                    if((*ppFactory)->EnumAdapters(i, &pTemp).SUCCEEDED)
                        foundList.Add(i);
                }

                var pAdapters = new ComPtr<IDXGIAdapter>[foundList.Count];
                for (var i = 0; i < pAdapters.Length; i++)
                {
                    pAdapters[i] = new ComPtr<IDXGIAdapter>();
                }
                for (var i = 0; i <  foundList.Count; i++)
                {
                    fixed (IDXGIAdapter** ppAdapter = pAdapters[i])
                    {
                        GraphicsSharpException.SetInfoQueue();
                        var result = (*ppFactory)->EnumAdapters(foundList[i], ppAdapter);

                        if (result.FAILED)
                        {
                            // error here.
                            throw GraphicsSharpException.GetLastGraphicsException
                                (new GraphicsSharpException($"Failed to get adapter.\nError Code: {result}"));
                        }
                    }
                }

                return pAdapters;
            }
        }
    }

    public static DXGIFactory GetInstance()
    {
        lock (_instanceLock)
        {
            _instance ??= new DXGIFactory();

            return _instance;
        }
    }

    private DXGIFactory()
    {
        _pFactory = new();
        Initialize();
    }

    private void Initialize()
    {
        NativeInitialize();

        unsafe void NativeInitialize()
        {
            fixed (IDXGIFactory** ppFactory = _pFactory)
            {
                var uuid = typeof(IDXGIFactory).GUID;

                GraphicsSharpException.SetInfoQueue();
                var result = DirectX.CreateDXGIFactory(&uuid, (void**)ppFactory);

                if(result.FAILED)
                {
                    // error here.
                    throw GraphicsSharpException.GetLastGraphicsException(
                        new GraphicsSharpException($"Failed to create DXGI Factory.\nError Code: {result}"));
                }
            }
        }
    }
}