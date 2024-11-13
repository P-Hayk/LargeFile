namespace Common;
public class Logger
{
    public void LogInfo(string message)
    {
        Log("INFO", message);
    }

    public void LogWarning(string message)
    {
        Log("WARNING", message);
    }

    public void LogError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Log("ERROR", message);
        Console.ForegroundColor = ConsoleColor.White;

    }

    private void Log(string level, string message)
    {
        string logMessage = $"{DateTime.Now:G} [{level}] {message}";
        Console.WriteLine(logMessage);
    }
}