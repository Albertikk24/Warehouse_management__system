namespace WarehouseSystem.Models;

public class Product {
  // ========== СВОЙСТВА ==========
  public string ProductName { get; set; }
  public string ProductSku { get; set; }
  public int CurrentQuantity { get; set; }
  public int MinimumThreshold { get; set; }

  private const int PERCENT_FOR_URGENT_WARNING = 50;
  private const int PERCENT_FOR_NORMAL_WARNING = 30;

  // ========== КОНСТРУКТОР ==========
  public Product(string productName, string productSku, int initialQuantity, int thresholdValue) {
    ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
    ProductSku = productSku ?? throw new ArgumentNullException(nameof(productSku));
    CurrentQuantity = initialQuantity;
    MinimumThreshold = thresholdValue;
  }

  // ========== ПРОВЕРКА ДОСТИЖЕНИЯ ПОРОГА ==========
  public bool IsBelowThreshold() {
    return CurrentQuantity <= MinimumThreshold;
  }

  // ========== РАСЧЕТ ПРОЦЕНТА КРИТИЧНОСТИ ==========
  public int CalculateCriticalityPercent() {
    if (MinimumThreshold <= 0) {
      return 0;
    }
    return (CurrentQuantity * 100) / MinimumThreshold;
  }

  // ========== ПЕРЕОПРЕДЕЛЕНИЕ TO STRING ==========
  public override string ToString() {
    return $"{ProductName} ({ProductSku}) - {CurrentQuantity} шт. (порог: {MinimumThreshold})";
  }
}