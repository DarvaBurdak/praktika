using System;
class Program
{
    static void Main()
    {
        int[] a = new int[10];
        // Ввод массива
        Console.WriteLine("Введите 10 элементов:");
        for (int i = 0; i < 10; i++)
        {
            a[i] = int.Parse(Console.ReadLine());
        }
        // Находим максимальный элемент
        int maxIndex = 0;
        for (int i = 1; i < 10; i++)
        {
            if (a[i] > a[maxIndex])
            {
                maxIndex = i;
            }
        }
        Console.Write("Введите новое значение: ");
        int number = int.Parse(Console.ReadLine());
        // Заменяем максимальный элемент
        a[maxIndex] = number;
        Console.WriteLine("Изменённый массив:");
        for (int i = 0; i < 10; i++)
        {
            Console.Write(a[i] + " ");
        }
    }
}
