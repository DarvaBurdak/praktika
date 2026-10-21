
using EmployeesWPF;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace WpfApp1
{
    // Класс отвечает за создание базы данных и работу с сотрудниками.
    public class EmployeeDatabase
    {
        // Путь к файлу базы данных.
        private readonly string databasePath;

        // Строка подключения к SQLite.
        private string ConnectionString
        {
            get { return "Data Source=" + databasePath; }
        }

        // Конструктор вызывается при создании объекта базы данных.
        public EmployeeDatabase()
        {
            // Получаем путь к локальной папке пользователя Windows.
            string folder = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "EmployeesWPF");

            // Создаём папку, если она ещё не существует.
            Directory.CreateDirectory(folder);

            // Указываем имя файла базы данных.
            databasePath = Path.Combine(folder, "employees.db");

            // Создаём таблицу при первом запуске.
            InitializeDatabase();
        }

        // Создаёт таблицу сотрудников, если её ещё нет.
        private void InitializeDatabase()
        {
            using (SqliteConnection connection =
                new SqliteConnection(ConnectionString))
            {
                connection.Open();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        CREATE TABLE IF NOT EXISTS Employees
                        (
                            Id INTEGER PRIMARY KEY AUTOINCREMENT,
                            LastName TEXT NOT NULL,
                            FirstName TEXT NOT NULL,
                            Position TEXT NOT NULL,
                            Department TEXT NOT NULL,
                            Phone TEXT NOT NULL,
                            Salary NUMERIC NOT NULL
                        );";

                    // Выполняем SQL-команду создания таблицы.
                    command.ExecuteNonQuery();
                }
            }
        }

        // Возвращает список всех сотрудников из базы данных.
        public List<Employee> GetAllEmployees()
        {
            List<Employee> employees = new List<Employee>();

            using (SqliteConnection connection =
                new SqliteConnection(ConnectionString))
            {
                connection.Open();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        SELECT Id, LastName, FirstName, Position,
                               Department, Phone, Salary
                        FROM Employees
                        ORDER BY Id;";

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        // Перебираем все найденные записи.
                        while (reader.Read())
                        {
                            Employee employee = new Employee();

                            employee.Id = reader.GetInt32(0);
                            employee.LastName = reader.GetString(1);
                            employee.FirstName = reader.GetString(2);
                            employee.Position = reader.GetString(3);
                            employee.Department = reader.GetString(4);
                            employee.Phone = reader.GetString(5);

                            // Преобразуем зарплату в decimal.
                            employee.Salary = Convert.ToDecimal(
                                reader.GetValue(6),
                                CultureInfo.InvariantCulture);

                            employees.Add(employee);
                        }
                    }
                }
            }

            return employees;
        }

        // Добавляет нового сотрудника в базу данных.
        public void AddEmployee(Employee employee)
        {
            using (SqliteConnection connection =
                new SqliteConnection(ConnectionString))
            {
                connection.Open();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        INSERT INTO Employees
                        (LastName, FirstName, Position,
                         Department, Phone, Salary)
                        VALUES
                        (@LastName, @FirstName, @Position,
                         @Department, @Phone, @Salary);";

                    // Передаём данные через параметры запроса.
                    command.Parameters.AddWithValue(
                        "@LastName", employee.LastName);
                    command.Parameters.AddWithValue(
                        "@FirstName", employee.FirstName);
                    command.Parameters.AddWithValue(
                        "@Position", employee.Position);
                    command.Parameters.AddWithValue(
                        "@Department", employee.Department);
                    command.Parameters.AddWithValue(
                        "@Phone", employee.Phone);
                    command.Parameters.AddWithValue(
                        "@Salary", employee.Salary);

                    command.ExecuteNonQuery();
                }
            }
        }

        // Изменяет данные сотрудника с указанным ID.
        public void UpdateEmployee(Employee employee)
        {
            using (SqliteConnection connection =
                new SqliteConnection(ConnectionString))
            {
                connection.Open();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                        UPDATE Employees
                        SET LastName = @LastName,
                            FirstName = @FirstName,
                            Position = @Position,
                            Department = @Department,
                            Phone = @Phone,
                            Salary = @Salary
                        WHERE Id = @Id;";

                    command.Parameters.AddWithValue(
                        "@LastName", employee.LastName);
                    command.Parameters.AddWithValue(
                        "@FirstName", employee.FirstName);
                    command.Parameters.AddWithValue(
                        "@Position", employee.Position);
                    command.Parameters.AddWithValue(
                        "@Department", employee.Department);
                    command.Parameters.AddWithValue(
                        "@Phone", employee.Phone);
                    command.Parameters.AddWithValue(
                        "@Salary", employee.Salary);
                    command.Parameters.AddWithValue(
                        "@Id", employee.Id);

                    // Сохраняем изменения в базе данных.
                    command.ExecuteNonQuery();
                }
            }
        }

        // Удаляет сотрудника по его уникальному номеру.
        public void DeleteEmployee(int id)
        {
            using (SqliteConnection connection =
                new SqliteConnection(ConnectionString))
            {
                connection.Open();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText =
                        "DELETE FROM Employees WHERE Id = @Id;";

                    command.Parameters.AddWithValue("@Id", id);

                    // Выполняем удаление записи.
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
