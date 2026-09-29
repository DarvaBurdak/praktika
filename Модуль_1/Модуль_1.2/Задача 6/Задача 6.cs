using System;
class Program
{
    static void Main()
    {
        double[] array = new double[10];
        int[] index = new int[10];
        Random random = new Random();
        // Заполняем массив
        for (int i = 0; i < 10; i++)
        {
            array[i] = random.Next(-10, 10);
            index[i] = i;
        }
        // Сортируем индексы
        for (int i = 0; i < 9; i++)
        {
            for (int j = i + 1; j < 10; j++)
            {
                if (array[index[i]] > array[index[j]])
                {
                    int temp = index[i];
                    index[i] = index[j];
                    index[j] = temp;
                }
            }
        }
        Console.WriteLine("Массив:");
        for (int i = 0; i < 10; i++)
        {
            Console.Write(array[i] + " ");
        }
        Console.WriteLine();
        Console.WriteLine("Индексы:");
        for (int i = 0; i < 10; i++)
        {
            Console.Write(index[i] + " ");
        }
    }
}
