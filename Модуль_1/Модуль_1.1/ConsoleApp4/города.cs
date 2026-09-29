using System;
class Program
{
    static void Main()
    {
        string[] cities = { "Москва", "Лондон", "Париж", "Берлин", "Рим" };
        Console.Write("Введите название города: ");
        string city = Console.ReadLine();
        int index = Array.IndexOf(cities, city);
        if (index != -1)
        {
            Console.WriteLine($"Индекс найденного города: {index}");
        }
        else
        {
            Console.WriteLine("Такого города нет в массиве.");
        }
    }
}