using System;
class Shape // Класс для всех геометрических фигур 
{
    // Метод вычисления площади, virtual позволяет изменять поведение этого метода в дочерних классах (полиморфизм)
    public virtual double Area() // Абстракция — это выделение главных характеристик объекта
    {
        return 0;
    }
    // Метод вычисления периметра
    public virtual double Perimeter()
    {
        return 0;
    }
}
// Класс Circle наследуется от класса Shape (наследование)
class Circle : Shape
{
    // Поле для хранения радиуса круга (double вещественные числа)
    private double radius;
    // Конструктор круга
    public Circle(double radius)
    {
        this.radius = radius;// Сохраняем переданный радиус
    }
    // Переопределяем метод вычисления площади
    public override double Area()
    {
        return Math.PI * radius * radius;// Формула площади круга
    }
    // Переопределяем метод вычисления периметра
    public override double Perimeter()
    {
        return 2 * Math.PI * radius;// Формула длины окружности
    }
}
// Класс Rectangle наследуется от Shape
class Rectangle : Shape
{
    private double width;// Ширина прямоугольника (инкапсуляция — объединение данных и методов внутри одного класса и ограничение прямого доступа к данным.)
    private double height;// Высота прямоугольника
    // Конструктор прямоугольника
    public Rectangle(double width, double height)
    {
        this.width = width;// Сохраняем ширину
        this.height = height;// Сохраняем высоту
    }
    // Переопределяем метод вычисления площади
    public override double Area()
    {
        return width * height;// Площадь прямоугольника
    }
    // Переопределяем метод вычисления периметра
    public override double Perimeter()
    {
        return 2 * (width + height);// Периметр прямоугольника
    }
}
class Program
{
    static void Main()
    {
        Circle circle = new Circle(5);// Создаём объект круга с радиусом 5
        Rectangle rectangle = new Rectangle(4, 6);// Создаём прямоугольник со сторонами 4 и 6

        // Выводим информацию о круге
        Console.WriteLine("Круг:");
        Console.WriteLine("Площадь: " + circle.Area());
        Console.WriteLine("Периметр: " + circle.Perimeter());
        Console.WriteLine();

        // Выводим информацию о прямоугольнике
        Console.WriteLine("Прямоугольник:");
        Console.WriteLine("Площадь: " + rectangle.Area());
        Console.WriteLine("Периметр: " + rectangle.Perimeter());
    }
}
