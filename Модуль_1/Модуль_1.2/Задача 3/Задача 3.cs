using System;
class Program
{
    static void Main()
    {
        Console.Write("Введите K: ");
        int k = int.Parse(Console.ReadLine());
        int count = 0;
        int number = 2;
        while (count < k)
        {
            bool simple = true;
            for (int i = 2; i < number; i++)
            {
                if (number % i == 0)
                {
                    simple = false;
                    break;
                }
            }
            if (simple)
            {
                Console.Write(number + " ");
                count++;

                if (count % 10 == 0)
                    Console.WriteLine();
            }
            number++;
        }
    }
}