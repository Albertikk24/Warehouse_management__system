using WarehouseSystem.Models;
using WarehouseSystem.Observer;
using WarehouseSystem.Infrastructure;

namespace WarehouseSystem.Services;

// Сервис поставок
public static class SupplyService
{
  public static void AddStockToWarehouse(Warehouse warehouse, string productSku, int addedQuantity)
  {
    Product targetProduct = warehouse.GetProductBySku(productSku);

    if (targetProduct == null)
    {
      Logger.Instance.LogMessage($"Ошибка: товар {productSku} не найден. Невозможно добавить поставку.");
      return;
    }

    int newTotalQuantity = targetProduct.CurrentQuantity + addedQuantity;
    warehouse.UpdateProductStock(productSku, newTotalQuantity, $"Поставка +{addedQuantity} шт.");

    Logger.Instance.LogMessage($"Поставка товара {targetProduct.ProductName} ({productSku}): +{addedQuantity} шт. Итого: {newTotalQuantity} шт.");
  }

  public static void AddNewProductViaSupply(Warehouse warehouse, Product newProduct, int initialQuantity)
  {
    Product existingProduct = warehouse.GetProductBySku(newProduct.ProductSku);

    if (existingProduct == null)
    {
      warehouse.AddNewProduct(newProduct);
      Logger.Instance.LogMessage($"Новый товар добавлен через поставку: {newProduct.ProductName}");
    }

    AddStockToWarehouse(warehouse, newProduct.ProductSku, initialQuantity);
  }
}