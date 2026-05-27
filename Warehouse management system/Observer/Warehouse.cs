using WarehouseSystem.Models;
using WarehouseSystem.Infrastructure;

namespace WarehouseSystem.Observer;

// Паттерн Наблюдатель: конкретный наблюдаемый объект (склад)
public class Warehouse : IWarehouseSubject
{
  private List<IWarehouseObserver> _subscribedObservers;
  private Dictionary<string, Product> _productStorage;
  private static int _warehouseIdCounter = 1;

  public string WarehouseIdentifier { get; private set; }

  public Warehouse()
  {
    _subscribedObservers = new List<IWarehouseObserver>();
    _productStorage = new Dictionary<string, Product>();
    WarehouseIdentifier = $"WR-{_warehouseIdCounter++:D3}";

    Logger.Instance.LogMessage($"Создан склад {WarehouseIdentifier}");
  }

  public void AttachObserver(IWarehouseObserver observer)
  {
    if (observer == null)
    {
      Logger.Instance.LogMessage("Ошибка: попытка подписки пустого наблюдателя");
      return;
    }

    if (_subscribedObservers.Contains(observer))
    {
      Logger.Instance.LogMessage($"Наблюдатель {observer.GetObserverName()} уже подписан");
      return;
    }

    _subscribedObservers.Add(observer);
    Logger.Instance.LogMessage($"Подписан наблюдатель: {observer.GetObserverRole()} {observer.GetObserverName()}");
  }

  public void DetachObserver(IWarehouseObserver observer)
  {
    if (_subscribedObservers.Remove(observer))
    {
      Logger.Instance.LogMessage($"Отписан наблюдатель: {observer.GetObserverRole()} {observer.GetObserverName()}");
    }
  }

  public void NotifyObservers(string notificationMessage, string productSku, int currentQuantity)
  {
    if (!_productStorage.ContainsKey(productSku))
    {
      return;
    }

    Product targetProduct = _productStorage[productSku];

    for (int observerIndex = 0; observerIndex < _subscribedObservers.Count; observerIndex++)
    {
      IWarehouseObserver currentObserver = _subscribedObservers[observerIndex];
      currentObserver.Update(
        notificationMessage,
        productSku,
        targetProduct.ProductName,
        currentQuantity,
        targetProduct.MinimumThreshold
      );
    }
  }

  public void AddNewProduct(Product newProduct)
  {
    if (_productStorage.ContainsKey(newProduct.ProductSku))
    {
      Logger.Instance.LogMessage($"Товар {newProduct.ProductSku} уже существует");
      return;
    }

    _productStorage.Add(newProduct.ProductSku, newProduct);
    Logger.Instance.LogMessage($"Добавлен товар: {newProduct}");

    if (newProduct.IsBelowThreshold())
    {
      NotifyObservers("Критический остаток при добавлении", newProduct.ProductSku, newProduct.CurrentQuantity);
    }
  }

  public Product? GetProductBySku(string productSku)
  {
    if (_productStorage.ContainsKey(productSku))
    {
      return _productStorage[productSku];
    }

    return null;
  }

  public void UpdateProductStock(string productSku, int newQuantity, string operationDescription)
  {
    if (!_productStorage.ContainsKey(productSku))
    {
      Logger.Instance.LogMessage($"Ошибка: товар {productSku} не найден");
      return;
    }

    Product targetProduct = _productStorage[productSku];
    int oldQuantity = targetProduct.CurrentQuantity;
    targetProduct.CurrentQuantity = newQuantity;

    Logger.Instance.LogMessage($"{operationDescription}: {targetProduct.ProductName} ({productSku}) - {oldQuantity} → {newQuantity} шт.");

    if (targetProduct.IsBelowThreshold())
    {
      NotifyObservers("Внимание! Товар достиг порогового значения", productSku, newQuantity);
    }
    else if (oldQuantity <= targetProduct.MinimumThreshold && newQuantity > targetProduct.MinimumThreshold)
    {
      NotifyObservers("Остаток товара восстановлен до нормы", productSku, newQuantity);
    }
  }

  public bool HasEnoughStock(string productSku, int requestedQuantity)
  {
    if (!_productStorage.ContainsKey(productSku))
    {
      return false;
    }

    return _productStorage[productSku].CurrentQuantity >= requestedQuantity;
  }

  public void DisplayAllProducts()
  {
    Console.WriteLine("\n=== Текущие остатки на складе ===");

    if (_productStorage.Count == 0)
    {
      Console.WriteLine("Склад пуст");
      Console.WriteLine();
      return;
    }

    List<string> productKeys = new List<string>(_productStorage.Keys);

    for (int keyIndex = 0; keyIndex < productKeys.Count; keyIndex++)
    {
      string currentSku = productKeys[keyIndex];
      Product currentProduct = _productStorage[currentSku];

      string warningMarker = currentProduct.IsBelowThreshold() ? " ВНИМАНИЕ: КРИТИЧЕСКИЙ ОСТАТОК" : "";
      Console.WriteLine($"  {currentProduct}{warningMarker}");
    }

    Console.WriteLine();
  }

  public List<Product> GetAllProducts()
  {
    return new List<Product>(_productStorage.Values);
  }

  public List<string> GetAllProductSkus()
  {
    return new List<string>(_productStorage.Keys);
  }
}