using System;
// Создаём делегат для работы с задачами.
// Он может хранить ссылку на метод, который получает строку и ничего не возвращает.
delegate void TaskDelegate(string task);
class Program
{
    // Метод отправки уведомления.
    // Получает название задачи в виде строки.
    static void SendNotification(string task)
    {
        // Выводим уведомление пользователю.
        Console.WriteLine("Уведомление: новая задача — " + task);
    }
    // Метод записи задачи в журнал.
    static void WriteToLog(string task)
    {
        // Выводим сообщение о записи в журнал.
        Console.WriteLine("В журнал записана задача: " + task);
    }
    static void Main()
    {
        // Просим пользователя ввести задачу.
        Console.Write("Введите задачу: ");
        // Считываем введённую пользователем строку.
        string task = Console.ReadLine();
        // Предлагаем пользователю выбрать действие.
        Console.WriteLine();
        Console.WriteLine("Выберите действие:");
        // Первый вариант — отправить уведомление.
        Console.WriteLine("1 - Отправить уведомление");
        // Второй вариант — записать задачу в журнал.
        Console.WriteLine("2 - Записать в журнал");
        // Считываем выбор пользователя.
        int choice = Convert.ToInt32(Console.ReadLine());
        // Создаём переменную типа нашего делегата.
        TaskDelegate action;
        // Проверяем выбор пользователя.
        if (choice == 1)
        {
            // Если пользователь выбрал 1, делегату передаём метод SendNotification.
            action = SendNotification;
        }
        else
        {
            // Если пользователь выбрал 2, делегату передаём метод WriteToLog.
            action = WriteToLog;
        }
        // Вызываем выбранный метод через делегат.
        // В качестве параметра передаём нашу задачу.
        action(task);
    }
}