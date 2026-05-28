using WarehouseSystem.Models;
using WarehouseSystem.Observer;
using WarehouseSystem.Infrastructure;

namespace WarehouseSystem.Services;

// Сервис для работы с поставками
public static class SupplyService {
  // ========== ДОБАВЛЕНИЕ ПОСТАВКИ НА СКЛАД ==========
  public static void AddStockToWarehouse(Warehouse warehouse, string productSku, int addedQuantity) {
    if (warehouse == null) {
      Logger.Instance.LogMessage("Ошибка: склад не может быть null");
      return;
    }
    
    if (string.IsNullOrEmpty(productSku)) {
      Logger.Instance.LogMessage("Ошибка: артикул товара не может быть пустым");
      return;
    }
    
    var stockService = warehouse.GetStockService();
    Product? targetProduct = warehouse.GetProductBySku(productSku);
    
    // Проверка существования товара
    if (targetProduct == null) {
      Logger.Instance.LogMessage($"Ошибка: товар {productSku} не найден");
      return;
    }
    
    int oldQuantity = targetProduct.CurrentQuantity;
    
    // Добавление остатка
    bool success = stockService.AddStock(productSku, addedQuantity);
    
    if (success) {
      Logger.Instance.LogMessage($"Поставка товара {targetProduct.ProductName}: +{addedQuantity} шт. Итого: {targetProduct.CurrentQuantity} шт.");
      // Проверка восстановления после поставки
      warehouse.CheckRecoveryAndNotify(productSku, oldQuantity);
    }
  }

  // ========== ДОБАВЛЕНИЕ НОВОГО ТОВАРА ЧЕРЕЗ ПОСТАВКУ ==========
  public static void AddNewProductViaSupply(Warehouse warehouse, Product newProduct, int initialQuantity) {
    if (warehouse == null) {
      Logger.Instance.LogMessage("Ошибка: склад не может быть null");
      return;
    }
    
    if (newProduct == null) {
      Logger.Instance.LogMessage("Ошибка: товар не может быть null");
      return;
    }
    
    Product? existingProduct = warehouse.GetProductBySku(newProduct.ProductSku);
    
    // Если товар новый - сначала добавляем в репозиторий
    if (existingProduct == null) {
      warehouse.AddNewProduct(newProduct);
      Logger.Instance.LogMessage($"Новый товар добавлен через поставку: {newProduct.ProductName}");
      AddStockToWarehouse(warehouse, newProduct.ProductSku, initialQuantity - newProduct.CurrentQuantity);
    } else {
      // Если товар уже есть - просто добавляем остаток
      AddStockToWarehouse(warehouse, newProduct.ProductSku, initialQuantity);
    }
  }
}