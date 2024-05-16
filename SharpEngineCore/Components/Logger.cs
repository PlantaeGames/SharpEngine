namespace SharpEngineCore.Components;

internal sealed class Logger
{
    private const ConsoleColor MESSAGE_COLOR = ConsoleColor.Cyan;
    private const ConsoleColor ERROR_COLOR = ConsoleColor.Red;

    public Logger()
    {

    }

    public void LogMessage(string message)
    {
       Log($"LOG: {message}", MESSAGE_COLOR);
    }

    public void LogError(string message)
    {
        Log($"ERROR: {message}", ERROR_COLOR);
    }

    private void Log(string message, ConsoleColor  color)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = color;

        Console.WriteLine(message);

        Console.ForegroundColor = previousColor;
    }
}