using System;
class Person
{
    private string name;// Закрытое поле для хранения имени
    private int age;// Закрытое поле для хранения возраста
    private string address;// Закрытое поле для хранения адреса
    // Метод для установки имени
    public void SetName(string name)
    {
        this.name = name;
    }
    // Метод для получения имени
    public string GetName()
    {
        return name;
    }
    // Метод для установки возраста
    public void SetAge(int age)
    {
        this.age = age;
    }
    // Метод для получения возраста
    public int GetAge()
    {
        return age;
    }
    // Метод для установки адреса
    public void SetAddress(string address)
    {
        this.address = address;
    }
    // Метод для получения адреса
    public string GetAddress()
    {
        return address;
    }
}
class Program
{
    static void Main()
    {
        Person person1 = new Person();// Создаём первый объект класса Person
        Person person2 = new Person();// Создаём второй объект класса Person

        // Данные первого человека
        person1.SetName("Иван");
        person1.SetAge(20);
        person1.SetAddress("Витебск");

        // данные второго человека
        person2.SetName("Анна");
        person2.SetAge(22);
        person2.SetAddress("Минск");

        // Вывод информации о первом человеке
        Console.WriteLine("Первый человек:");
        Console.WriteLine("Имя: " + person1.GetName());
        Console.WriteLine("Возраст: " + person1.GetAge());
        Console.WriteLine("Адрес: " + person1.GetAddress());

        Console.WriteLine();

        // Вывод информации о втором человеке
        Console.WriteLine("Второй человек:");
        Console.WriteLine("Имя: " + person2.GetName());
        Console.WriteLine("Возраст: " + person2.GetAge());
        Console.WriteLine("Адрес: " + person2.GetAddress());
    }
}