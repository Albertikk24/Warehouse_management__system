namespace WarehouseSystem.Observer;

// Паттерн Наблюдатель: интерфейс наблюдателя
public interface IWarehouseObserver {
  // Метод, вызываемый при изменении состояния склада
  void Update(string message, string productSku, string productName, int currentQuantity, int thresholdValue);
  
  // Получение имени наблюдателя
  string GetObserverName();
  
  // Получение роли наблюдателя
  string GetObserverRole();
}