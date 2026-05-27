namespace WarehouseSystem.Observer;

// Паттерн Наблюдатель: интерфейс наблюдателя
public interface IWarehouseObserver
{
  // Вызывается при изменении состояния склада
  void Update(
    string message,
    string productSku,
    string productName,
    int currentQuantity,
    int thresholdValue);

  string GetObserverName();
  string GetObserverRole();
}