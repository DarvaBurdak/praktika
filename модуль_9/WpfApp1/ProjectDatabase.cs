
using Microsoft.Data.Sqlite;
using ProjectTasksWPF.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProjectTasksWPF.Data
{
    // Класс отвечает за все действия с базой данных:
    // создание таблиц, добавление, получение, изменение и удаление записей.
    public class ProjectDatabase
    {
        // Храним строку подключения к SQLite.
        // Она указывает, в каком файле будет находиться база данных.
        private readonly string connectionString;

        // Конструктор вызывается при создании объекта ProjectDatabase.
        public ProjectDatabase()
        {
            // Получаем путь к локальной папке данных текущего пользователя.
            string folder = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "ProjectTasksWPF");

            // Если папки ещё нет, создаём её.
            Directory.CreateDirectory(folder);

            // Формируем полный путь к файлу базы данных.
            string databasePath = Path.Combine(folder, "projects.db");

            // Создаём строку подключения SQLite.
            connectionString = "Data Source=" + databasePath;

            // При первом запуске создаём таблицы.
            InitializeDatabase();
        }

        // Открывает соединение с базой данных.
        private SqliteConnection OpenConnection()
        {
            // Создаём соединение на основании строки подключения.
            SqliteConnection connection =
                new SqliteConnection(connectionString);

            // Открываем соединение.
            connection.Open();

            // В SQLite поддержку внешних ключей нужно включать
            // для каждого нового соединения.
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = "PRAGMA foreign_keys = ON;";
                command.ExecuteNonQuery();
            }

            // Возвращаем открытое соединение вызывающему методу.
            return connection;
        }

        // Создаёт таблицы, если они ещё не существуют.
        private void InitializeDatabase()
        {
            using (SqliteConnection connection = OpenConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                // В одном запросе создаём три таблицы:
                // Projects, Employees и Tasks.
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Projects
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Description TEXT,
                        StartDate TEXT NOT NULL,
                        EndDate TEXT NOT NULL,
                        Status TEXT NOT NULL
                    );

                    CREATE TABLE IF NOT EXISTS Employees
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        FullName TEXT NOT NULL,
                        Position TEXT,
                        Email TEXT
                    );

                    CREATE TABLE IF NOT EXISTS Tasks
                    (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Description TEXT,
                        ProjectId INTEGER NOT NULL,
                        EmployeeId INTEGER NULL,
                        DueDate TEXT NOT NULL,
                        Progress INTEGER NOT NULL DEFAULT 0
                            CHECK (Progress >= 0 AND Progress <= 100),
                        Status TEXT NOT NULL,

                        FOREIGN KEY (ProjectId)
                            REFERENCES Projects(Id)
                            ON DELETE CASCADE,

                        FOREIGN KEY (EmployeeId)
                            REFERENCES Employees(Id)
                            ON DELETE SET NULL
                    );
                ";

                // Выполняем SQL-запросы создания таблиц.
                command.ExecuteNonQuery();
            }
        }

        // =========================================================
        // РАЗДЕЛ 1. РАБОТА С ПРОЕКТАМИ
        // =========================================================

        // Возвращает список всех проектов из базы данных.
        public List<Project> GetProjects()
        {
            List<Project> projects = new List<Project>();

            using (SqliteConnection connection = OpenConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT Id, Name, Description, StartDate, EndDate, Status
                      FROM Projects
                      ORDER BY Id DESC;";

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    // Читаем записи одну за другой.
                    while (reader.Read())
                    {
                        Project project = new Project();

                        project.Id = reader.GetInt32(0);
                        project.Name = reader.GetString(1);

                        // Проверяем NULL, поскольку описание может отсутствовать.
                        project.Description = reader.IsDBNull(2)
                            ? ""
                            : reader.GetString(2);

                        project.StartDate =
                            DateTime.Parse(reader.GetString(3));

                        project.EndDate =
                            DateTime.Parse(reader.GetString(4));

                        project.Status = reader.GetString(5);

                        projects.Add(project);
                    }
                }
            }

            return projects;
        }

        // Добавляет новый проект.
        public void AddProject(Project project)
        {
            using (SqliteConnection connection = OpenConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO Projects
                    (Name, Description, StartDate, EndDate, Status)
                    VALUES
                    (@Name, @Description, @StartDate, @EndDate, @Status);";

                // Параметры защищают запрос от ошибок при работе с текстом.
                command.Parameters.AddWithValue("@Name", project.Name);
                command.Parameters.AddWithValue(
                    "@Description", project.Description ?? "");
                command.Parameters.AddWithValue(
                    "@StartDate", project.StartDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue(
                    "@EndDate", project.EndDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@Status", project.Status);

                command.ExecuteNonQuery();
            }
        }

        // Изменяет существующий проект.
        public void UpdateProject(Project project)
        {
            using (SqliteConnection connection = OpenConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE Projects
                    SET Name = @Name,
                        Description = @Description,
                        StartDate = @StartDate,
                        EndDate = @EndDate,
                        Status = @Status
                    WHERE Id = @Id;";

                command.Parameters.AddWithValue("@Id", project.Id);
                command.Parameters.AddWithValue("@Name", project.Name);
                command.Parameters.AddWithValue(
                    "@Description", project.Description ?? "");
                command.Parameters.AddWithValue(
                    "@StartDate", project.StartDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue(
                    "@EndDate", project.EndDate.ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@Status", project.Status);

                command.ExecuteNonQuery();
            }
        }

        // Удаляет проект по его идентификатору.
        // Связанные задачи удаляются благодаря ON DELETE CASCADE.
        public void DeleteProject(int id)
        {
            using (SqliteConnection connection = OpenConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "DELETE FROM Projects WHERE Id = @Id;";

                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        // =========================================================
        // РАЗДЕЛ 2. РАБОТА С СОТРУДНИКАМИ
        // =========================================================

        // Возвращает список сотрудников.
        public List<Employee> GetEmployees()
        {
            List<Employee> employees = new List<Employee>();

            using (SqliteConnection connection = OpenConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    @"SELECT Id, FullName, Position, Email
                      FROM Employees
                      ORDER BY FullName;";

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Employee employee = new Employee();

                        employee.Id = reader.GetInt32(0);
                        employee.FullName = reader.GetString(1);

                        employee.Position = reader.IsDBNull(2)
                            ? ""
                            : reader.GetString(2);

                        employee.Email = reader.IsDBNull(3)
                            ? ""
                            : reader.GetString(3);

                        employees.Add(employee);
                    }
                }
            }

            return employees;
        }

        // Добавляет сотрудника.
        public void AddEmployee(Employee employee)
        {
            using (SqliteConnection connection = OpenConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO Employees (FullName, Position, Email)
                    VALUES (@FullName, @Position, @Email);";

                command.Parameters.AddWithValue(
                    "@FullName", employee.FullName);
                command.Parameters.AddWithValue(
                    "@Position", employee.Position ?? "");
                command.Parameters.AddWithValue(
                    "@Email", employee.Email ?? "");

                command.ExecuteNonQuery();
            }
        }

        // Изменяет сведения о сотруднике.
        public void UpdateEmployee(Employee employee)
        {
            using (SqliteConnection connection = OpenConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE Employees
                    SET FullName = @FullName,
                        Position = @Position,
                        Email = @Email
                    WHERE Id = @Id;";

                command.Parameters.AddWithValue("@Id", employee.Id);
                command.Parameters.AddWithValue(
                    "@FullName", employee.FullName);
                command.Parameters.AddWithValue(
                    "@Position", employee.Position ?? "");
                command.Parameters.AddWithValue(
                    "@Email", employee.Email ?? "");

                command.ExecuteNonQuery();
            }
        }

        // Удаляет сотрудника.
        // Если сотрудник был назначен на задачи, исполнитель в этих задачах
        // станет пустым благодаря ON DELETE SET NULL.
        public void DeleteEmployee(int id)
        {
            using (SqliteConnection connection = OpenConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "DELETE FROM Employees WHERE Id = @Id;";

                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        // =========================================================
        // РАЗДЕЛ 3. РАБОТА С ЗАДАЧАМИ
        // =========================================================

        // Возвращает все задачи вместе с названиями проектов
        // и именами назначенных сотрудников.
        public List<TaskItem> GetTasks()
        {
            List<TaskItem> tasks = new List<TaskItem>();

            using (SqliteConnection connection = OpenConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                // LEFT JOIN позволяет показать задачу даже тогда,
                // когда исполнитель не назначен.
                command.CommandText = @"
                    SELECT
                        T.Id,
                        T.Title,
                        T.Description,
                        T.ProjectId,
                        T.EmployeeId,
                        T.DueDate,
                        T.Progress,
                        T.Status,
                        P.Name,
                        E.FullName
                    FROM Tasks T
                    INNER JOIN Projects P ON T.ProjectId = P.Id
                    LEFT JOIN Employees E ON T.EmployeeId = E.Id
                    ORDER BY T.Id DESC;";

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        TaskItem task = new TaskItem();

                        task.Id = reader.GetInt32(0);
                        task.Title = reader.GetString(1);

                        task.Description = reader.IsDBNull(2)
                            ? ""
                            : reader.GetString(2);

                        task.ProjectId = reader.GetInt32(3);

                        task.EmployeeId = reader.IsDBNull(4)
                            ? (int?)null
                            : reader.GetInt32(4);

                        task.DueDate =
                            DateTime.Parse(reader.GetString(5));

                        task.Progress = reader.GetInt32(6);
                        task.Status = reader.GetString(7);
                        task.ProjectName = reader.GetString(8);

                        task.EmployeeName = reader.IsDBNull(9)
                            ? "Не назначен"
                            : reader.GetString(9);

                        tasks.Add(task);
                    }
                }
            }

            return tasks;
        }

        // Добавляет новую задачу.
        public void AddTask(TaskItem task)
        {
            using (SqliteConnection connection = OpenConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    INSERT INTO Tasks
                    (Title, Description, ProjectId, EmployeeId,
                     DueDate, Progress, Status)
                    VALUES
                    (@Title, @Description, @ProjectId, @EmployeeId,
                     @DueDate, @Progress, @Status);";

                command.Parameters.AddWithValue("@Title", task.Title);
                command.Parameters.AddWithValue(
                    "@Description", task.Description ?? "");
                command.Parameters.AddWithValue(
                    "@ProjectId", task.ProjectId);

                // DBNull.Value означает SQL NULL:
                // задача может существовать без назначенного сотрудника.
                command.Parameters.AddWithValue(
                    "@EmployeeId",
                    task.EmployeeId.HasValue
                        ? (object)task.EmployeeId.Value
                        : DBNull.Value);

                command.Parameters.AddWithValue(
                    "@DueDate", task.DueDate.ToString("yyyy-MM-dd"));

                command.Parameters.AddWithValue(
                    "@Progress", task.Progress);

                command.Parameters.AddWithValue("@Status", task.Status);

                command.ExecuteNonQuery();
            }
        }

        // Изменяет существующую задачу.
        public void UpdateTask(TaskItem task)
        {
            using (SqliteConnection connection = OpenConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = @"
                    UPDATE Tasks
                    SET Title = @Title,
                        Description = @Description,
                        ProjectId = @ProjectId,
                        EmployeeId = @EmployeeId,
                        DueDate = @DueDate,
                        Progress = @Progress,
                        Status = @Status
                    WHERE Id = @Id;";

                command.Parameters.AddWithValue("@Id", task.Id);
                command.Parameters.AddWithValue("@Title", task.Title);
                command.Parameters.AddWithValue(
                    "@Description", task.Description ?? "");
                command.Parameters.AddWithValue(
                    "@ProjectId", task.ProjectId);

                command.Parameters.AddWithValue(
                    "@EmployeeId",
                    task.EmployeeId.HasValue
                        ? (object)task.EmployeeId.Value
                        : DBNull.Value);

                command.Parameters.AddWithValue(
                    "@DueDate", task.DueDate.ToString("yyyy-MM-dd"));

                command.Parameters.AddWithValue(
                    "@Progress", task.Progress);

                command.Parameters.AddWithValue("@Status", task.Status);

                command.ExecuteNonQuery();
            }
        }

        // Удаляет задачу по идентификатору.
        public void DeleteTask(int id)
        {
            using (SqliteConnection connection = OpenConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    "DELETE FROM Tasks WHERE Id = @Id;";

                command.Parameters.AddWithValue("@Id", id);
                command.ExecuteNonQuery();
            }
        }

        // =========================================================
        // РАЗДЕЛ 4. ОТЧЁТЫ
        // =========================================================

        // Формирует сводку по каждому проекту.
        public List<ProjectReport> GetProjectReports()
        {
            List<ProjectReport> reports = new List<ProjectReport>();

            using (SqliteConnection connection = OpenConnection())
            using (SqliteCommand command = connection.CreateCommand())
            {
                // Считаем общее количество задач, завершённые задачи
                // и средний процент выполнения.
                command.CommandText = @"
                    SELECT
                        P.Name,
                        P.Status,
                        COUNT(T.Id) AS TotalTasks,
                        COALESCE(
                            SUM(CASE WHEN T.Progress = 100
                                THEN 1 ELSE 0 END), 0
                        ) AS CompletedTasks,
                        COALESCE(AVG(T.Progress), 0) AS AverageProgress
                    FROM Projects P
                    LEFT JOIN Tasks T ON T.ProjectId = P.Id
                    GROUP BY P.Id, P.Name, P.Status
                    ORDER BY P.Name;";

                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ProjectReport report = new ProjectReport();

                        report.ProjectName = reader.GetString(0);
                        report.Status = reader.GetString(1);
                        report.TotalTasks = reader.GetInt32(2);
                        report.CompletedTasks = reader.GetInt32(3);

                        // Количество оставшихся задач вычисляем
                        // как разницу между общим и завершённым количеством.
                        report.RemainingTasks =
                            report.TotalTasks - report.CompletedTasks;

                        report.AverageProgress = reader.GetDouble(4);

                        reports.Add(report);
                    }
                }
            }

            return reports;
        }
    }
}
