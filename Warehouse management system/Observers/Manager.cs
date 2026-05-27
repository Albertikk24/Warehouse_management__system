using WarehouseSystem.Infrastructure;
using WarehouseSystem.Observer;

namespace WarehouseSystem.Observers;

// Конкретный наблюдатель: Менеджер
public class Manager : IWarehouseObserver
{
  private string _employeeName;
  private string _emailAddress;

  public Manager(string employeeName, string emailAddress)
  {
    _employeeName = employeeName;
    _emailAddress = emailAddress;
  }

  public void Update(
    string message,
    string productSku,
    string productName,
    int currentQuantity,
    int thresholdValue)
  {
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

  public string GetObserverName()
  {
    return _employeeName;
  }

  public string GetObserverRole()
  {
    return "Менеджер";
  }
}