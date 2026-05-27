using WarehouseSystem.Models;
using WarehouseSystem.Observer;
using WarehouseSystem.Observers;
using WarehouseSystem.Services;
using WarehouseSystem.Infrastructure;

namespace WarehouseSystem;

public class Program
{
  private static Random _random = new Random();

  public static void Main(string[] args)
  {
    // ========== БЛОК 1: ИНИЦИАЛИЗАЦИЯ ==========
    Console.Title = "Система управления складом";

    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine(new string('=', 50));
    Console.WriteLine("     СИСТЕМА УПРАВЛЕНИЯ СКЛАДОМ");
    Console.WriteLine(new string('=', 50));
    Console.ResetColor();

    Warehouse mainWarehouse = new Warehouse();

    Manager operationsManager = new Manager("Иван Петров", "ivan.petrov@company.com");
    Storekeeper warehouseKeeper = new Storekeeper("Сергей Сидоров", "+7-999-123-4567");

    mainWarehouse.AttachObserver(operationsManager);
    mainWarehouse.AttachObserver(warehouseKeeper);

    Console.WriteLine("\n[OK] Система инициализирована. Персонал подписан на уведомления.\n");

    System.Threading.Thread.Sleep(1000);

    // ========== БЛОК 2: ДОБАВЛЕНИЕ ТОВАРОВ СО СЛУЧАЙНЫМИ ДАННЫМИ ==========
    Console.WriteLine(new string('-', 50));
    Console.WriteLine("ДОБАВЛЕНИЕ ТОВАРОВ");
    Console.WriteLine(new string('-', 50));

    List<string> productNames = new List<string> { "Ноутбук", "Смартфон", "Монитор", "Клавиатура", "Мышь", "Наушники", "Планшет", "Принтер" };
    List<string> productSkus = new List<string> { "NB-001", "PH-002", "MN-003", "KB-004", "MS-005", "HP-006", "TB-007", "PR-008" };

    for (int i = 0; i < 5; i++)
    {
      string name = productNames[_random.Next(productNames.Count)];
      string sku = productSkus[_random.Next(productSkus.Count)];
      int initialStock = _random.Next(5, 30);
      int threshold = _random.Next(3, 12);

      Product newProduct = new Product(name, sku, initialStock, threshold);
      mainWarehouse.AddNewProduct(newProduct);

      System.Threading.Thread.Sleep(300);
    }

    mainWarehouse.DisplayAllProducts();

    System.Threading.Thread.Sleep(1000);

    // ========== БЛОК 3: СОЗДАНИЕ СЛУЧАЙНОГО ЗАКАЗА ==========
    Console.WriteLine(new string('-', 50));
    Console.WriteLine("СОЗДАНИЕ ЗАКАЗА");
    Console.WriteLine(new string('-', 50));

    Order randomOrder = new Order($"ORD-{_random.Next(100, 999)}");

    List<string> existingSkus = mainWarehouse.GetAllProductSkus();

    int orderItemCount = _random.Next(1, 4);
    for (int i = 0; i < orderItemCount; i++)
    {
      string randomSku = existingSkus[_random.Next(existingSkus.Count)];
      int randomQuantity = _random.Next(1, 10);
      randomOrder.AddOrderItem(randomSku, randomQuantity);
    }

    OrderService.DisplayOrderDetails(randomOrder);

    bool orderProcessingResult = OrderService.ProcessOrder(mainWarehouse, randomOrder);
    Console.WriteLine($"Результат заказа: {(orderProcessingResult ? "ВЫПОЛНЕН" : "ОТКЛОНЕН")}\n");

    mainWarehouse.DisplayAllProducts();

    System.Threading.Thread.Sleep(1500);

    // ========== БЛОК 4: СЛУЧАЙНАЯ ПОСТАВКА ==========
    Console.WriteLine(new string('-', 50));
    Console.WriteLine("ПОСТАВКА ТОВАРОВ");
    Console.WriteLine(new string('-', 50));

    List<string> allSkus = mainWarehouse.GetAllProductSkus();
    for (int i = 0; i < 3; i++)
    {
      string randomSku = allSkus[_random.Next(allSkus.Count)];
      int supplyQuantity = _random.Next(10, 50);
      SupplyService.AddStockToWarehouse(mainWarehouse, randomSku, supplyQuantity);

      System.Threading.Thread.Sleep(500);
    }

    mainWarehouse.DisplayAllProducts();

    System.Threading.Thread.Sleep(1000);

    // ========== БЛОК 5: ДЕМОНСТРАЦИЯ ОТПИСКИ ==========
    Console.WriteLine(new string('-', 50));
    Console.WriteLine("ДЕМОНСТРАЦИЯ ОТПИСКИ");
    Console.WriteLine(new string('-', 50));

    Console.WriteLine("Отписываем кладовщика от уведомлений...");
    mainWarehouse.DetachObserver(warehouseKeeper);

    Console.WriteLine("\nИзменяем остаток товара (без уведомления кладовщика):");
    string testSku = allSkus[0];
    SupplyService.AddStockToWarehouse(mainWarehouse, testSku, -5);

    Console.WriteLine("\nВозвращаем кладовщика в подписку:");
    mainWarehouse.AttachObserver(warehouseKeeper);

    System.Threading.Thread.Sleep(1000);

    // ========== БЛОК 6: ВЫВОД ЛОГОВ ==========
    Logger.Instance.DisplayAllLogs();

    // ========== БЛОК 7: ЗАВЕРШЕНИЕ ==========
    Console.WriteLine(new string('=', 50));
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("РАБОТА СИСТЕМЫ ЗАВЕРШЕНА");
    Console.ResetColor();
    Console.WriteLine(new string('=', 50));

    Console.WriteLine("\nНажмите любую клавишу для выхода...");
    Console.ReadKey();
  }
}