using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Microsoft.Data.Sqlite;

namespace WpfApp1
{
    public class FinanceDatabase
    {
        // Путь к файлу базы данных.
        private readonly string databasePath;

        public FinanceDatabase()
        {
            // Создаём отдельную папку для финансового приложения.
            string folder = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "FinanceApp");

            Directory.CreateDirectory(folder);

            databasePath = Path.Combine(folder, "finance.db");

            // Создаём таблицу, если её ещё нет.
            InitializeDatabase();
        }

        // Создание таблицы финансовых операций.
        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(
                "Data Source=" + databasePath))
            {
                connection.Open();

                string sql = @"
                    CREATE TABLE IF NOT EXISTS FinanceRecords (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Type TEXT NOT NULL,
                        Category TEXT NOT NULL,
                        Amount TEXT NOT NULL,
                        Date TEXT NOT NULL,
                        Description TEXT
                    );";

                using (var command = new SqliteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        // Получает все финансовые записи из базы.
        public List<FinanceRecord> GetAllRecords()
        {
            var records = new List<FinanceRecord>();

            using (var connection = new SqliteConnection(
                "Data Source=" + databasePath))
            {
                connection.Open();

                string sql = @"
                    SELECT Id, Type, Category, Amount, Date, Description
                    FROM FinanceRecords
                    ORDER BY Date DESC, Id DESC;";

                using (var command = new SqliteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Преобразуем строку базы в объект C#.
                        records.Add(new FinanceRecord
                        {
                            Id = reader.GetInt32(0),
                            Type = reader.GetString(1),
                            Category = reader.GetString(2),

                            // Используем инвариантный формат,
                            // чтобы сумма не зависела от региональных настроек.
                            Amount = decimal.Parse(
                                reader.GetString(3),
                                CultureInfo.InvariantCulture),

                            Date = DateTime.ParseExact(
                                reader.GetString(4),
                                "yyyy-MM-dd",
                                CultureInfo.InvariantCulture),

                            Description = reader.IsDBNull(5)
                                ? ""
                                : reader.GetString(5)
                        });
                    }
                }
            }

            return records;
        }

        // Добавляет новую финансовую операцию.
        public void AddRecord(FinanceRecord record)
        {
            using (var connection = new SqliteConnection(
                "Data Source=" + databasePath))
            {
                connection.Open();

                string sql = @"
                    INSERT INTO FinanceRecords
                    (Type, Category, Amount, Date, Description)
                    VALUES
                    (@type, @category, @amount, @date, @description);";

                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@type", record.Type);
                    command.Parameters.AddWithValue(
                        "@category", record.Category);

                    // Сохраняем сумму в независимом от языка формате.
                    command.Parameters.AddWithValue(
                        "@amount",
                        record.Amount.ToString(CultureInfo.InvariantCulture));

                    command.Parameters.AddWithValue(
                        "@date",
                        record.Date.ToString("yyyy-MM-dd",
                            CultureInfo.InvariantCulture));

                    command.Parameters.AddWithValue(
                        "@description", record.Description ?? "");

                    command.ExecuteNonQuery();
                }
            }
        }

        // Изменяет существующую финансовую операцию.
        public void UpdateRecord(FinanceRecord record)
        {
            using (var connection = new SqliteConnection(
                "Data Source=" + databasePath))
            {
                connection.Open();

                string sql = @"
                    UPDATE FinanceRecords
                    SET Type = @type,
                        Category = @category,
                        Amount = @amount,
                        Date = @date,
                        Description = @description
                    WHERE Id = @id;";

                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", record.Id);
                    command.Parameters.AddWithValue("@type", record.Type);
                    command.Parameters.AddWithValue(
                        "@category", record.Category);
                    command.Parameters.AddWithValue(
                        "@amount",
                        record.Amount.ToString(CultureInfo.InvariantCulture));
                    command.Parameters.AddWithValue(
                        "@date",
                        record.Date.ToString("yyyy-MM-dd",
                            CultureInfo.InvariantCulture));
                    command.Parameters.AddWithValue(
                        "@description", record.Description ?? "");

                    command.ExecuteNonQuery();
                }
            }
        }

        // Удаляет запись по её уникальному номеру.
        public void DeleteRecord(int id)
        {
            using (var connection = new SqliteConnection(
                "Data Source=" + databasePath))
            {
                connection.Open();

                string sql = @"
                    DELETE FROM FinanceRecords
                    WHERE Id = @id;";

                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
