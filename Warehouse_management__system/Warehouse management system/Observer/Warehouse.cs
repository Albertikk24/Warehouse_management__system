using WarehouseSystem.Models;
using WarehouseSystem.Infrastructure;
using WarehouseSystem.Services;

namespace WarehouseSystem.Observer;

public class Warehouse : IWarehouseSubject {
  // ========== ПОЛЯ ==========
  private List<IWarehouseObserver> _subscribedObservers;  // Список подписанных наблюдателей
  private IProductRepository _repository;                  // Репозиторий товаров
  private StockService _stockService;                      // Сервис работы с остатками
  private static int _warehouseIdCounter = 1;              // Счетчик для идентификатора

  public string WarehouseIdentifier { get; private set; }

  // ========== КОНСТРУКТОР ==========
  public Warehouse(IProductRepository repository, StockService stockService) {
    _subscribedObservers = new List<IWarehouseObserver>();
    _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    _stockService = stockService ?? throw new ArgumentNullException(nameof(stockService));
    WarehouseIdentifier = $"WR-{_warehouseIdCounter++:D3}";
    Logger.Instance.LogMessage($"Создан склад {WarehouseIdentifier}");
  }

  // ========== УПРАВЛЕНИЕ ПОДПИСКОЙ ==========
  public void AttachObserver(IWarehouseObserver observer) {
    if (observer == null) {
      Logger.Instance.LogMessage("Ошибка: попытка подписки пустого наблюдателя");
      return;
    }
    if (_subscribedObservers.Contains(observer)) {
      Logger.Instance.LogMessage($"Наблюдатель {observer.GetObserverName()} уже подписан");
      return;
    }
    _subscribedObservers.Add(observer);
    Logger.Instance.LogMessage($"Подписан наблюдатель: {observer.GetObserverRole()} {observer.GetObserverName()}");
  }

  public void DetachObserver(IWarehouseObserver observer) {
    if (observer == null) return;
    if (_subscribedObservers.Remove(observer)) {
      Logger.Instance.LogMessage($"Отписан наблюдатель: {observer.GetObserverRole()} {observer.GetObserverName()}");
    }
  }

  // ========== ОПОВЕЩЕНИЕ НАБЛЮДАТЕЛЕЙ ==========
  public void NotifyObservers(string notificationMessage, string productSku, int currentQuantity) {
    if (string.IsNullOrEmpty(productSku)) return;
    Product? targetProduct = _repository.GetBySku(productSku);
    if (targetProduct == null) return;
    
    for (int observerIndex = 0; observerIndex < _subscribedObservers.Count; observerIndex++) {
      IWarehouseObserver currentObserver = _subscribedObservers[observerIndex];
      currentObserver.Update(notificationMessage, productSku, targetProduct.ProductName, currentQuantity, targetProduct.MinimumThreshold);
    }
  }

  // ========== ПРОВЕРКА ПОРОГА С УВЕДОМЛЕНИЕМ ==========
  public void CheckThresholdAndNotify(string productSku) {
    (Product? product, bool isBelowThreshold) = _stockService.GetProductWithStatus(productSku);
    if (product != null && isBelowThreshold) {
      NotifyObservers("Внимание! Товар достиг порогового значения", productSku, product.CurrentQuantity);
    }
  }

  // ========== ПРОВЕРКА ВОССТАНОВЛЕНИЯ С УВЕДОМЛЕНИЕМ ==========
  public void CheckRecoveryAndNotify(string productSku, int oldQuantity) {
    Product? product = _repository.GetBySku(productSku);
    if (product != null && oldQuantity <= product.MinimumThreshold && product.CurrentQuantity > product.MinimumThreshold) {
      NotifyObservers("Остаток товара восстановлен до нормы", productSku, product.CurrentQuantity);
    }
  }

  // ========== МЕТОДЫ-ОБЕРТКИ ДЛЯ РАБОТЫ С РЕПОЗИТОРИЕМ ==========
  public void AddNewProduct(Product newProduct) {
    if (newProduct == null) {
      Logger.Instance.LogMessage("Ошибка: нельзя добавить пустой товар");
      return;
    }
    _repository.Add(newProduct);
  }

  public List<string> GetAllProductSkus() {
    return _repository.GetAllSkus();
  }

  public List<Product> GetAllProducts() {
    return _repository.GetAll();
  }

  public Product? GetProductBySku(string productSku) {
    return _repository.GetBySku(productSku);
  }

  public bool HasEnoughStock(string productSku, int requestedQuantity) {
    return _stockService.HasEnoughStock(productSku, requestedQuantity);
  }

  public void UpdateProductStock(string productSku, int newQuantity, string operationDescription) {
    int oldQuantity = _stockService.GetCurrentStock(productSku);
    _stockService.UpdateStock(productSku, newQuantity, operationDescription);
    CheckThresholdAndNotify(productSku);
    CheckRecoveryAndNotify(productSku, oldQuantity);
  }

  // ========== ДОСТУП К ЗАВИСИМОСТЯМ ==========
  public IProductRepository GetRepository() {
    return _repository;
  }

  public StockService GetStockService() {
    return _stockService;
  }

  // ========== ОТОБРАЖЕНИЕ ВСЕХ ТОВАРОВ ==========
  public void DisplayAllProducts() {
    var products = _repository.GetAll();
    string output = "\n=== Текущие остатки на складе ===\n";
    if (products.Count == 0) {
      output += "Склад пуст\n";
    } else {
      for (int i = 0; i < products.Count; i++) {
        Product product = products[i];
        string warning = product.IsBelowThreshold() ? " ВНИМАНИЕ: КРИТИЧЕСКИЙ ОСТАТОК" : "";
        output += $"  {i + 1}. {product}{warning}\n";
      }
      output += $"\nВсего товаров: {products.Count}\n";
    }
    Console.WriteLine(output);
  }
}