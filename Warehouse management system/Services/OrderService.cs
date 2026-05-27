using WarehouseSystem.Models;
using WarehouseSystem.Observer;
using WarehouseSystem.Infrastructure;

namespace WarehouseSystem.Services;

// Сервис обработки заказов
public static class OrderService
{
  private const int ZERO_QUANTITY = 0;

  public static bool ProcessOrder(Warehouse warehouse, Order clientOrder)
  {
    Logger.Instance.LogMessage($"Создан заказ {clientOrder.OrderNumber}");

    bool hasAllItemsInStock = CheckStockAvailability(warehouse, clientOrder);

    if (!hasAllItemsInStock)
    {
      Logger.Instance.LogMessage($"Ошибка заказа {clientOrder.OrderNumber}: недостаточно товаров");
      clientOrder.MarkAsRejected();
      return false;
    }

    DeductOrderItems(warehouse, clientOrder);

    clientOrder.MarkAsCompleted();
    Logger.Instance.LogMessage($"Заказ {clientOrder.OrderNumber} выполнен успешно");

    return true;
  }

  // Проверка наличия всех товаров на складе
  private static bool CheckStockAvailability(Warehouse warehouse, Order clientOrder)
  {
    Dictionary<string, int>.KeyCollection orderItemKeys = clientOrder.OrderItems.Keys;
    List<string> productSkuList = new List<string>(orderItemKeys);

    for (int itemIndex = 0; itemIndex < productSkuList.Count; itemIndex++)
    {
      string currentSku = productSkuList[itemIndex];
      int requestedQuantity = clientOrder.OrderItems[currentSku];

      if (!warehouse.HasEnoughStock(currentSku, requestedQuantity))
      {
        Product targetProduct = warehouse.GetProductBySku(currentSku);
        string errorDetails = targetProduct == null
          ? $"Товар {currentSku} не найден"
          : $"Товар {targetProduct.ProductName}. Доступно: {targetProduct.CurrentQuantity}, запрошено: {requestedQuantity}";

        Logger.Instance.LogMessage($"Недостаточно товара: {errorDetails}");
        return false;
      }
    }

    return true;
  }

  // Списание товаров со склада
  private static void DeductOrderItems(Warehouse warehouse, Order clientOrder)
  {
    List<string> productSkuList = new List<string>(clientOrder.OrderItems.Keys);

    for (int itemIndex = 0; itemIndex < productSkuList.Count; itemIndex++)
    {
      string currentSku = productSkuList[itemIndex];
      int requestedQuantity = clientOrder.OrderItems[currentSku];

      Product targetProduct = warehouse.GetProductBySku(currentSku);

      if (targetProduct != null)
      {
        int newStockQuantity = targetProduct.CurrentQuantity - requestedQuantity;
        warehouse.UpdateProductStock(currentSku, newStockQuantity, $"Заказ {clientOrder.OrderNumber}");
      }
    }
  }

  public static void DisplayOrderDetails(Order clientOrder)
  {
    Console.WriteLine($"\n=== {clientOrder} ===");

    if (clientOrder.OrderItems.Count == ZERO_QUANTITY)
    {
      Console.WriteLine("  Заказ пуст");
      Console.WriteLine();
      return;
    }

    List<string> productSkuList = new List<string>(clientOrder.OrderItems.Keys);

    for (int itemIndex = 0; itemIndex < productSkuList.Count; itemIndex++)
    {
      string currentSku = productSkuList[itemIndex];
      int itemQuantity = clientOrder.OrderItems[currentSku];

      Console.WriteLine($"  Товар: {currentSku}, количество: {itemQuantity} шт.");
    }

    Console.WriteLine();
  }
}