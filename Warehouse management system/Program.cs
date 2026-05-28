using WarehouseSystem.Models;
using WarehouseSystem.Observer;
using WarehouseSystem.Services;
using WarehouseSystem.Infrastructure;
using WarehouseSystem.UI;

namespace WarehouseSystem;

// Главный класс приложения
public class Program {
  public static void Main(string[] args) {
    // ========== НАСТРОЙКА КОНСОЛИ ==========
    Console.Title = "Система управления складом";

    // ========== ИНИЦИАЛИЗАЦИЯ ЗАВИСИМОСТЕЙ ==========
    // Создание репозитория для хранения товаров
    IProductRepository repository = new ProductRepository();
    
    // Создание сервиса для работы с остатками
    StockService stockService = new StockService(repository);
    
    // Создание склада (наблюдаемый объект)
    Warehouse mainWarehouse = new Warehouse(repository, stockService);

    // ========== ПОДПИСКА СОТРУДНИКОВ ПО УМОЛЧАНИЮ ==========
    Observers.Manager manager = new Observers.Manager("Иван Петров", "ivan@company.com");
    Observers.Storekeeper storekeeper = new Observers.Storekeeper("Сергей Сидоров", "+7-999-123-4567");

    mainWarehouse.AttachObserver(manager);
    mainWarehouse.AttachObserver(storekeeper);

    // ========== ДОБАВЛЕНИЕ ТЕСТОВЫХ ТОВАРОВ ==========
    Product laptop = new Product("Ноутбук", "NB-001", 10, 5);
    Product phone = new Product("Смартфон", "PH-002", 3, 10);
    Product monitor = new Product("Монитор", "MN-003", 15, 4);

    mainWarehouse.AddNewProduct(laptop);
    mainWarehouse.AddNewProduct(phone);
    mainWarehouse.AddNewProduct(monitor);

    mainWarehouse.DisplayAllProducts();

    // ========== ЗАПУСК ИНТЕРАКТИВНОГО МЕНЮ ==========
    ConsoleMenu menu = new ConsoleMenu(mainWarehouse);
    menu.Show();
  }
}