using System;
class Program
{
    static void Main()
    {
        Console.Write("Введите температуру: ");
        double temperature = double.Parse(Console.ReadLine());
        Console.Write("Введите единицу измерения (C или F): ");
        string unit = Console.ReadLine().ToUpper();
        if (unit == "C")
        {
            double fahrenheit = temperature * 9 / 5 + 32;
            Console.WriteLine($"Температура в Фаренгейтах: {fahrenheit}");
        }
        else if (unit == "F")
        {
            double celsius = (temperature - 32) * 5 / 9;
            Console.WriteLine($"Температура в Цельсиях: {celsius}");
        }
        else
        {
            Console.WriteLine("используйте C или F");
        }
    }
}