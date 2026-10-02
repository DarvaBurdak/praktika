using System;
class MyClass
{
    private int number;// Первая переменная класса
    private string text;// Вторая переменная класса
    // Конструктор с параметрами
    public MyClass(int number, string text)
    {
        // Записываем переданные значения в поля
        this.number = number;
        this.text = text;
        Console.WriteLine("Создан объект с параметрами");
    }
    // Конструктор без параметров
    public MyClass()
    {
        // Задаём значения по умолчанию
        number = 0;
        text = "По умолчанию";
        Console.WriteLine("Создан объект со значениями по умолчанию");
    }
    // Метод для вывода значений
    public void ShowInfo()
    {
        Console.WriteLine("Число: " + number);
        Console.WriteLine("Текст: " + text);
    }
    // Деструктор
    ~MyClass()
    {
        //Сообщение выводится при удалении объекта 
        Console.WriteLine("Объект удалён.");
    }
}
class Program
{
    static void Main()
    {
        MyClass object1 = new MyClass(10, "Привет");// Создаём объект с параметрами
        // Выводим данные
        object1.ShowInfo();
        Console.WriteLine();
        MyClass object2 = new MyClass();// Создаём объект без параметров
        // Выводим данные
        object2.ShowInfo();
        Console.WriteLine();
        // Убираем ссылки на объекты
        object1 = null;
        object2 = null;
        // Запускаем удаление объектов
        GC.Collect();
        // Завершение работы деструкторов
        GC.WaitForPendingFinalizers();
        Console.WriteLine("Ненужные объекты удалены");
        Console.ReadLine();
    }
}