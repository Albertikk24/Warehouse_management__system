using WarehouseSystem.Models;
using WarehouseSystem.Observer;
using WarehouseSystem.Infrastructure;

namespace WarehouseSystem.Services;

// Сервис для обработки заказов
public static class OrderService {
  // ========== ХРАНЕНИЕ ИСТОРИИ ЗАКАЗОВ ==========
  private static List<Order> _orderHistory = new List<Order>();

  // ========== ОСНОВНОЙ МЕТОД ОБРАБОТКИ ЗАКАЗА ==========
  public static bool ProcessOrder(Warehouse warehouse, Order clientOrder) {
    if (warehouse == null || clientOrder == null) {
      Logger.Instance.LogMessage("Ошибка: заказ или склад не могут быть null");
      return false;
    }

    Logger.Instance.LogMessage($"Создан заказ {clientOrder.OrderNumber}");
    
    // Шаг 1: проверка наличия всех товаров
    bool hasAllItemsInStock = CheckStockAvailability(warehouse, clientOrder);
    
    if (!hasAllItemsInStock) {
      Logger.Instance.LogMessage($"Ошибка заказа {clientOrder.OrderNumber}: недостаточно товаров");
      clientOrder.MarkAsRejected();
      _orderHistory.Add(clientOrder);
      return false;
    }
    
    // Шаг 2: списание товаров
    DeductOrderItems(warehouse, clientOrder);
    
    // Шаг 3: завершение заказа
    clientOrder.MarkAsCompleted();
    Logger.Instance.LogMessage($"Заказ {clientOrder.OrderNumber} выполнен успешно");
    _orderHistory.Add(clientOrder);
    
    return true;
  }

  // ========== ПРОВЕРКА НАЛИЧИЯ ВСЕХ ТОВАРОВ ==========
  private static bool CheckStockAvailability(Warehouse warehouse, Order clientOrder) {
    if (warehouse == null || clientOrder == null) return false;
    
    var stockService = warehouse.GetStockService();
    
    foreach (var item in clientOrder.OrderItems) {
      string currentSku = item.Key;
      int requestedQuantity = item.Value;
      
      if (!stockService.HasEnoughStock(currentSku, requestedQuantity)) {
        Product? targetProduct = warehouse.GetProductBySku(currentSku);
        string errorDetails = targetProduct == null 
          ? $"Товар {currentSku} не найден" 
          : $"Товар {targetProduct.ProductName}. Доступно: {targetProduct.CurrentQuantity}, запрошено: {requestedQuantity}";
        
        Logger.Instance.LogMessage($"Недостаточно товара: {errorDetails}");
        return false;
      }
    }
    return true;
  }

  // ========== СПИСАНИЕ ТОВАРОВ СО СКЛАДА ==========
  private static void DeductOrderItems(Warehouse warehouse, Order clientOrder) {
    if (warehouse == null || clientOrder == null) return;
    
    var stockService = warehouse.GetStockService();
    
    foreach (var item in clientOrder.OrderItems) {
      string currentSku = item.Key;
      int requestedQuantity = item.Value;
      
      stockService.DeductStock(currentSku, requestedQuantity, $"Заказ {clientOrder.OrderNumber}");
      warehouse.CheckThresholdAndNotify(currentSku);
    }
  }

  // ========== ОТОБРАЖЕНИЕ ДЕТАЛЕЙ ЗАКАЗА ==========
  public static void DisplayOrderDetails(Order clientOrder) {
    if (clientOrder == null) {
      Console.WriteLine("\n=== Заказ не существует ===\n");
      return;
    }

    string output = $"\n=== {clientOrder} ===\n";
    
    if (clientOrder.OrderItems.Count == 0) {
      output += "  Заказ пуст\n";
    } else {
      foreach (var item in clientOrder.OrderItems) {
        output += $"  Товар: {item.Key}, количество: {item.Value} шт.\n";
      }
    }
    
    Console.WriteLine(output);
  }

  // ========== ПОКАЗ ИСТОРИИ ЗАКАЗОВ ==========
  public static void ShowOrderHistory() {
    string output = "\n=== ИСТОРИЯ ЗАКАЗОВ ===\n";
    
    if (_orderHistory.Count == 0) {
      output += "История заказов пуста\n";
    } else {
      for (int i = 0; i < _orderHistory.Count; i++) {
        Order order = _orderHistory[i];
        output += $"{i + 1}. {order}\n";
        foreach (var item in order.OrderItems) {
          output += $"     Товар: {item.Key}, количество: {item.Value} шт.\n";
        }
      }
    }
    
    Console.WriteLine(output);
  }
}