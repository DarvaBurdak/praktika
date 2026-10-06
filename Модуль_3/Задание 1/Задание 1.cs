using System;
//abstract означает, что напрямую создать объект Shape нельзя. Определяет общую структуру и базовое поведение для целого семейства связанных типов
abstract class Shape
{
    // Абстрактный метод вычисления площади.
    public abstract double Area();
}
class Circle : Shape
{
    private double radius;
    // Конструктор класса Circle.
    // Получает радиус и сохраняет его в поле.
    public Circle(double radius)
    {
        this.radius = radius;
    }
    // Реализуем метод Area
    public override double Area()
    {
        // Формула площади круга
        return Math.PI * radius * radius;
    }
}
class Rectangle : Shape
{
    private double width;
    private double height;
    // Конструктор прямоугольника.
    public Rectangle(double width, double height)
    {
        // Сохраняем полученную ширину.
        this.width = width;
        // Сохраняем полученную высоту.
        this.height = height;
    }
    // Реализация метода вычисления площади.
    public override double Area()
    {
        // Площадь прямоугольника
        return width * height;
    }
}
class Triangle : Shape
{
    private double baseSide;
    private double height;
    // Конструктор треугольника.
    public Triangle(double baseSide, double height)
    {
        this.baseSide = baseSide;
        this.height = height;
    }
    // Реализация метода Area
    public override double Area()
    {
        // Площадь треугольника
        return 0.5 * baseSide * height;
    }
}
// Создаём делегат.
// Делегат AreaDelegate может хранить ссылку
// на любой метод, который: не принимает параметры; возвращает число типа double.
delegate double AreaDelegate();
class Program
{
    static void Main()
    {
        // Создаём объект круга с радиусом 5.
        Circle circle = new Circle(5);
        // Создаём прямоугольник со сторонами 4 и 6.
        Rectangle rectangle = new Rectangle(4, 6);
        // Создаём треугольник с основанием 4 и высотой 5.
        Triangle triangle = new Triangle(4, 5);
        // Создаём делегат и передаём ему метод Area() круга.
        AreaDelegate circleArea = circle.Area;
        // Делегат хранит ссылку на Area() прямоугольника.
        AreaDelegate rectangleArea = rectangle.Area;
        // Делегат хранит ссылку на Area() треугольника.
        AreaDelegate triangleArea = triangle.Area;
        // Вызываем метод площади круга через делегат.
        Console.WriteLine("Площадь круга: " + circleArea());
        // Вызываем метод площади прямоугольника через делегат.
        Console.WriteLine("Площадь прямоугольника: " + rectangleArea());
        // Вызываем метод площади треугольника через делегат.
        Console.WriteLine("Площадь треугольника: " + triangleArea());
    }
}