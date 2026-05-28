namespace WarehouseSystem.Models;

public class Order {
  // ========== СВОЙСТВА ==========
  public string OrderNumber { get; set; }
  public Dictionary<string, int> OrderItems { get; set; }
  public DateTime CreationTime { get; set; }
  public string OrderStatus { get; set; }

  // ========== КОНСТАНТЫ СТАТУСОВ ==========
  private const string STATUS_CREATED = "Создан";
  private const string STATUS_COMPLETED = "Выполнен";
  private const string STATUS_REJECTED = "Отклонен";

  // ========== КОНСТРУКТОР ==========
  public Order(string orderNumber) {
    OrderNumber = orderNumber;
    OrderItems = new Dictionary<string, int>();
    CreationTime = DateTime.Now;
    OrderStatus = STATUS_CREATED;
  }

  // ========== ДОБАВЛЕНИЕ ПОЗИЦИИ В ЗАКАЗ ==========
  public void AddOrderItem(string productSku, int requestedQuantity) {
    if (OrderItems.ContainsKey(productSku)) {
      OrderItems[productSku] += requestedQuantity;
    } else {
      OrderItems.Add(productSku, requestedQuantity);
    }
  }

  // ========== ИЗМЕНЕНИЕ СТАТУСА ==========
  public void MarkAsCompleted() {
    OrderStatus = STATUS_COMPLETED;
  }

  public void MarkAsRejected() {
    OrderStatus = STATUS_REJECTED;
  }

  // ========== ПЕРЕОПРЕДЕЛЕНИЕ TO STRING ==========
  public override string ToString() {
    return $"Заказ {OrderNumber} от {CreationTime:dd.MM.yyyy HH:mm}, статус: {OrderStatus}";
  }
}