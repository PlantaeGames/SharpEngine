using System.Text;

using TerraFX.Interop.Windows;
using Win32 = TerraFX.Interop.Windows.Windows;

using SharpEngineCore.Utilities;
using System.Diagnostics;

namespace SharpEngineCore.Exceptions;

/// <summary>
/// A base for all exceptions of sharp engine.
/// </summary>
public class SharpException : Exception
{
    private const string ERROR_LABEL = "Error";

    public SharpException() : 
        base($"[Stack Trace]\n{new StackTrace()}")
    { }

    public SharpException(string message) :
        base($"{message}\n\n[Stack Trace]\n{new StackTrace()}")
    { }

    public SharpException(string message, Exception inner) :
        base($"{message}\n\n[Stack Trace]\n{new StackTrace()}", inner)
    { }

    public virtual void Show()
    {
        NativeShow();

        unsafe void NativeShow()
        {
            var message = this.ToString();
            var caption = "Error";

            fixed(char* pMessage = message)
            {
                fixed(char* pCaption = caption)
                {
                    Win32.MessageBoxExW((HWND)IntPtr.Zero, pMessage, pCaption, MB.MB_OK, 0);
                }
            }
        }

        new Logger().LogError(this.ToString());
    }

    public static SharpException GetLastWin32Exception()
    {
        return GetLastWin32Exception("");
    }

    public static SharpException GetLastWin32Exception(string message)
    {
        var errorMessage = NativeGetError();

        return new SharpException($"{message}\n\n[Win32 Exception]\n{errorMessage}");

        unsafe string NativeGetError()
        {
            var result = string.Empty;

            var errorCode = Win32.GetLastError();

            const int bufferSize = 512;
            char* buffer = stackalloc char[bufferSize];

            if (Win32.FormatMessageW(FORMAT.FORMAT_MESSAGE_FROM_SYSTEM,
                null, errorCode, 0u, buffer, bufferSize, null) == 0u)
            {
                throw new SharpException("Failed to format error code.");
            }

            Span<char> message = new(buffer, bufferSize);
            var sb = new StringBuilder(bufferSize);
            foreach (var c in message)
            {
                sb.Append(c);
            }

            result = message.ToString();

            return result;
        }
    }

    public static void ThrowLastWin32Exception(string message)
    {
        throw GetLastWin32Exception(message);
    }

    public override string ToString()
    {
        var result = new StringBuilder();
        foreach (var exception in this.GetExceptions())
        {
            result.AppendLine($"{exception.Message}\n");
        }

        return result.ToString();
    }
}