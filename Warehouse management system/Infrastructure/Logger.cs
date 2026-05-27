namespace WarehouseSystem.Infrastructure;

// Паттерн Одиночка (Singleton): потокобезопасный логгер
public sealed class Logger
{
  private static readonly Lazy<Logger> _singletonInstance = new Lazy<Logger>(() => new Logger());

  private List<string> _logEntries;
  private readonly object _threadLock = new object();

  private const int MAXIMUM_LOG_ENTRIES = 1000;

  private Logger()
  {
    _logEntries = new List<string>();
    LogMessage("Логгер инициализирован");
  }

  public static Logger Instance
  {
    get { return _singletonInstance.Value; }
  }

  public void LogMessage(string logText)
  {
    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    string formattedLogEntry = $"[{timestamp}] {logText}";

    lock (_threadLock)
    {
      _logEntries.Add(formattedLogEntry);

      if (_logEntries.Count > MAXIMUM_LOG_ENTRIES)
      {
        _logEntries.RemoveAt(0);
      }

      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine(formattedLogEntry);
      Console.ResetColor();
    }
  }

  public void DisplayAllLogs()
  {
    Console.WriteLine("\n" + new string('=', 60));
    Console.WriteLine("ИСТОРИЯ ЛОГОВ");
    Console.WriteLine(new string('=', 60));

    if (_logEntries.Count == 0)
    {
      Console.WriteLine("Логи отсутствуют");
      Console.WriteLine(new string('=', 60));
      Console.WriteLine();
      return;
    }

    for (int logIndex = 0; logIndex < _logEntries.Count; logIndex++)
    {
      Console.WriteLine(_logEntries[logIndex]);
    }

    Console.WriteLine(new string('=', 60));
    Console.WriteLine($"Всего записей: {_logEntries.Count}");
    Console.WriteLine();
  }

  public void ClearAllLogs()
  {
    lock (_threadLock)
    {
      _logEntries.Clear();
      LogMessage("Логи очищены");
    }
  }

  public List<string> GetCopyOfLogs()
  {
    lock (_threadLock)
    {
      return new List<string>(_logEntries);
    }
  }
}