using WarehouseSystem.Models;

namespace WarehouseSystem.Infrastructure;

// Реализация репозитория с использованием Dictionary
public class ProductRepository : IProductRepository {
  // ========== ПОЛЯ ==========
  private Dictionary<string, Product> _productStorage;

  // ========== КОНСТРУКТОР ==========
  public ProductRepository() {
    _productStorage = new Dictionary<string, Product>();
    Logger.Instance.LogMessage("Репозиторий товаров инициализирован");
  }

  // ========== ДОБАВЛЕНИЕ ==========
  public void Add(Product product) {
    if (product == null) {
      Logger.Instance.LogMessage("Ошибка: товар не может быть null");
      return;
    }
    // Проверка на дубликаты по артикулу
    if (_productStorage.ContainsKey(product.ProductSku)) {
      Logger.Instance.LogMessage($"Ошибка: товар {product.ProductSku} уже существует");
      return;
    }
    _productStorage.Add(product.ProductSku, product);
    Logger.Instance.LogMessage($"Товар добавлен: {product.ProductName}");
  }

  // ========== ПОЛУЧЕНИЕ ПО АРТИКУЛУ ==========
  public Product? GetBySku(string productSku) {
    if (string.IsNullOrEmpty(productSku)) {
      return null;
    }
    if (_productStorage.ContainsKey(productSku)) {
      return _productStorage[productSku];
    }
    return null;
  }

  // ========== ПОЛУЧЕНИЕ ВСЕХ ТОВАРОВ ==========
  public List<Product> GetAll() {
    return new List<Product>(_productStorage.Values);
  }

  // ========== ПОЛУЧЕНИЕ ВСЕХ АРТИКУЛОВ ==========
  public List<string> GetAllSkus() {
    return new List<string>(_productStorage.Keys);
  }

  // ========== ПРОВЕРКА СУЩЕСТВОВАНИЯ ==========
  public bool Exists(string productSku) {
    return !string.IsNullOrEmpty(productSku) && _productStorage.ContainsKey(productSku);
  }

  // ========== УДАЛЕНИЕ ==========
  public bool Remove(string productSku) {
    if (string.IsNullOrEmpty(productSku)) {
      return false;
    }
    if (_productStorage.ContainsKey(productSku)) {
      Product removed = _productStorage[productSku];
      _productStorage.Remove(productSku);
      Logger.Instance.LogMessage($"Товар удален: {removed.ProductName}");
      return true;
    }
    return false;
  }

  // ========== ОБНОВЛЕНИЕ ==========
  public void Update(Product product) {
    if (product == null) {
      Logger.Instance.LogMessage("Ошибка: товар не может быть null");
      return;
    }
    if (_productStorage.ContainsKey(product.ProductSku)) {
      _productStorage[product.ProductSku] = product;
      Logger.Instance.LogMessage($"Товар обновлен: {product.ProductName}");
    }
  }

  // ========== КОЛИЧЕСТВО ТОВАРОВ ==========
  public int Count() {
    return _productStorage.Count;
  }
}