namespace WarehouseSystem.Observer;

// Паттерн Наблюдатель: интерфейс наблюдаемого объекта
public interface IWarehouseSubject
{
  void AttachObserver(IWarehouseObserver observer);   // Подписать наблюдателя
  void DetachObserver(IWarehouseObserver observer);   // Отписать наблюдателя
  void NotifyObservers(string notificationMessage, string productSku, int currentQuantity);  // Оповестить всех
}