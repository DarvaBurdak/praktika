using System; 
// Интерфейс IFigure описывает общие действия для всех геометрических фигур.
// Любой класс, который реализует этот интерфейс, должен иметь методы Area() и Perimeter().
interface IFigure
{
    double Area();       // Метод для вычисления площади фигуры.
    double Perimeter();  // Метод для вычисления периметра фигуры.
}
// Класс Circle реализует интерфейс IFigure.
class Circle : IFigure
{
    private double radius; // Закрытое поле для хранения радиуса круга.
    // Конструктор вызывается при создании нового объекта круга.
    public Circle(double radius)
    {
        this.radius = radius; // Сохраняем переданный радиус в поле объекта.
    }
    // Метод вычисляет площадь круга 
    public double Area()
    {
        return Math.PI * radius * radius;
    }
    // Метод вычисляет длину окружности по формуле 
    public double Perimeter()
    {
        return 2 * Math.PI * radius; 
    }
}
// Класс Rectangle описывает прямоугольник и реализует интерфейс IFigure.
class Rectangle : IFigure
{
    private double width;  // Поле для хранения ширины прямоугольника.
    private double height; // Поле для хранения высоты прямоугольника.
    // Конструктор прямоугольника 
    public Rectangle(double width, double height)
    {
        this.width = width;   // Сохраняем ширину в поле объекта.
        this.height = height; // Сохраняем высоту в поле объекта.
    }
    // Метод вычисляет площадь прямоугольника 
    public double Area()
    {
        return width * height; 
    }
    // Метод вычисляет периметр прямоугольника 
    public double Perimeter()
    {
        return 2 * (width + height); 
    }
}
// Класс Triangle описывает треугольник и реализует интерфейс IFigure.
class Triangle : IFigure
{
    private double a; // Первая сторона треугольника.
    private double b; // Вторая сторона треугольника.
    private double c; // Третья сторона треугольника.
    // Конструктор получает длины трёх сторон треугольника.
    public Triangle(double a, double b, double c)
    {
        this.a = a; // Сохраняем длину первой стороны.
        this.b = b; // Сохраняем длину второй стороны.
        this.c = c; // Сохраняем длину третьей стороны.
    }
    // Метод вычисляет площадь треугольника 
    public double Area()
    {
        // Находим полупериметр
        double p = (a + b + c) / 2;
        // Math.Sqrt() извлекает квадратный корень из выражения.
        return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
    }
    // Метод вычисляет периметр треугольника.
    public double Perimeter()
    {
        return a + b + c; 
    }
}
class Program
{
    static void Main()
    {
        // Создаём объект круга с радиусом 5.
        IFigure circle = new Circle(5);
        // Создаём объект прямоугольника с шириной 4 и высотой 6.
        IFigure rectangle = new Rectangle(4, 6);
        // Создаём объект треугольника со сторонами 3, 4 и 5.
        IFigure triangle = new Triangle(3, 4, 5);
        Console.WriteLine("Круг:");
        // Формат F2 означает, что результат отображается с двумя знаками после запятой.
        Console.WriteLine($"Площадь: {circle.Area():F2}");
        // Вызываем метод Perimeter() и выводим длину окружности.
        Console.WriteLine($"Периметр: {circle.Perimeter():F2}");
        Console.WriteLine("\nПрямоугольник:");
        // Вычисляем и выводим площадь прямоугольника.
        Console.WriteLine($"Площадь: {rectangle.Area():F2}");
        // Вычисляем и выводим периметр прямоугольника.
        Console.WriteLine($"Периметр: {rectangle.Perimeter():F2}");
        Console.WriteLine("\nТреугольник:");
        // Вычисляем и выводим площадь треугольника.
        Console.WriteLine($"Площадь: {triangle.Area():F2}");
        // Вычисляем и выводим периметр треугольника.
        Console.WriteLine($"Периметр: {triangle.Perimeter():F2}");
    }
}