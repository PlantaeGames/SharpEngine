﻿using System.Runtime.CompilerServices;
using System.Text;

using TerraFX.Interop.Windows;
using Win32 = TerraFX.Interop.Windows.Windows;

namespace SharpEngineCore.Exceptions;

/// <summary>
/// A base for all exceptions of sharp engine.
/// </summary>
public class SharpException : Exception
{
    private const string ERROR_LABEL = "Error";

    public SharpException(
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string fileName = "") : base($"{lineNumber}\nMemberName: {memberName}\nFileName: {fileName}")
    { }

    public SharpException(string message,
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string fileName = "") :
        base($"{message}\n\nLineNumber: {lineNumber}\nMemberName: {memberName}\nFileName: {fileName}")
    { }

    public SharpException(string message, Exception inner,      
        [CallerLineNumber] int lineNumber = 0,
        [CallerMemberName] string memberName = "",
        [CallerFilePath] string fileName = "") :
        base($"{message}\n\nLineNumber: {lineNumber}\nMemberName: {memberName}\nFileName: {fileName}", inner)
    { }

    public virtual void Show()
    {
       MessageBox.Show($"{this}", ERROR_LABEL, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public static SharpException GetLastWin32Exception(SharpException e)
    {
        var errorMessage = NativeGetError();
        
        var exception = new SharpException(errorMessage, e);

        unsafe string NativeGetError()
        {
            var result = string.Empty;

            var errorCode = Win32.GetLastError();

            const int bufferSize = 512;
            char* buffer = stackalloc char[bufferSize];

            if (Win32.FormatMessageW(FORMAT.FORMAT_MESSAGE_FROM_SYSTEM,
                null, errorCode, 0u, buffer, (uint)bufferSize, null) == 0u)
            {
                throw new SharpException("Failed to format error code.");
            }

            Span<char[]> message = new(buffer, bufferSize);
            var sb = new StringBuilder(bufferSize);
            foreach (var c in message)
            {
                sb.Append(c);
            }

            result = message.ToString();

            return result;
        }

        return exception;
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