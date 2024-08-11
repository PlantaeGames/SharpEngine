using System.Text;
using System.Runtime.InteropServices;

using TerraFX.Interop.Windows;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.Windows.Windows;

using SharpEngineCore.Exceptions;
using SharpEngineCore.Utilities;

namespace SharpEngineEditor.ImGui.Backend;

public class GraphicsException : SharpException
{
#if DEBUG
    /// <summary>
    /// Wraps the COM IDXGIInfoQueue.
    /// Provides methods to obtains messages from DXGIQueue.
    /// </summary>
    private sealed class DXGIInfoQueue
    {
        private ComPtr<IDXGIInfoQueue> _pInfoQueue;
        private ulong _messageCount = 0u;

        private HMODULE _debugLibHandle;
        public void Set()
        {
            _messageCount = GetStoredMessageCount();
        }

        /// <summary>
        /// Obtains any new messages sent to info queue.
        /// </summary>
        /// <returns>Messages.</returns>
        public string[] GetMessages()
        {
            var result = NativeGetMessage();
            return result;

            unsafe string[] NativeGetMessage()
            {
                var messages = new List<string>(10);

                messages.Add("[ DXGI INFO QUEUE MESSAGES ]\n");
                if (IsMessageAvailable() == false)
                    messages.Add("[ No Messages ]\n");


                var messageCount = GetStoredMessageCount();
                var uuid = DXGI.DXGI_DEBUG_ALL;

                fixed (IDXGIInfoQueue** ppInfoQueue = _pInfoQueue)
                {
                    for (var i = _messageCount; i < messageCount; i++)
                    {
                        HRESULT result = 0;

                        // getting message length
                        nuint messageSize = 0u;
                        result = (*ppInfoQueue)->GetMessage(uuid, i,
                            (DXGI_INFO_QUEUE_MESSAGE*)IntPtr.Zero, &messageSize);

                        if (result.FAILED)
                        {
                            // error here
                            SharpException.ThrowLastWin32Exception(
                                "Unable to retive DXGI Info Queue Message Length.");
                        }

                        // getting message
                        nint pMessage = 0;
                        try
                        {
                            pMessage = Marshal.AllocHGlobal((int)messageSize);
                        }
                        catch (Exception e)
                        {
                            throw new SharpException("Failed to obtain message from DXGI Info Queue", e);
                        }

                        for (var x = 0; x < (int)messageSize; x++)
                        {
                            *((byte*)pMessage + (x * sizeof(byte))) = 0;
                        }

                        result = (*ppInfoQueue)->GetMessage(uuid, i,
                                    (DXGI_INFO_QUEUE_MESSAGE*)pMessage, &messageSize);

                        if (result.FAILED)
                        {
                            Marshal.FreeHGlobal(pMessage);

                            // error here
                            SharpException.ThrowLastWin32Exception(
                                "Unable to retive DXGI Info Queue Message.");
                        }

                        // coping c string
                        var sb = new StringBuilder();

                        for (var x = 0; x < (int)messageSize; x++)
                        {
                            byte target = (byte)*(((DXGI_INFO_QUEUE_MESSAGE*)pMessage)->pDescription
                                + (x * sizeof(sbyte)));

                            if (target == 0)
                                break;

                            sb.Append(Convert.ToChar(target));
                        }

                        Marshal.FreeHGlobal(pMessage);

                        messages.Add($"{sb}\n");
                    }
                }

                return messages.ToArray();
            }

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

                fixed (IDXGIInfoQueue** ppInfoQueue = _pInfoQueue)
                {
                    result = (*ppInfoQueue)->GetNumStoredMessages(DXGI.DXGI_DEBUG_ALL);
                }

                return result;
            }
        }

        public DXGIInfoQueue()
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
                    if (handle == IntPtr.Zero)
                    {
                        // error here
                        SharpException.ThrowLastWin32Exception($"Error in loading {libName}");
                    }

                    _debugLibHandle = handle;

                    var bytes = Encoding.ASCII.GetBytes("DXGIGetDebugInterface");
                    var sbytes = new sbyte[bytes.Length];
                    for (var i = 0; i < bytes.Length; i++)
                        sbytes[i] = (sbyte)bytes[i];

                    fixed (sbyte* pFunctionName = sbytes)
                    {
                        var fnPtr = (delegate* unmanaged<Guid, void**, void>)
                            GetProcAddress(handle, pFunctionName);
                        if (fnPtr == null)
                        {
                            // error here
                            SharpException.ThrowLastWin32Exception($"Error in finding {bytes}");
                        }

                        fixed (IDXGIInfoQueue** ppInfoQueue = _pInfoQueue)
                        {
                            var uuid = typeof(IDXGIInfoQueue).GUID;
                            fnPtr(uuid, (void**)ppInfoQueue);
                        }
                    }
                }
            }
        }

        ~DXGIInfoQueue()
        {
            NativeFree();

            void NativeFree()
            {
                if (_debugLibHandle != 0)
                    FreeLibrary(_debugLibHandle);
            }
        }
    }

    private static DXGIInfoQueue _InfoQueue = new();

#endif

#nullable enable
    public static bool CheckIfAny()
    {
#if DEBUG
        return _InfoQueue.IsMessageAvailable();
#else
        return false;
#endif
    }
#nullable disable

    public static void SetInfoQueue()
    {
#if DEBUG
        _InfoQueue.Set();
#endif
    }

    public static void ThrowLastGraphicsException(string message)
    {
#if DEBUG
        var infoQueueMsg = _InfoQueue.GetMessages().ToSingleString();
#else
        var infoQueueMsg = string.Empty;
#endif

        throw new GraphicsException(
                    $"{message}\n" +
                    $"\n{infoQueueMsg}\n");
    }

    public GraphicsException() 
        : base()
    {
    }

    public GraphicsException(string message) 
        : base(message)
    {
    }

    public GraphicsException(string message, Exception inner) :
        base(message, inner)
    {
    }
}