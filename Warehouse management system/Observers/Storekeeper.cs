using WarehouseSystem.Infrastructure;
using WarehouseSystem.Observer;

namespace WarehouseSystem.Observers;

// Конкретный наблюдатель: Кладовщик (получает SMS-уведомления)
public class Storekeeper : IWarehouseObserver {
  // ========== ПОЛЯ ==========
  private string _employeeName;
  private string _phoneNumber;

  // ========== КОНСТРУКТОР ==========
  public Storekeeper(string employeeName, string phoneNumber) {
    _employeeName = employeeName ?? throw new ArgumentNullException(nameof(employeeName));
    _phoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
  }

  // ========== ПОЛУЧЕНИЕ УВЕДОМЛЕНИЯ ==========
  public void Update(string message, string productSku, string productName, int currentQuantity, int thresholdValue) {
    // Формирование одного вывода в виде таблицы
    string notification = $@"
+---------------------------------------------------+
| SMS УВЕДОМЛЕНИЕ - КЛАДОВЩИК                       |
+---------------------------------------------------+
| Кому: {_employeeName} <{_phoneNumber}>
|
| {message}
| 
| Товар: {productName} ({productSku})
| Остаток: {currentQuantity}/{thresholdValue} шт.
|
| {(currentQuantity <= thresholdValue ? "СРОЧНО ПОПОЛНИТЕ СКЛАД!" : "Остаток в норме")}
+---------------------------------------------------+";
    Console.WriteLine(notification);
    Logger.Instance.LogMessage($"Уведомление отправлено кладовщику {_employeeName}: {productName} - {currentQuantity} шт.");
  }

  // ========== ПОЛУЧЕНИЕ ДАННЫХ ==========
  public string GetObserverName() {
    return _employeeName;
  }

  public string GetObserverRole() {
    return "Кладовщик";
  }
}