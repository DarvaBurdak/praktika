using System;
class Employee// Создаём класс Employee (Сотрудник)
{
    private string name;
    private int age;
    private string position;// Поле для хранения должности
    private double salary;// Поле для хранения месячной зарплаты
    // Конструктор без параметров
    public Employee()
    {
        // Задаём значения по умолчанию
        name = "Неизвестно";
        age = 0;
        position = "Не указана";
        salary = 0;
    }
    // Конструктор с параметрами
    public Employee(string name, int age, string position, double salary)
    {
        // Записываем переданные значения в поля
        this.name = name;
        this.age = age;
        this.position = position;
        this.salary = salary;
    }
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
    // Метод для установки должности
    public void SetPosition(string position)
    {
        this.position = position;
    }
    // Метод для получения должности
    public string GetPosition()
    {
        return position;
    }
    // Метод для установки зарплаты
    public void SetSalary(double salary)
    {
        this.salary = salary;
    }
    // Метод для получения зарплаты
    public double GetSalary()
    {
        return salary;
    }
    // Метод для расчёта годового дохода
    public double GetYearSalary()
    {
        return salary * 12; // Умножаем месячную зарплату на 12 месяцев
    }
    // Метод для вывода информации о сотруднике
    public void ShowInfo()
    {
        Console.WriteLine("Имя: " + name);
        Console.WriteLine("Возраст: " + age);
        Console.WriteLine("Должность: " + position);
        Console.WriteLine("Месячная зарплата: " + salary);
        Console.WriteLine("Годовой доход: " + GetYearSalary());
    }
}
class Program
{
    static void Main()
    {
        // Создаём первого сотрудника с помощью конструктора с параметрами
        Employee employee1 = new Employee(
            "Иван",
            25,
            "Программист",
            1900
        );
        // Создаём второго сотрудника с помощью конструктора без параметров
        Employee employee2 = new Employee();
        // Используем методы установки значений
        employee2.SetName("Анна");
        employee2.SetAge(30);
        employee2.SetPosition("Менеджер");
        employee2.SetSalary(2800);
        // Выводим информацию о первом сотруднике
        Console.WriteLine("Первый сотрудник:");
        employee1.ShowInfo();
        Console.WriteLine();
        // Выводим информацию о втором сотруднике
        Console.WriteLine("Второй сотрудник:");
        employee2.ShowInfo();
    }
}