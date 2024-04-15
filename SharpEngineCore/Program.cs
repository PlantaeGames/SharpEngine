using SharpEngineCore.Components;
using SharpEngineCore.Exceptions;

namespace SharpEngineCore;
internal sealed class Program
{
    /// <summary>
    /// Main start point of the application.
    /// </summary>
    [STAThread]
    public static void Main()
    {
        // handles UI exceptions
        Application.ThreadException += OnThreadException;
        // forcing ui exception handling in favor of our handler
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        // handles non UI exceptions
        AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;

        var app = new App();
        var context = app.Context;

        Application.Run(context);
    }

    /// <summary>
    /// Non UI Exception Handler.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    /// <exception cref="NotImplementedException"></exception>
    private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        var exception = e.ExceptionObject as Exception;
        HandleException(exception);
    }

    /// <summary>
    /// UI Exception Handler.
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Arguments</param>
    private static void OnThreadException(object sender, ThreadExceptionEventArgs e)
    {
        HandleException(e.Exception);
    }

    /// <summary>
    /// The exception handler of the program.
    /// </summary>
    /// <param name="exception">The unhandled exception.</param>
    private static void HandleException(Exception? exception)
    {
        if (exception == null)
            goto ABORT;

        if (exception is SharpException sharpException)
        {
            sharpException.Show();
        }
        else
        {
            new SharpException(exception.Message).Show();
        }

    ABORT:
        Abort();
    }

    /// <summary>
    /// Exits the application.
    /// </summary>
    private static void Abort()
    {
        Application.Exit();
    }
}