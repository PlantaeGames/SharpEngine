namespace SharpEngineCore.Utilities;

internal sealed class Logger
{
    private const ConsoleColor MESSAGE_COLOR = ConsoleColor.Green;
    private const ConsoleColor ERROR_COLOR = ConsoleColor.Red;
    private const ConsoleColor HEADER_COLOR = ConsoleColor.Cyan;

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

    public void BreakLine()
    {
        Log($"", MESSAGE_COLOR);
    }

    public void LogHeader(string message)
    {
        Log($"ERROR: {message}", HEADER_COLOR);
    }

    private void Log(string message, ConsoleColor color)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = color;

        Console.WriteLine(message);

        Console.ForegroundColor = previousColor;
    }
}