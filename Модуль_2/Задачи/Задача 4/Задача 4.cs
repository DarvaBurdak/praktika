using System;
interface IDrawable // Создаём интерфейс IDrawable (набор требований для класса)
{
    // Каждый класс, использующий этот интерфейс, должен иметь метод Draw
    void Draw();
}
class Circle : IDrawable // Создаём класс Circle, который реализует интерфейс IDrawable
{
    public void Draw() // Реализуем обязательный метод Draw
    {
        // Выводим информацию о рисовании круга
        Console.WriteLine("Рисуется круг");
    }
}
class Rectangle : IDrawable // Создаём класс Rectangle, который реализует IDrawable
{
    public void Draw()
    {
        // Выводим информацию о рисовании прямоугольника
        Console.WriteLine("Рисуется прямоугольник");
    }
}
class Triangle : IDrawable // Создаём класс Triangle, который реализует IDrawable
{
    public void Draw()
    {
        // Выводим информацию о рисовании треугольника
        Console.WriteLine("Рисуется треугольник");
    }
}
class Program
{
    static void Main()
    {
        // Создаём массив объектов типа IDrawable, в него можно помещать любые объекты, которые реализуют интерфейс IDrawable
        IDrawable[] shapes =
        {
            new Circle(),
            new Rectangle(),
            new Triangle()
        };
        // Перебираем все объекты массива
        for (int i = 0; i < shapes.Length; i++)
        {
            // Вызываем метод Draw у текущего объекта
            shapes[i].Draw();
        }
    }
}
