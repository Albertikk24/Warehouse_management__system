namespace WarehouseSystem.Models;

public class Order
{
  public string OrderNumber { get; set; }
  public Dictionary<string, int> OrderItems { get; set; }
  public DateTime CreationTime { get; set; }
  public string OrderStatus { get; set; }

  private const string STATUS_CREATED = "Создан";
  private const string STATUS_COMPLETED = "Выполнен";
  private const string STATUS_REJECTED = "Отклонен";

  public Order(string orderNumber)
  {
    OrderNumber = orderNumber;
    OrderItems = new Dictionary<string, int>();
    CreationTime = DateTime.Now;
    OrderStatus = STATUS_CREATED;
  }

  public void AddOrderItem(string productSku, int requestedQuantity)
  {
    if (OrderItems.ContainsKey(productSku))
    {
      OrderItems[productSku] += requestedQuantity;
    }
    else
    {
      OrderItems.Add(productSku, requestedQuantity);
    }
  }

  public void MarkAsCompleted()
  {
    OrderStatus = STATUS_COMPLETED;
  }

  public void MarkAsRejected()
  {
    OrderStatus = STATUS_REJECTED;
  }

  public override string ToString()
  {
    return $"Заказ {OrderNumber} от {CreationTime:dd.MM.yyyy HH:mm}, статус: {OrderStatus}";
  }
}