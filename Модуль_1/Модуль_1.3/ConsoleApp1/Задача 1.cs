using System;
class Program
{
    // Функция нахождения НОД
    static int NOD(int a, int b)
    {
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
    static void Main()
    {
        Console.Write("Введите числитель: ");
        int a = int.Parse(Console.ReadLine());
        Console.Write("Введите знаменатель: ");
        int b = int.Parse(Console.ReadLine());
        int nod = NOD(a, b);
        a = a / nod;
        b = b / nod;
        Console.WriteLine("Сокращённая дробь: " + a + "/" + b);
    }
}