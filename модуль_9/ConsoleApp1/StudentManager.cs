using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
namespace StudentsApp
{
    // Класс отвечает за управление студентами и работу с файлом.
    public class StudentManager
    {
        // Список всех студентов, с которыми работает программа.
        private List<Student> students = new List<Student>();
        // Файл, в котором постоянно хранятся данные студентов.
        private readonly string filePath = "students.json";
        // Следующий идентификатор для нового студента.
        private int nextId = 1;
        // Конструктор автоматически загружает сохранённые данные.
        public StudentManager()
        {
            // Если файл существует, читаем из него студентов.
            LoadFromFile();
            // Определяем следующий свободный ID.
            // Если список пустой, начинаем с 1.
            if (students.Count > 0)
            {
                nextId = students.Max(s => s.Id) + 1;
            }
        }
        // Возвращает копию списка всех студентов.
        public List<Student> GetAll()
        {
            // Создаём новый список, чтобы не выдавать исходную коллекцию напрямую.
            return new List<Student>(students);
        }
        // Добавляет нового студента.
        public void AddStudent(string fullName, string group,
                               int age, double averageGrade)
        {
            // Проверяем, что ФИО заполнено.
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("ФИО не может быть пустым.");
            }
            // Проверяем корректность возраста.
            if (age < 1 || age > 120)
            {
                throw new ArgumentException("Возраст должен быть от 1 до 120.");
            }
            // Средний балл должен находиться в диапазоне от 0 до 10.
            if (averageGrade < 0 || averageGrade > 10)
            {
                throw new ArgumentException("Средний балл должен быть от 0 до 10.");
            }
            // Создаём объект студента с новым уникальным ID.
            Student student = new Student(
                nextId,
                fullName.Trim(),
                (group ?? "").Trim(),
                age,
                averageGrade);
            // Добавляем студента в список.
            students.Add(student);
            // Увеличиваем ID для следующей записи.
            nextId++;
            // Сохраняем обновлённый список в файл.
            SaveToFile();
        }
        // Редактирует существующего студента по ID.
        public void UpdateStudent(int id, string fullName, string group,
                                  int age, double averageGrade)
        {
            // Ищем студента с указанным ID.
            Student student = students.FirstOrDefault(s => s.Id == id);
            // Если студент не найден, сообщаем об ошибке.
            if (student == null)
            {
                throw new ArgumentException("Студент не найден.");
            }
            // Проверяем новые данные перед сохранением.
            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw new ArgumentException("ФИО не может быть пустым.");
            }
            if (age < 1 || age > 120)
            {
                throw new ArgumentException("Возраст должен быть от 1 до 120.");
            }

            if (averageGrade < 0 || averageGrade > 10)
            {
                throw new ArgumentException("Средний балл должен быть от 0 до 10.");
            }
            // Заменяем старые значения новыми.
            student.FullName = fullName.Trim();
            student.Group = (group ?? "").Trim();
            student.Age = age;
            student.AverageGrade = averageGrade;
            // Сохраняем изменения в файле.
            SaveToFile();
        }
        // Удаляет студента по ID.
        public void DeleteStudent(int id)
        {
            // Находим нужную запись.
            Student student = students.FirstOrDefault(s => s.Id == id);
            // Проверяем, существует ли студент.
            if (student == null)
            {
                throw new ArgumentException("Студент не найден.");
            }
            // Удаляем студента из списка.
            students.Remove(student);
            // Сохраняем список без удалённой записи.
            SaveToFile();
        }
        // Ищет студентов по части ФИО.
        public List<Student> SearchByName(string name)
        {
            // Если строка поиска пустая, возвращаем весь список.
            if (string.IsNullOrWhiteSpace(name))
            {
                return GetAll();
            }
            // Находим все совпадения без учёта регистра букв.
            return students
                .Where(s => s.FullName.IndexOf(
                    name.Trim(),
                    StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();
        }
        // Сортирует студентов по выбранному критерию.
        public List<Student> SortStudents(int criterion)
        {
            // В зависимости от выбора пользователя выполняется нужная сортировка.
            switch (criterion)
            {
                // Сортировка по ФИО в алфавитном порядке.
                case 1:
                    return students.OrderBy(s => s.FullName).ToList();
                // Сортировка по возрасту от меньшего к большему.
                case 2:
                    return students.OrderBy(s => s.Age).ToList();
                // Сортировка по среднему баллу от большего к меньшему.
                case 3:
                    return students.OrderByDescending(
                        s => s.AverageGrade).ToList();
                // Сортировка по названию группы.
                case 4:
                    return students.OrderBy(s => s.Group).ToList();
                // Если критерий неизвестен, возвращаем обычный список.
                default:
                    return GetAll();
            }
        }
        // Сохраняет весь список студентов в JSON-файл.
        public void SaveToFile()
        {
            // Настраиваем красивое форматирование JSON для удобства чтения.
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            // Преобразуем список объектов в текст JSON.
            string json = JsonSerializer.Serialize(students, options);
            // Записываем полученный текст в файл.
            // При повторном сохранении прежнее содержимое заменяется актуальным.
            File.WriteAllText(filePath, json);
        }
        // Загружает список студентов из JSON-файла.
        public void LoadFromFile()
        {
            // Если файл ещё не создан, начинаем с пустого списка.
            if (!File.Exists(filePath))
            {
                students = new List<Student>();
                return;
            }
            // Читаем весь текст из файла.
            string json = File.ReadAllText(filePath);
            // Преобразуем JSON обратно в список объектов Student.
            List<Student> loadedStudents =
                JsonSerializer.Deserialize<List<Student>>(json);
            // Если файл пустой или содержит JSON null, используем пустой список.
            students = loadedStudents ?? new List<Student>();
        }
    }
}
