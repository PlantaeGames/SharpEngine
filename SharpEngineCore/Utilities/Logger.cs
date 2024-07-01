namespace SharpEngineCore.Utilities;

internal sealed class Logger
{
    private const ConsoleColor MESSAGE_COLOR = ConsoleColor.Green;
    private const ConsoleColor ERROR_COLOR = ConsoleColor.Red;
    private const ConsoleColor HEADER_COLOR = ConsoleColor.Cyan;

    public Logger() { }

    public void LogMessage(string message, bool pause = false)
    {
        Log($"LOG: {message}", MESSAGE_COLOR, pause);
    }

    public void LogError(string message)
    {
        Log($"ERROR: {message}", ERROR_COLOR, false);
    }

    public void BreakLine()
    {
        Log($"", MESSAGE_COLOR, false);
    }

    public void LogHeader(string message, bool pause = false)
    {
        Log($"HEADER: {message}", HEADER_COLOR, pause);
    }

    private void Log(string message, ConsoleColor color, bool pause)
    {
        return;

        var (initX, initY) = (Console.CursorLeft, Console.CursorTop);
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = color;

        Console.WriteLine(message);

        Console.ForegroundColor = previousColor;

        if(pause)
        {
            Console.CursorLeft = initX;
            Console.CursorTop = initY;

            Console.WriteLine(new char[message.Length]);

            Console.CursorLeft = initX;
            Console.CursorTop = initY;
        }
    }
}