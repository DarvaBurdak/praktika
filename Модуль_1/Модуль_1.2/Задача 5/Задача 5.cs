using System;
class Program
{
    static void Main()
    {
        Console.Write("Введите K: ");
        int k = int.Parse(Console.ReadLine());
        char[] array = new char[k];
        char[] consonants = new char[k];
        string alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        string glasnye = "аеёиоуыэюя";
        Random random = new Random();
        int count = 0;
        // Заполняем первый массив
        for (int i = 0; i < k; i++)
        {
            array[i] = alphabet[random.Next(alphabet.Length)];
        }
        // Находим согласные
        for (int i = 0; i < k; i++)
        {
            if (!glasnye.Contains(array[i]) &&
                array[i] != 'ъ' && array[i] != 'ь')
            {
                consonants[count] = array[i];
                count++;
            }
        }
        Console.WriteLine("Первый массив:");
        for (int i = 0; i < k; i++)
        {
            Console.Write(array[i] + " ");
        }
        Console.WriteLine();
        Console.WriteLine("Согласные:");
        for (int i = 0; i < count; i++)
        {
            Console.Write(consonants[i] + " ");
        }
    }
}
