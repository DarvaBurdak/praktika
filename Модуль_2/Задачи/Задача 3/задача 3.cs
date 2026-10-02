using System;
class Author
{
    public string Name;
    public int BirthYear;
    // Конструктор класса Author
    public Author(string name, int birthYear)
    {
        Name = name;// Записываем имя автора
        BirthYear = birthYear;// Записываем год рождения
    }
}
class Book
{
    public string Title;
    public int Year;
    // Объект класса Author, то есть книга будет хранить информацию о своём авторе
    public Author Author;
    // Конструктор класса Book
    public Book(string title, int year, Author author)
    {
        Title = title;// Записываем название книги
        Year = year;// Записываем год выпуска
        Author = author;// Записываем объект автора
    }
    // Метод для вывода информации о книге
    public void ShowInfo()
    {

        Console.WriteLine("Название: " + Title);
        Console.WriteLine("Год выпуска: " + Year);
        Console.WriteLine("Автор: " + Author.Name);
        Console.WriteLine("Год рождения автора: " + Author.BirthYear);
    }
}
class Program
{
    static void Main()
    {
        Author author1 = new Author("Лев Толстой", 1828);
        Author author2 = new Author("Александр Пушкин", 1799);
        // Создаём первую книгу и связываем её с первым автором
        Book book1 = new Book("Война и мир", 1869, author1);
        // Создаём вторую книгу и связываем её со вторым автором
        Book book2 = new Book("Евгений Онегин", 1833, author2);
        // Выводим информацию о первой книге
        book1.ShowInfo();
        Console.WriteLine();
        // Выводим информацию о второй книге
        book2.ShowInfo();
    }
}