
namespace WpfApp1
{
    // Класс описывает одну книгу в библиотеке.
    public class Book
    {
        // Уникальный номер книги в базе данных.
        public int Id { get; set; }

        // Название книги.
        public string Title { get; set; }

        // Автор книги.
        public string Author { get; set; }

        // Год издания.
        public int Year { get; set; }

        // Жанр книги.
        public string Genre { get; set; }

        // Количество экземпляров, доступных для аренды.
        public int AvailableCopies { get; set; }

        // Общее количество экземпляров книги.
        public int TotalCopies { get; set; }
    }
}
