namespace WarehouseSystem.Models;

public class Product
{
  public string ProductName { get; set; }
  public string ProductSku { get; set; }
  public int CurrentQuantity { get; set; }
  public int MinimumThreshold { get; set; }

  private const int PERCENT_FOR_URGENT_WARNING = 50;
  private const int PERCENT_FOR_NORMAL_WARNING = 30;

  public Product(string productName, string productSku, int initialQuantity, int thresholdValue)
  {
    ProductName = productName;
    ProductSku = productSku;
    CurrentQuantity = initialQuantity;
    MinimumThreshold = thresholdValue;
  }

  // Проверка: остаток ниже порога
  public bool IsBelowThreshold()
  {
    return CurrentQuantity <= MinimumThreshold;
  }

  // Расчет процента критичности
  public int CalculateCriticalityPercent()
  {
    if (MinimumThreshold <= 0)
    {
      return 0;
    }

    return (CurrentQuantity * 100) / MinimumThreshold;
  }

  public override string ToString()
  {
    return $"{ProductName} ({ProductSku}) - {CurrentQuantity} шт. (порог: {MinimumThreshold})";
  }
}