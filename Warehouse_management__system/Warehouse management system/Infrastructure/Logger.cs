namespace WarehouseSystem.Infrastructure;

// Паттерн Одиночка (Singleton): потокобезопасный логгер
public sealed class Logger {
  // ========== ОДИНОЧКА (SINGLETON) ==========
  private static readonly Lazy<Logger> _singletonInstance = new Lazy<Logger>(() => new Logger());
  
  // ========== ПОЛЯ ==========
  private List<string> _logEntries;
  private readonly object _threadLock = new object();
  
  // ========== КОНСТАНТЫ ==========
  private const int MAXIMUM_LOG_ENTRIES = 1000;

  // ========== ПРИВАТНЫЙ КОНСТРУКТОР ==========
  private Logger() {
    _logEntries = new List<string>();
    LogMessage("Логгер инициализирован");
  }

  // ========== СВОЙСТВО ДЛЯ ДОСТУПА К ЭКЗЕМПЛЯРУ ==========
  public static Logger Instance {
    get { return _singletonInstance.Value; }
  }

  // ========== ДОБАВЛЕНИЕ ЗАПИСИ В ЛОГ ==========
  public void LogMessage(string logText) {
    string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    string formattedLogEntry = $"[{timestamp}] {logText}";
    
    lock (_threadLock) {
      _logEntries.Add(formattedLogEntry);
      
      // Ограничение размера лога (кольцевой буфер)
      if (_logEntries.Count > MAXIMUM_LOG_ENTRIES) {
        _logEntries.RemoveAt(0);
      }
      
      // Вывод в консоль серым цветом
      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.WriteLine(formattedLogEntry);
      Console.ResetColor();
    }
  }

  // ========== ВЫВОД ВСЕХ ЛОГОВ НА ЭКРАН ==========
  public void DisplayAllLogs() {
    // Формирование одного вывода
    string output = "\n" + new string('=', 60) + "\n";
    output += "ИСТОРИЯ ЛОГОВ\n";
    output += new string('=', 60) + "\n";
    
    if (_logEntries.Count == 0) {
      output += "Логи отсутствуют\n";
    } else {
      for (int logIndex = 0; logIndex < _logEntries.Count; logIndex++) {
        output += _logEntries[logIndex] + "\n";
      }
    }
    
    output += new string('=', 60) + "\n";
    output += $"Всего записей: {_logEntries.Count}\n";
    
    Console.WriteLine(output);
  }

  // ========== ОЧИСТКА ЛОГОВ ==========
  public void ClearAllLogs() {
    lock (_threadLock) {
      _logEntries.Clear();
      LogMessage("Логи очищены");
    }
  }

  // ========== ПОЛУЧЕНИЕ КОПИИ ЛОГОВ ==========
  public List<string> GetCopyOfLogs() {
    lock (_threadLock) {
      return new List<string>(_logEntries);
    }
  }
}