using WarehouseSystem.Infrastructure;
using WarehouseSystem.Observer;

namespace WarehouseSystem.Observers;

// Конкретный наблюдатель: Кладовщик
public class Storekeeper : IWarehouseObserver
{
  private string _employeeName;
  private string _phoneNumber;

  public Storekeeper(string employeeName, string phoneNumber)
  {
    _employeeName = employeeName;
    _phoneNumber = phoneNumber;
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

  public string GetObserverName()
  {
    return _employeeName;
  }

  public string GetObserverRole()
  {
    return "Кладовщик";
  }
}