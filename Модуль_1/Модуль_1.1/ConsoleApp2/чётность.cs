using System;
class Program
{
    static void Main()
    {
        Console.Write("Введите число: ");
        int number = Convert.ToInt32(Console.ReadLine());
        // Вызываем метод и выводим результат
        bool result = CheckNumber(number);
        if (result == true)
            Console.WriteLine("Четное");
        else
            Console.WriteLine("Не четное");

    }
    public static bool CheckNumber(int number)
    {
        return number % 2 == 0;
    }
}