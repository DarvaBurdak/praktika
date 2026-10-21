
using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Data.Sqlite;

namespace WpfApp1 // Замени на пространство имён своего проекта
{
    public class TaskDatabase
    {
        // Путь к файлу базы данных на компьютере пользователя.
        private readonly string databasePath;

        // Конструктор: задаёт путь и создаёт таблицу, если её ещё нет.
        public TaskDatabase()
        {
            string folder = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "TaskManager");

            // Создаём папку для базы данных, если её нет.
            Directory.CreateDirectory(folder);

            databasePath = Path.Combine(folder, "tasks.db");

            // Создаём таблицу при первом запуске.
            InitializeDatabase();
        }

        // Создаёт таблицу для хранения задач.
        private void InitializeDatabase()
        {
            using (var connection = new SqliteConnection(
                "Data Source=" + databasePath))
            {
                connection.Open();

                string sql = @"
                    CREATE TABLE IF NOT EXISTS Tasks (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Description TEXT,
                        DueDate TEXT NOT NULL,
                        Status TEXT NOT NULL
                    );";

                using (var command = new SqliteCommand(sql, connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        // Получает все задачи из базы данных.
        public List<TaskItem> GetAllTasks()
        {
            var tasks = new List<TaskItem>();

            using (var connection = new SqliteConnection(
                "Data Source=" + databasePath))
            {
                connection.Open();

                string sql = @"
                    SELECT Id, Title, Description, DueDate, Status
                    FROM Tasks
                    ORDER BY Id DESC;";

                using (var command = new SqliteCommand(sql, connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        // Преобразуем запись базы данных в объект задачи.
                        var task = new TaskItem
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Description = reader.IsDBNull(2)
                                ? ""
                                : reader.GetString(2),
                            DueDate = DateTime.Parse(reader.GetString(3)),
                            Status = reader.GetString(4)
                        };

                        tasks.Add(task);
                    }
                }
            }

            return tasks;
        }

        // Добавляет новую задачу.
        public void AddTask(TaskItem task)
        {
            using (var connection = new SqliteConnection(
                "Data Source=" + databasePath))
            {
                connection.Open();

                string sql = @"
                    INSERT INTO Tasks (Title, Description, DueDate, Status)
                    VALUES (@title, @description, @dueDate, @status);";

                using (var command = new SqliteCommand(sql, connection))
                {
                    // Параметры защищают запрос и корректно передают значения.
                    command.Parameters.AddWithValue("@title", task.Title);
                    command.Parameters.AddWithValue(
                        "@description", task.Description ?? "");
                    command.Parameters.AddWithValue(
                        "@dueDate", task.DueDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@status", task.Status);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Обновляет выбранную задачу по её идентификатору.
        public void UpdateTask(TaskItem task)
        {
            using (var connection = new SqliteConnection(
                "Data Source=" + databasePath))
            {
                connection.Open();

                string sql = @"
                    UPDATE Tasks
                    SET Title = @title,
                        Description = @description,
                        DueDate = @dueDate,
                        Status = @status
                    WHERE Id = @id;";

                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", task.Id);
                    command.Parameters.AddWithValue("@title", task.Title);
                    command.Parameters.AddWithValue(
                        "@description", task.Description ?? "");
                    command.Parameters.AddWithValue(
                        "@dueDate", task.DueDate.ToString("yyyy-MM-dd"));
                    command.Parameters.AddWithValue("@status", task.Status);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Удаляет задачу по её идентификатору.
        public void DeleteTask(int id)
        {
            using (var connection = new SqliteConnection(
                "Data Source=" + databasePath))
            {
                connection.Open();

                string sql = "DELETE FROM Tasks WHERE Id = @id;";

                using (var command = new SqliteCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
