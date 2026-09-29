using System;
class Program
{
    static void Main()
    {
        Console.Write("Введите размер массива N: ");
        int n = int.Parse(Console.ReadLine());
        double[] a = new double[n];
        // Ввод элементов
        for (int i = 0; i < n; i++)
        {
            Console.Write($"Введите элемент {i}: ");
            a[i] = double.Parse(Console.ReadLine());
        }
        // Ищем максимальный по модулю элемент
        double max = Math.Abs(a[0]);
        for (int i = 1; i < n; i++)
        {
            if (Math.Abs(a[i]) > max)
            {
                max = Math.Abs(a[i]);
            }
        }
        // Делим все элементы на максимальный по модулю
        for (int i = 0; i < n; i++)
        {
            a[i] = a[i] / max;
        }
        Console.WriteLine("Изменённый массив:");
        for (int i = 0; i < n; i++)
        {
            Console.Write($"{a[i]:F2} ");
        }
    }
}