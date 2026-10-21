
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace WpfApp1
{
    public class LibraryDatabase
    {
        // Путь к файлу базы данных.
        private readonly string databasePath;

        public LibraryDatabase()
        {
            // Создаём отдельную папку для базы библиотеки.
            string folder = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "LibraryApp");

            Directory.CreateDirectory(folder);

            databasePath = Path.Combine(folder, "library.db");

            // Создаём таблицы при первом запуске.
            InitializeDatabase();
        }

        // Создание таблиц базы данных.
        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(
                "Data Source=" + databasePath))
            {
                connection.Open();

                string sql = @"
                    CREATE TABLE IF NOT EXISTS Books (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Author TEXT NOT NULL,
                        Year INTEGER NOT NULL,
                        Genre TEXT,
                        TotalCopies INTEGER NOT NULL,
                        AvailableCopies INTEGER NOT NULL
                    );

                    CREATE TABLE IF NOT EXISTS Rentals (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        BookId INTEGER NOT NULL,
                        RentalDate TEXT NOT NULL,
                        ReturnDate TEXT,
                        IsReturned INTEGER NOT NULL DEFAULT 0,
                        FOREIGN KEY (BookId) REFERENCES Books(Id)
                    );";

                using (var command = new SqliteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        // Получение списка всех книг.
        public List<Book> GetBooks(string search = "")
        {
            var books = new List<Book>();

            using (var connection = new SqliteConnection(
                "Data Source=" + databasePath))
            {
                connection.Open();

                string sql = @"
                    SELECT Id, Title, Author, Year, Genre,
                           TotalCopies, AvailableCopies
                    FROM Books
                    WHERE Title LIKE @search OR Author LIKE @search
                    ORDER BY Title;";

                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue(
                        "@search", "%" + search + "%");

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            books.Add(new Book
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Author = reader.GetString(2),
                                Year = reader.GetInt32(3),
                                Genre = reader.IsDBNull(4)
                                    ? ""
                                    : reader.GetString(4),
                                TotalCopies = reader.GetInt32(5),
                                AvailableCopies = reader.GetInt32(6)
                            });
                        }
                    }
                }
            }

            return books;
        }

        // Добавление книги в каталог.
        public void AddBook(Book book)
        {
            using (var connection = new SqliteConnection(
                "Data Source=" + databasePath))
            {
                connection.Open();

                string sql = @"
                    INSERT INTO Books
                    (Title, Author, Year, Genre, TotalCopies, AvailableCopies)
                    VALUES
                    (@title, @author, @year, @genre, @total, @available);";

                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@title", book.Title);
                    command.Parameters.AddWithValue("@author", book.Author);
                    command.Parameters.AddWithValue("@year", book.Year);
                    command.Parameters.AddWithValue("@genre", book.Genre ?? "");
                    command.Parameters.AddWithValue("@total", book.TotalCopies);
                    command.Parameters.AddWithValue(
                        "@available", book.TotalCopies);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Аренда одного экземпляра книги.
        public bool RentBook(int bookId)
        {
            using (var connection = new SqliteConnection(
                "Data Source=" + databasePath))
            {
                connection.Open();

                // Начинаем транзакцию, чтобы изменение остатков
                // и регистрация аренды выполнялись вместе.
                using (var transaction = connection.BeginTransaction())
                {
                    string updateSql = @"
                        UPDATE Books
                        SET AvailableCopies = AvailableCopies - 1
                        WHERE Id = @id AND AvailableCopies > 0;";

                    using (var command = new SqliteCommand(
                        updateSql, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@id", bookId);

                        if (command.ExecuteNonQuery() == 0)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }

                    string rentalSql = @"
                        INSERT INTO Rentals (BookId, RentalDate, IsReturned)
                        VALUES (@id, @date, 0);";

                    using (var command = new SqliteCommand(
                        rentalSql, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@id", bookId);
                        command.Parameters.AddWithValue(
                            "@date", DateTime.Today.ToString("yyyy-MM-dd"));

                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                    return true;
                }
            }
        }

        // Возврат самого раннего невозвращённого экземпляра книги.
        public bool ReturnBook(int bookId)
        {
            using (var connection = new SqliteConnection(
                "Data Source=" + databasePath))
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    int rentalId;

                    // Находим действующую аренду этой книги.
                    string findSql = @"
                        SELECT Id FROM Rentals
                        WHERE BookId = @id AND IsReturned = 0
                        ORDER BY Id LIMIT 1;";

                    using (var command = new SqliteCommand(
                        findSql, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@id", bookId);

                        object result = command.ExecuteScalar();

                        if (result == null)
                        {
                            transaction.Rollback();
                            return false;
                        }

                        rentalId = Convert.ToInt32(result);
                    }

                    // Отмечаем аренду как возвращённую.
                    string returnSql = @"
                        UPDATE Rentals
                        SET IsReturned = 1, ReturnDate = @date
                        WHERE Id = @rentalId;";

                    using (var command = new SqliteCommand(
                        returnSql, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@rentalId", rentalId);
                        command.Parameters.AddWithValue(
                            "@date", DateTime.Today.ToString("yyyy-MM-dd"));

                        command.ExecuteNonQuery();
                    }

                    // Увеличиваем количество доступных экземпляров.
                    string updateSql = @"
                        UPDATE Books
                        SET AvailableCopies = AvailableCopies + 1
                        WHERE Id = @id AND AvailableCopies < TotalCopies;";

                    using (var command = new SqliteCommand(
                        updateSql, connection, transaction))
                    {
                        command.Parameters.AddWithValue("@id", bookId);

                        if (command.ExecuteNonQuery() == 0)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }

                    transaction.Commit();
                    return true;
                }
            }
        }
    }
}
