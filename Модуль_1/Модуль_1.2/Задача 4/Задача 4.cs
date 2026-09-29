using System;
class Program
{
    static void Main()
    {
        Console.Write("Введите k: ");
        int k = int.Parse(Console.ReadLine());
        Console.Write("Введите a: ");
        int a = int.Parse(Console.ReadLine());
        Console.Write("Введите b: ");
        int b = int.Parse(Console.ReadLine());
        int[] array = new int[k];
        Random random = new Random();
        // Заполняем массив
        for (int i = 0; i < k; i++)
        {
            array[i] = random.Next(a, b);
            Console.Write(array[i] + " ");
        }
        int min = 0;
        int max = 0;
        // Ищем минимальный и максимальный элементы
        for (int i = 1; i < k; i++)
        {
            if (array[i] < array[min])
                min = i;
            if (array[i] > array[max])
                max = i;
        }
        Console.WriteLine();
        Console.WriteLine("Индекс минимума: " + min);
        Console.WriteLine("Индекс максимума: " + max);
        // Меняем местами индексы, если максимум находится раньше минимума
        if (min > max)
        {
            int temp = min;
            min = max;
            max = temp;
        }
        Console.WriteLine("Элементы между ними:");
        for (int i = min + 1; i < max; i++)
        {
            Console.Write(array[i] + " ");
        }
    }
}