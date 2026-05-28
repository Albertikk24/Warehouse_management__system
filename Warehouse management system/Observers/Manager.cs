using WarehouseSystem.Infrastructure;
using WarehouseSystem.Observer;

namespace WarehouseSystem.Observers;

// Конкретный наблюдатель: Менеджер (получает email-уведомления)
public class Manager : IWarehouseObserver {
  // ========== ПОЛЯ ==========
  private string _employeeName;
  private string _emailAddress;

  // ========== КОНСТРУКТОР ==========
  public Manager(string employeeName, string emailAddress) {
    _employeeName = employeeName ?? throw new ArgumentNullException(nameof(employeeName));
    _emailAddress = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
  }

  // ========== ПОЛУЧЕНИЕ УВЕДОМЛЕНИЯ ==========
  public void Update(string message, string productSku, string productName, int currentQuantity, int thresholdValue) {
    // Формирование одного вывода в виде таблицы
    string notification = $@"
+---------------------------------------------------+
| EMAIL УВЕДОМЛЕНИЕ - МЕНЕДЖЕР                      |
+---------------------------------------------------+
| Кому: {_employeeName} <{_emailAddress}>
| Тема: {message}
+---------------------------------------------------+
| Товар: {productName} ({productSku})
| Текущий остаток: {currentQuantity} шт.
| Пороговое значение: {thresholdValue} шт.
|
| {(currentQuantity <= thresholdValue ? "ТРЕБУЕТСЯ ВМЕШАТЕЛЬСТВО!" : "Ситуация в норме")}
+---------------------------------------------------+";
    Console.WriteLine(notification);
    Logger.Instance.LogMessage($"Уведомление отправлено менеджеру {_employeeName}: {productName} - {currentQuantity} шт.");
  }

  // ========== ПОЛУЧЕНИЕ ДАННЫХ ==========
  public string GetObserverName() {
    return _employeeName;
  }

  public string GetObserverRole() {
    return "Менеджер";
  }
}