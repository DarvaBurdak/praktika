using System;
class Program
{
    static void Main()
    {
        Random random = new Random();
        int number = random.Next(1, 11);
        Console.Write("Угадайте число от 1 до 10: ");
        int guess = int.Parse(Console.ReadLine());
        if (guess == number)
        {
            Console.WriteLine("Поздравляем! Вы угадали число!");
        }
        else
        {
            Console.WriteLine($"Вы не угадали. Было загадано число {number}.");
        }
    }
}