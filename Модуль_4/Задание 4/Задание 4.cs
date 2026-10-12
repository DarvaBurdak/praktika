using System;
// Создаём интерфейс "Книга".
// Интерфейс определяет два действия: проверить, доступна ли книга; выдать книгу.
interface IBook
{
    // Возвращает true, если книга доступна.
    // Возвращает false, если книга уже выдана.
    bool IsAvailable();
    // Метод выдачи книги.
    void GiveBook();
}
// Создаём класс "Book".
class Book : IBook
{
    // Название книги.
    private string title;
    // Состояние книги.
    // true  = книга доступна, false = книга уже выдана.
    private bool available;
    // Конструктор книги.
    public Book(string title)
    {
        // Сохраняем название книги.
        this.title = title;
        // Изначально считаем, что книга находится в библиотеке.
        available = true;
    }
    // Проверяем, доступна ли книга.
    public bool IsAvailable()
    {
        // Возвращаем значение переменной available.
        return available;
    }
    // Метод выдачи книги.
    public void GiveBook()
    {
        // Проверяем, доступна ли книга.
        if (available)
        {
            // Если книга доступна меняем её состояние на "выдана".
            available = false;
            // Сообщаем пользователю, что книга была выдана.
            Console.WriteLine(
                $"Книга \"{title}\" выдана."
            );
        }
        else
        {
            // Если available == false, значит книга уже выдана.
            Console.WriteLine(
                $"Книга \"{title}\" уже выдана."
            );
        }
    }
}
class Program
{
    static void Main()
    {
        // Создаём книгу.
        IBook book = new Book("Война и мир");
        // Проверяем начальное состояние книги.
        // Здесь должно быть true, потому что книгу ещё никто не выдавал.
        Console.WriteLine(
            $"Книга доступна: {(book.IsAvailable() ? "Да" : "Нет")}"
        );
        // Вызываем метод выдачи книги.
        book.GiveBook();
        // Проверяем состояние после выдачи.
        // Теперь должно быть false.
        Console.WriteLine(
            $"Книга доступна: {(book.IsAvailable() ? "Да" : "Нет")}"
        );
        // Пытаемся выдать ту же книгу ещё раз.
        book.GiveBook();
    }
}