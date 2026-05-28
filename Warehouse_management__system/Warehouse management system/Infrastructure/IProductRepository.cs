using WarehouseSystem.Models;

namespace WarehouseSystem.Infrastructure;

// Репозиторий для хранения товаров - отделяет логику хранения от бизнес-логики
public interface IProductRepository {
  // ========== БАЗОВЫЕ CRUD ОПЕРАЦИИ ==========
  void Add(Product product);                    // Добавление товара
  Product? GetBySku(string productSku);         // Получение по артикулу (может вернуть null)
  List<Product> GetAll();                       // Получение всех товаров
  List<string> GetAllSkus();                    // Получение всех артикулов
  bool Exists(string productSku);               // Проверка существования
  bool Remove(string productSku);               // Удаление товара
  void Update(Product product);                 // Обновление товара
  int Count();                                  // Количество товаров
}