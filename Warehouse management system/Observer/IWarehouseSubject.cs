namespace WarehouseSystem.Observer;

// Паттерн Наблюдатель: интерфейс наблюдаемого объекта
public interface IWarehouseSubject {
  // Подписка наблюдателя
  void AttachObserver(IWarehouseObserver observer);
  
  // Отписка наблюдателя
  void DetachObserver(IWarehouseObserver observer);
  
  // Оповещение всех наблюдателей
  void NotifyObservers(string notificationMessage, string productSku, int currentQuantity);
}