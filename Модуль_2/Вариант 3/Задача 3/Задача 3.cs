using System;
using System.Collections.Generic;
class Author
{
    public string FirstName; // Имя автора
    public string LastName; // Фамилия автора
    public int BirthYear;// Год рождения
    // Конструктор автора
    public Author(string firstName, string lastName, int birthYear)
    {
        FirstName = firstName;// Сохраняем имя
        LastName = lastName;// Сохраняем фамилию
        BirthYear = birthYear;// Сохраняем год рождения
    }
    // Метод для получения полного имени
    public string GetFullName()
    {
        return FirstName + " " + LastName;
    }
}
class Book
{
    public string Title; // Название книги
    public int Year;// Год издания
    public Author Author;// Автор книги, используется объект класса Author
    // Конструктор книги
    public Book(string title, int year, Author author)
    {
        Title = title;// Сохраняем название
        Year = year;// Сохраняем год издания
        Author = author;// Сохраняем автора
    }
    // Метод для вывода информации о книге
    public void ShowInfo()
    {
        Console.WriteLine(
            Title + " - " +
            Author.GetFullName() + ", " +
            Year
        );
    }
}
class Library
{
    private List<Book> books = new List<Book>();// Список книг библиотеки
    public void AddBook(Book book) // Метод добавления книги
    {
        // Добавляем книгу в список
        books.Add(book);
        Console.WriteLine("Книга добавлена: " + book.Title);
    }
    // Метод удаления книги
    public void RemoveBook(Book book)
    {
        // Удаляем книгу из списка
        books.Remove(book);
        Console.WriteLine("Книга удалена: " + book.Title);
    }
    // Поиск книг по автору
    public void FindByAuthor(string authorName)
    {
        Console.WriteLine("Книги автора " + authorName + ":");
        // Перебираем все книги
        for (int i = 0; i < books.Count; i++)
        {
            // Проверяем имя автора текущей книги
            if (books[i].Author.GetFullName() == authorName)
            {
                // Если автор совпал, выводим книгу
                books[i].ShowInfo();
            }
        }
    }
    // Поиск книг по году издания
    public void FindByYear(int year)
    {
        Console.WriteLine("Книги, изданные в " + year + " году:");
        // Перебираем все книги
        for (int i = 0; i < books.Count; i++)
        {
            // Проверяем год текущей книги
            if (books[i].Year == year)
            {
                // Выводим найденную книгу
                books[i].ShowInfo();
            }
        }
    }
    // Метод для вывода всех книг
    public void ShowAllBooks()
    {
        Console.WriteLine("Все книги библиотеки:");
        // Перебираем список книг
        for (int i = 0; i < books.Count; i++)
        {
            // Выводим информацию о текущей книге
            books[i].ShowInfo();
        }
    }
}
class Program
{
    static void Main()
    {
        // Создаём двух авторов
        Author author1 = new Author(
            "Лев",
            "Толстой",
            1828
        );
        Author author2 = new Author(
            "Александр",
            "Пушкин",
            1799
        );
        // Создаём книги и связываем их с авторами
        Book book1 = new Book(
            "Война и мир",
            1869,
            author1
        );
        Book book2 = new Book(
            "Евгений Онегин",
            1833,
            author2
        );
        Book book3 = new Book(
            "Анна Каренина",
            1878,
            author1
        );
        // Создаём объект библиотеки
        Library library = new Library();
        // Добавляем книги в библиотеку
        library.AddBook(book1);
        library.AddBook(book2);
        library.AddBook(book3);
        Console.WriteLine();
        // Выводим все книги
        library.ShowAllBooks();
        Console.WriteLine();
        // Ищем книги определённого автора
        library.FindByAuthor("Лев Толстой");
        Console.WriteLine();
        // Ищем книги определённого года
        library.FindByYear(1833);
        Console.WriteLine();
        // Удаляем одну книгу
        library.RemoveBook(book2);
        Console.WriteLine();
        // Показываем оставшиеся книги
        library.ShowAllBooks();
    }
}