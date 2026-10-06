using System;
// Создаём делегат для фильтрации.
// Метод, который будет храниться в этом делегате, принимает строку и возвращает true или false.
delegate bool FilterDelegate(string data);
class Program
{
    // Фильтр по ключевому слову.
    static bool FilterByWord(string data)
    {
        // Проверяем, содержит ли строка "C#".
        // Если содержит — возвращается true.
        // Если не содержит — возвращается false.
        return data.Contains("C#");
    }
    // Фильтр по длине строки.
    static bool FilterByLength(string data)
    {
        // Проверяем, содержит ли строка больше 5 символов.
        return data.Length > 5;
    }
    // Метод, который выполняет фильтрацию.
    // Первый параметр — массив данных, второй — выбранный пользователем фильтр.
    static void FilterData(string[] data, FilterDelegate filter)
    {
        // Заголовок результата.
        Console.WriteLine();
        Console.WriteLine("Результат фильтрации:");
        // Перебираем все элементы массива.
        foreach (string item in data)
        {
            // Передаём текущий элемент выбранному фильтру.
            // Если фильтр возвращает true, элемент подходит и его нужно вывести.
            if (filter(item))
            {
                Console.WriteLine(item);
            }
        }
    }
    static void Main()
    {
        // Создаём массив строк, который будем фильтровать.
        string[] data =
        {
            "C#",
            "Программирование",
            "Java",
            "C# WinForms",
            "Код"
        };
        // Показываем пользователю доступные фильтры.
        Console.WriteLine("Выберите фильтр:");
        // Фильтр по слову C#.
        Console.WriteLine("1 - По ключевому слову C#");
        // Фильтр по количеству символов.
        Console.WriteLine("2 - По длине строки");
        // Считываем выбор пользователя.
        int choice = Convert.ToInt32(Console.ReadLine());
        // Создаём переменную делегата.
        FilterDelegate filter;
        // Проверяем выбор пользователя.
        if (choice == 1)
        {
            // Выбран фильтр по ключевому слову.
            filter = FilterByWord;
        }
        else
        {
            // Выбран фильтр по длине.
            filter = FilterByLength;
        }
        // Передаём массив и выбранный фильтр в метод FilterData.
        FilterData(data, filter);
    }
}
