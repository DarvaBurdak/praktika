using System;
class Program
{
    static void Main()
    {
        Console.Write("Введите число: ");
        int n = int.Parse(Console.ReadLine());
        Random random = new Random();
        int[] array = new int[n];
        int sum = 0;
        int count = 0;
        while (sum < n)
        {
            int number = random.Next(1, 10);
            if (sum + number <= n)
            {
                array[count] = number;
                sum += number;
                count++;
            }
            else
            {
                break;
            }
        }
        Console.WriteLine("Массив:");
        for (int i = 0; i < count; i++)
        {
            Console.Write(array[i] + " ");
        }
        Console.WriteLine();
        Console.WriteLine("Сумма: " + sum);
    }
}