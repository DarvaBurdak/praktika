using System;
class Program
{
    static void Main()
    {
        Console.Write("Введите строку: ");
        string text = Console.ReadLine();
        string reversed = "";
        for (int i = text.Length - 1; i >= 0; i--)
        {
            reversed += text[i];
        }
        if (text == reversed)
        {
            Console.WriteLine("Строка является палиндромом.");
        }
        else
        {
            Console.WriteLine("Строка не является палиндромом.");
        }
    }
}