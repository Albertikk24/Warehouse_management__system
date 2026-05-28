using WarehouseSystem.Models;
using WarehouseSystem.Infrastructure;

namespace WarehouseSystem.Services;

// Сервис для работы с остатками товаров
public class StockService {
  // ========== ПОЛЯ ==========
  private IProductRepository _repository;

  // ========== КОНСТРУКТОР ==========
  public StockService(IProductRepository repository) {
    _repository = repository ?? throw new ArgumentNullException(nameof(repository));
  }

  // ========== ПРОВЕРКА ДОСТАТОЧНОСТИ ОСТАТКА ==========
  public bool HasEnoughStock(string productSku, int requestedQuantity) {
    if (string.IsNullOrEmpty(productSku)) return false;
    Product? product = _repository.GetBySku(productSku);
    return product != null && product.CurrentQuantity >= requestedQuantity;
  }

  // ========== ПОЛУЧЕНИЕ ТЕКУЩЕГО ОСТАТКА ==========
  public int GetCurrentStock(string productSku) {
    if (string.IsNullOrEmpty(productSku)) return -1;
    Product? product = _repository.GetBySku(productSku);
    return product?.CurrentQuantity ?? -1;
  }

  // ========== ОБНОВЛЕНИЕ ОСТАТКА ==========
  public bool UpdateStock(string productSku, int newQuantity, string operationDescription) {
    if (string.IsNullOrEmpty(productSku)) return false;
    Product? product = _repository.GetBySku(productSku);
    if (product == null) return false;
    
    int oldQuantity = product.CurrentQuantity;
    product.CurrentQuantity = newQuantity;
    _repository.Update(product);
    
    Logger.Instance.LogMessage($"{operationDescription}: {product.ProductName} - {oldQuantity} → {newQuantity} шт.");
    return true;
  }

  // ========== УВЕЛИЧЕНИЕ ОСТАТКА (ПОСТАВКА) ==========
  public bool AddStock(string productSku, int addedQuantity) {
    if (string.IsNullOrEmpty(productSku)) return false;
    Product? product = _repository.GetBySku(productSku);
    if (product == null) return false;
    return UpdateStock(productSku, product.CurrentQuantity + addedQuantity, $"Поставка +{addedQuantity} шт.");
  }

  // ========== УМЕНЬШЕНИЕ ОСТАТКА (СПИСАНИЕ) ==========
  public bool DeductStock(string productSku, int deductedQuantity, string reason) {
    if (string.IsNullOrEmpty(productSku)) return false;
    Product? product = _repository.GetBySku(productSku);
    if (product == null || product.CurrentQuantity < deductedQuantity) return false;
    return UpdateStock(productSku, product.CurrentQuantity - deductedQuantity, reason);
  }

  // ========== ПРОВЕРКА НИЖЕ ПОРОГА ==========
  public bool IsBelowThreshold(string productSku) {
    if (string.IsNullOrEmpty(productSku)) return false;
    Product? product = _repository.GetBySku(productSku);
    return product != null && product.IsBelowThreshold();
  }

  // ========== ПОЛУЧЕНИЕ ТОВАРА СО СТАТУСОМ ПОРОГА ==========
  public (Product? product, bool isBelowThreshold) GetProductWithStatus(string productSku) {
    if (string.IsNullOrEmpty(productSku)) {
      return (null, false);
    }
    Product? product = _repository.GetBySku(productSku);
    return (product, product != null && product.IsBelowThreshold());
  }
}