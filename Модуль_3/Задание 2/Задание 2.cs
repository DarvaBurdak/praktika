using System;
// Класс, который отвечает за уведомления.
class Notification
{
    // Событие для отправки сообщений.
    // Action означает, что обработчик события не получает никаких параметров и ничего не возвращает.(делегат)
    public event Action MessageSent;
    // Событие для входящего звонка.
    public event Action CallReceived;
    // Событие для получения электронной почты.
    public event Action EmailReceived;
    // Метод отправки сообщения.
    public void SendMessage()
    {
        // Выводим информацию о том, что сообщение отправлено.
        Console.WriteLine("Отправлено сообщение.");
        // Вызываем событие MessageSent.
        // Если на событие кто-то подписан, его обработчик будет выполнен.
        MessageSent?.Invoke();
    }
    // Метод получения звонка.
    public void ReceiveCall()
    {
        // Сообщаем пользователю о входящем звонке.
        Console.WriteLine("Поступил звонок.");
        // Вызываем событие CallReceived.
        CallReceived?.Invoke();
    }
    // Метод получения электронной почты.
    public void ReceiveEmail()
    {
        // Сообщаем о новом письме.
        Console.WriteLine("Получено электронное письмо.");
        // Вызываем событие EmailReceived.
        EmailReceived?.Invoke();
    }
}
class Program
{
    // Этот метод будет обработчиком события сообщения.
    static void MessageHandler()
    {
        // Выполняется, когда произошло событие MessageSent.
        Console.WriteLine("Обработчик: сообщение получено.");
    }
    // Этот метод будет обработчиком события звонка.
    static void CallHandler()
    {
        // Выполняется при событии CallReceived.
        Console.WriteLine("Обработчик: звонок получен.");
    }
    // Этот метод будет обработчиком события электронной почты.
    static void EmailHandler()
    {
        // Выполняется при событии EmailReceived.
        Console.WriteLine("Обработчик: письмо получено.");
    }
    static void Main()
    {
        // Создаём объект класса Notification.
        Notification notification = new Notification();
        // Подписываем MessageHandler на событие MessageSent.    
        notification.MessageSent += MessageHandler;
        // Подписываем CallHandler на событие CallReceived.
        notification.CallReceived += CallHandler;
        // Подписываем EmailHandler на событие EmailReceived.
        notification.EmailReceived += EmailHandler;
        // Отправляем сообщение.
        // Внутри метода будет вызвано событие MessageSent.
        notification.SendMessage();
        // Получаем звонок.
        // Будет вызвано событие CallReceived.
        notification.ReceiveCall();
        // Получаем электронное письмо.
        // Будет вызвано событие EmailReceived.
        notification.ReceiveEmail();
    }
}
