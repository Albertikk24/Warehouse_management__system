using WarehouseSystem.Models;
using WarehouseSystem.Observer;
using WarehouseSystem.Observers;
using WarehouseSystem.Services;
using WarehouseSystem.Infrastructure;

namespace WarehouseSystem.UI;

// Класс для отображения консольного меню
public class ConsoleMenu {
  // ========== ПОЛЯ ==========
  private Warehouse _warehouse;      // Ссылка на склад
  private Random _random = new Random();  // Генератор случайных чисел

  // ========== КОНСТРУКТОР ==========
  public ConsoleMenu(Warehouse warehouse) {
    _warehouse = warehouse ?? throw new ArgumentNullException(nameof(warehouse));
  }

  // ========== ГЛАВНЫЙ ЦИКЛ МЕНЮ ==========
  public void Show() {
    bool exitRequested = false;
    
    while (!exitRequested) {
      // Формирование главного меню (один вывод)
      string menu = "\n" + new string('=', 50) + "\n";
      menu += "     СИСТЕМА УПРАВЛЕНИЯ СКЛАДОМ\n";
      menu += new string('=', 50) + "\n";
      menu += "\n 1. Показать все товары";
      menu += "\n 2. Добавить товар";
      menu += "\n 3. Создать заказ";
      menu += "\n 4. Поставка товаров";
      menu += "\n 5. История заказов";
      menu += "\n 6. Показать логи";
      menu += "\n 7. Добавить тестовые товары (случайные)";
      menu += "\n 8. Подписка/отписка сотрудников";
      menu += "\n 0. Выход";
      menu += "\n" + new string('-', 50) + "\n";
      menu += "Выберите действие: ";
      
      // Отрисовка меню
      Console.Clear();
      Console.ForegroundColor = ConsoleColor.Cyan;
      Console.Write(menu);
      Console.ResetColor();
      
      string? choice = Console.ReadLine();
      
      // Обработка выбора пользователя
      switch (choice) {
        case "1": ShowAllProducts(); break;
        case "2": AddProductManually(); break;
        case "3": CreateOrderManually(); break;
        case "4": AddSupplyManually(); break;
        case "5": OrderService.ShowOrderHistory(); WaitForUser(); break;
        case "6": Logger.Instance.DisplayAllLogs(); WaitForUser(); break;
        case "7": AddRandomTestProducts(); break;
        case "8": ManageSubscriptions(); break;
        case "0": exitRequested = true; Console.WriteLine("\nВыход из системы..."); break;
        default: Console.WriteLine("\nНеверный выбор. Нажмите любую клавишу..."); Console.ReadKey(); break;
      }
    }
  }

  // ========== ОТОБРАЖЕНИЕ ВСЕХ ТОВАРОВ ==========
  private void ShowAllProducts() {
    // Получение списка товаров из склада
    var products = _warehouse.GetAllProducts();
    
    // Формирование вывода (один Console.WriteLine)
    string output = "\n=== Товары на складе ===\n";
    
    if (products.Count == 0) {
      output += "Склад пуст\n";
    } else {
      for (int i = 0; i < products.Count; i++) {
        Product product = products[i];
        // Добавление предупреждения если остаток ниже порога
        string warning = product.IsBelowThreshold() ? " [КРИТИЧЕСКИЙ ОСТАТОК]" : "";
        output += $"  {i + 1}. {product}{warning}\n";
      }
      output += $"\nВсего товаров: {products.Count}\n";
    }
    
    Console.Clear();
    Console.WriteLine(output);
    WaitForUser();
  }

  // ========== РУЧНОЕ ДОБАВЛЕНИЕ ТОВАРА ==========
  private void AddProductManually() {
    Console.Clear();
    Console.WriteLine("\n=== Добавление нового товара ===");
    
    // Ввод названия товара
    Console.Write("Название товара: ");
    string? name = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(name)) {
      Console.WriteLine("\nОшибка: название не может быть пустым");
      WaitForUser();
      return;
    }
    
    // Ввод артикула (с преобразованием в верхний регистр)
    Console.Write("Артикул (SKU): ");
    string? sku = Console.ReadLine()?.ToUpper();
    if (string.IsNullOrWhiteSpace(sku)) {
      Console.WriteLine("\nОшибка: артикул не может быть пустым");
      WaitForUser();
      return;
    }
    
    // Ввод начального количества с проверкой
    Console.Write("Начальное количество: ");
    if (!int.TryParse(Console.ReadLine(), out int quantity)) {
      Console.WriteLine("\nОшибка: введите число");
      WaitForUser();
      return;
    }
    
    // Ввод порогового значения с проверкой
    Console.Write("Пороговое значение: ");
    if (!int.TryParse(Console.ReadLine(), out int threshold)) {
      Console.WriteLine("\nОшибка: введите число");
      WaitForUser();
      return;
    }
    
    // Создание и добавление товара
    Product newProduct = new Product(name, sku, quantity, threshold);
    _warehouse.AddNewProduct(newProduct);
    
    Console.WriteLine($"\nТовар {name} добавлен на склад");
    WaitForUser();
  }

  // ========== РУЧНОЕ СОЗДАНИЕ ЗАКАЗА ==========
  private void CreateOrderManually() {
    Console.Clear();
    Console.WriteLine("\n=== Создание заказа ===");
    
    // Ввод номера заказа
    Console.Write("Номер заказа: ");
    string? orderNumber = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(orderNumber)) {
      Console.WriteLine("\nОшибка: номер заказа не может быть пустым");
      WaitForUser();
      return;
    }
    
    Order newOrder = new Order(orderNumber);
    bool addingItems = true;
    
    // Цикл добавления позиций в заказ
    while (addingItems) {
      // Получение актуального списка товаров
      var products = _warehouse.GetAllProducts();
      
      // Проверка что склад не пуст
      if (products.Count == 0) {
        Console.WriteLine("\nНа складе нет товаров. Сначала добавьте товары.");
        WaitForUser();
        return;
      }
      
      // Формирование списка доступных товаров
      string productList = "\nДоступные товары:\n";
      for (int i = 0; i < products.Count; i++) {
        productList += $"  {i + 1}. {products[i].ProductName} ({products[i].ProductSku}) - {products[i].CurrentQuantity} шт.\n";
      }
      Console.Write(productList);
      
      // Выбор товара
      Console.Write("Выберите товар (номер) или 0 для завершения: ");
      if (!int.TryParse(Console.ReadLine(), out int productChoice) || productChoice == 0) {
        if (productChoice == 0) addingItems = false;
        continue;
      }
      
      // Проверка корректности выбора
      if (productChoice < 1 || productChoice > products.Count) {
        Console.WriteLine("Неверный выбор");
        continue;
      }
      
      Product selected = products[productChoice - 1];
      
      // Ввод количества
      Console.Write($"Количество (доступно: {selected.CurrentQuantity}): ");
      if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0) {
        Console.WriteLine("Ошибка: введите корректное количество");
        continue;
      }
      
      // Добавление позиции в заказ
      newOrder.AddOrderItem(selected.ProductSku, quantity);
      Console.WriteLine($"Добавлен товар {selected.ProductName} x{quantity}");
    }
    
    // Проверка что заказ не пуст
    if (newOrder.OrderItems.Count == 0) {
      Console.WriteLine("\nЗаказ пуст. Отмена.");
      WaitForUser();
      return;
    }
    
    // Показ деталей заказа
    OrderService.DisplayOrderDetails(newOrder);
    
    // Подтверждение заказа
    Console.Write("Подтвердить заказ? (д/н): ");
    string? confirm = Console.ReadLine();
    if (confirm?.ToLower() == "д") {
      bool result = OrderService.ProcessOrder(_warehouse, newOrder);
      Console.WriteLine(result ? "Заказ выполнен успешно" : "Заказ отклонен: недостаточно товаров");
    } else {
      Console.WriteLine("Заказ отменен");
    }
    
    WaitForUser();
  }

  // ========== РУЧНАЯ ПОСТАВКА ТОВАРОВ ==========
  private void AddSupplyManually() {
    Console.Clear();
    Console.WriteLine("\n=== Поставка товаров ===");
    
    // Получение списка товаров
    var products = _warehouse.GetAllProducts();
    
    // Проверка что склад не пуст
    if (products.Count == 0) {
      Console.WriteLine("На складе нет товаров. Сначала добавьте товары.");
      WaitForUser();
      return;
    }
    
    // Формирование списка доступных товаров
    string productList = "Доступные товары:\n";
    for (int i = 0; i < products.Count; i++) {
      productList += $"  {i + 1}. {products[i].ProductName} ({products[i].ProductSku}) - {products[i].CurrentQuantity} шт.\n";
    }
    Console.Write(productList);
    
    // Выбор товара
    Console.Write("Выберите товар (номер): ");
    if (!int.TryParse(Console.ReadLine(), out int productChoice) || productChoice < 1 || productChoice > products.Count) {
      Console.WriteLine("Неверный выбор");
      WaitForUser();
      return;
    }
    
    Product selected = products[productChoice - 1];
    
    // Ввод количества
    Console.Write("Количество для добавления: ");
    if (!int.TryParse(Console.ReadLine(), out int quantity) || quantity <= 0) {
      Console.WriteLine("Ошибка: введите корректное количество");
      WaitForUser();
      return;
    }
    
    // Выполнение поставки
    SupplyService.AddStockToWarehouse(_warehouse, selected.ProductSku, quantity);
    Console.WriteLine($"Поставка {selected.ProductName} +{quantity} шт. выполнена");
    
    WaitForUser();
  }

  // ========== ДОБАВЛЕНИЕ СЛУЧАЙНЫХ ТЕСТОВЫХ ТОВАРОВ ==========
  private void AddRandomTestProducts() {
    Console.Clear();
    Console.WriteLine("\n=== Добавление случайных тестовых товаров ===");
    
    // Список возможных названий товаров
    List<string> productNames = new List<string> { "Ноутбук", "Смартфон", "Монитор", "Клавиатура", "Мышь", "Наушники", "Планшет", "Принтер" };
    
    // Ввод количества товаров
    Console.Write("Сколько товаров добавить? (1-10): ");
    if (!int.TryParse(Console.ReadLine(), out int count) || count < 1 || count > 10) {
      Console.WriteLine("Введите число от 1 до 10");
      WaitForUser();
      return;
    }
    
    // Генерация и добавление случайных товаров
    string addedList = "";
    for (int i = 0; i < count; i++) {
      string name = productNames[_random.Next(productNames.Count)];
      string sku = $"{name.Substring(0, 2).ToUpper()}-{_random.Next(100, 999)}";
      int stock = _random.Next(5, 50);
      int threshold = _random.Next(3, 15);
      
      Product newProduct = new Product(name, sku, stock, threshold);
      _warehouse.AddNewProduct(newProduct);
      
      addedList += $"  Добавлен: {newProduct}\n";
      Thread.Sleep(100);  // Небольшая задержка для читаемости
    }
    
    Console.WriteLine($"\n{addedList}\nДобавлено {count} товаров");
    WaitForUser();
  }

  // ========== УПРАВЛЕНИЕ ПОДПИСКОЙ СОТРУДНИКОВ ==========
  private void ManageSubscriptions() {
    Console.Clear();
    
    // Формирование меню управления подпиской
    string subscriptionMenu = "\n=== Управление подпиской сотрудников ===\n";
    subscriptionMenu += " 1. Подписать менеджера\n";
    subscriptionMenu += " 2. Отписать менеджера\n";
    subscriptionMenu += " 3. Подписать кладовщика\n";
    subscriptionMenu += " 4. Отписать кладовщика\n";
    subscriptionMenu += " 0. Назад\n";
    subscriptionMenu += "Выберите действие: ";
    Console.Write(subscriptionMenu);
    
    string? choice = Console.ReadLine();
    
    // Создание наблюдателей
    Manager manager = new Manager("Иван Петров", "ivan@company.com");
    Storekeeper storekeeper = new Storekeeper("Сергей Сидоров", "+7-999-123-4567");
    
    // Обработка выбора
    string resultMessage = "";
    switch (choice) {
      case "1": 
        _warehouse.AttachObserver(manager); 
        resultMessage = "Менеджер подписан на уведомления"; 
        break;
      case "2": 
        _warehouse.DetachObserver(manager); 
        resultMessage = "Менеджер отписан от уведомлений"; 
        break;
      case "3": 
        _warehouse.AttachObserver(storekeeper); 
        resultMessage = "Кладовщик подписан на уведомления"; 
        break;
      case "4": 
        _warehouse.DetachObserver(storekeeper); 
        resultMessage = "Кладовщик отписан от уведомлений"; 
        break;
      default: 
        return;
    }
    
    Console.WriteLine($"\n{resultMessage}");
    WaitForUser();
  }

  // ========== ОЖИДАНИЕ НАЖАТИЯ КЛАВИШИ ==========
  private void WaitForUser() {
    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
    Console.ReadKey();
  }
}