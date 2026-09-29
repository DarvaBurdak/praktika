using System;
class Program
{
    static void Main()
    {
        Console.Write("Введите размер матрицы: ");
        int n = int.Parse(Console.ReadLine());
        int[,] matrix = new int[n, n];
        Random random = new Random();
        // Заполняем матрицу
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                matrix[i, j] = random.Next(-50, 51);
            }
        }
        Console.WriteLine("Исходная матрица:");
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                Console.Write(matrix[i, j] + "\t");
            }
            Console.WriteLine();
        }
        // Сортируем строки по сумме
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = i + 1; j < n; j++)
            {
                int sum1 = 0;
                int sum2 = 0;
                // Считаем сумму первой строки
                for (int k = 0; k < n; k++)
                {
                    sum1 += matrix[i, k];
                }
                // Считаем сумму второй строки
                for (int k = 0; k < n; k++)
                {
                    sum2 += matrix[j, k];
                }
                // Если первая сумма больше второй, меняем строки местами
                if (sum1 > sum2)
                {
                    for (int k = 0; k < n; k++)
                    {
                        int temp = matrix[i, k];
                        matrix[i, k] = matrix[j, k];
                        matrix[j, k] = temp;
                    }
                }
            }
        }
        Console.WriteLine("Матрица после сортировки:");
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                Console.Write(matrix[i, j] + "\t");
            }
            Console.WriteLine();
        }
    }
}
