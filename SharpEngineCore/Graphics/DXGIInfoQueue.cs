using System.Text;

using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;

using SharpEngineCore.Exceptions;
using SharpEngineCore.Utilities;

namespace SharpEngineCore.Graphics;

internal sealed class DXGIInfoQueue
{
    private static DXGIInfoQueue? _instance;

    private ComPtr<IDXGIInfoQueue> _pInfoQueue;
    private ulong _messageCount = 0u;

    private static object _instanceLock = new();

    public static DXGIInfoQueue GetInstance()
    {
        lock (_instanceLock)
        {
            _instance ??= new DXGIInfoQueue();

            return _instance;
        }
    }
    public void Set()
    {
        _messageCount = GetStoredMessageCount();
    }

    public string[] GetMessages()
    {
        var result = NativeGetMessage();

        unsafe string[] NativeGetMessage()
        {
            var result = new List<string>(10);
            result.Add("[ DXGI INFO QUEUE MESSAGES ]");

            var messageCount = GetStoredMessageCount();

            fixed (IDXGIInfoQueue** ppInfoQueue = _pInfoQueue)
            {
                for (var i = _messageCount; i < messageCount; i++)
                {
                    HRESULT returnCode = 0;

                    nuint messageSize = 0u;
                    returnCode = (*ppInfoQueue)->GetMessage(DXGI.DXGI_DEBUG_ALL, i, 
                        (DXGI_INFO_QUEUE_MESSAGE*)IntPtr.Zero, &messageSize);

                    if(Errors.CheckHResult(returnCode) == false)
                    {
                        // error here
                        throw SharpException.GetLastWin32Exception(
                            new SharpException("Unable to retive DXGI Info Queue Message Length."));
                    }

                    DXGI_INFO_QUEUE_MESSAGE message = new DXGI_INFO_QUEUE_MESSAGE();

                    char[] buffer = new char[(int)messageSize];
                    fixed(char* pBuffer = buffer)
                    {
                        returnCode = (*ppInfoQueue)->GetMessage(DXGI.DXGI_DEBUG_ALL, i,
                            &message, &messageSize);

                        if(Errors.CheckHResult(returnCode) == false)
                        {
                            // error here
                            throw SharpException.GetLastWin32Exception(
                                new SharpException("Unable to retive DXGI Info Queue Message."));
                        }
                        for(var x = 0; x < (int)messageSize; x++)
                        {
                            buffer[x] = (char)*(message.pDescription + (x * sizeof(sbyte)));
                        }

                        result.Add(buffer.ToSingleString());
                    }
                }
            }

            return result.ToArray();
        }

        return result;
    }

    public bool IsMessageAvailable()
    {
        var result = NativeIsMessageAvailable();
        return result;

        unsafe bool NativeIsMessageAvailable()
        {
            var result = GetStoredMessageCount() - _messageCount > 0;
            return result;
        }
    }

    private ulong GetStoredMessageCount()
    {
        return NativeGetStoredMessageCount();

        unsafe ulong NativeGetStoredMessageCount()
        {
            ulong result = 0u;

            fixed(IDXGIInfoQueue** ppInfoQueue = _pInfoQueue)
            {
                result = (*ppInfoQueue)->GetNumStoredMessages(DXGI.DXGI_DEBUG_ALL);
            }

            return result;
        }
    }

    private DXGIInfoQueue()
    {
        Initialize();
    }

    private void Initialize()
    {
        NativeInitialize();

        unsafe void NativeInitialize()
        {
            const string libName = "dxgidebug.dll";
            fixed (char* pLibName = libName)
            {
                var handle = LoadLibraryExW(pLibName, (HANDLE)IntPtr.Zero, LOAD.LOAD_LIBRARY_SEARCH_SYSTEM32);
                if(handle == IntPtr.Zero)
                {
                    // error here
                    throw SharpException.GetLastWin32Exception(new SharpException($"Error in loading {libName}"));
                }

                var bytes = Encoding.UTF8.GetBytes("DXGIGetDebugInterface");
                var sbytes = new sbyte[bytes.Length];
                for (var i = 0; i < bytes.Length; i++)
                    sbytes[i] = (sbyte)bytes[i];

                fixed (sbyte* pFunctionName = sbytes)
                {
                    var fnPtr = (delegate* unmanaged<Guid, void**, void>)GetProcAddress(handle, pFunctionName);
                    if (fnPtr == null)
                    {
                        // error here
                        throw SharpException.GetLastWin32Exception(new SharpException($"Error in finding {bytes}"));
                    }

                    fixed(IDXGIInfoQueue** ppInfoQueue = _pInfoQueue)
                    {
                        fnPtr(typeof(IDXGIInfoQueue).GUID, (void**)ppInfoQueue);
                    }
                }
            }
        }
    }
}
