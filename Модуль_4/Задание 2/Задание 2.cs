using System;
// Создаём интерфейс "Товар".
// Он определяет, какие действия должны поддерживать все товары магазина.
interface IProduct
{
    // Метод возвращает стоимость товара.
    double GetCost();
    // Метод возвращает количество товара на складе.
    int GetStock();
}
// После IProduct указываем, что телефон реализует интерфейс товара.
class Phone : IProduct
{
    // Цена телефона.
    private double price;
    // Количество телефонов на складе.
    private int stock;
    // Конструктор телефона.
    // При создании объекта передаём цену и количество на складе.
    public Phone(double price, int stock)
    {
        this.price = price;
        this.stock = stock;
    }
    // Реализуем метод интерфейса IProduct.
    public double GetCost()
    {
        return price;
    }
    // Реализуем второй метод интерфейса.
    public int GetStock()
    {
        return stock;
    }
}
// Создаём класс "Ноутбук".
class Laptop : IProduct
{
    // Цена ноутбука.
    private double price;
    // Количество ноутбуков на складе.
    private int stock;
    // Конструктор ноутбука.
    public Laptop(double price, int stock)
    {
        this.price = price;
        this.stock = stock;
    }
    // Возвращаем стоимость ноутбука.
    public double GetCost()
    {
        return price;
    }
    // Возвращаем количество ноутбуков на складе.
    public int GetStock()
    {
        return stock;
    }
}
class Program
{
    static void Main()
    {
        // Создаём объект телефона.
        IProduct phone = new Phone(1200, 10);
        // Создаём объект ноутбука.
        IProduct laptop = new Laptop(5400, 5);
        // Выводим информацию о телефоне.
        Console.WriteLine("ТЕЛЕФОН");
        // Получаем стоимость через метод интерфейса.
        Console.WriteLine(
            $"Стоимость: {phone.GetCost()} руб."
        );
        // Получаем количество товара на складе.
        Console.WriteLine(
            $"На складе: {phone.GetStock()} шт."
        );
        // Выводим информацию о ноутбуке.
        Console.WriteLine("\nНОУТБУК");
        // Получаем стоимость ноутбука.
        Console.WriteLine(
            $"Стоимость: {laptop.GetCost()} руб."
        );
        // Получаем количество ноутбуков.
        Console.WriteLine(
            $"На складе: {laptop.GetStock()} шт."
        );
    }
}