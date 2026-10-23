using System; 
using System.Collections.Generic; 
using System.Globalization;
using StudentsApp;
namespace StudentsApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Создаём менеджер студентов.
            // Его конструктор автоматически загружает данные из JSON-файла.
            StudentManager manager = new StudentManager();
            // Переменная будет хранить пункт меню, введённый пользователем.
            string choice;
            // do-while сначала выполняет команды внутри блока, а затем проверяет условие продолжения цикла.
            // Поэтому меню появится как минимум один раз.
            do
            {
                // Пустая строка отделяет новое меню от предыдущего вывода.
                Console.WriteLine();
                // Заголовок программы.
                Console.WriteLine("=== УПРАВЛЕНИЕ СТУДЕНТАМИ ===");
                // Выводим доступные действия.
                Console.WriteLine("1. Показать всех студентов");
                Console.WriteLine("2. Добавить студента");
                Console.WriteLine("3. Редактировать студента");
                Console.WriteLine("4. Удалить студента");
                Console.WriteLine("5. Найти студента по имени");
                Console.WriteLine("6. Сортировать студентов");
                Console.WriteLine("0. Выход");
                // Просим пользователя выбрать команду.
                Console.Write("Выберите действие: ");
                // ReadLine считывает введённый текст до нажатия Enter.
                choice = Console.ReadLine();
                // switch сравнивает введённый текст с вариантами меню.
                switch (choice)
                {
                    case "1":
                        // Получаем список студентов.
                        // Передаём его методу ShowStudents для отображения.
                        ShowStudents(manager.GetAll());
                        break; // Завершаем текущую ветку switch.
                    case "2":
                        // Запускаем процедуру ввода данных нового студента.
                        AddStudent(manager);
                        break;
                    case "3":
                        // Запрашиваем ID и новые данные студента.
                        EditStudent(manager);
                        break;
                    case "4":
                        // Запрашиваем ID и удаляем соответствующую запись.
                        DeleteStudent(manager);
                        break;
                    case "5":
                        // Запрашиваем имя или его часть и выводим совпадения.
                        SearchStudents(manager);
                        break;
                    case "6":
                        // Предлагаем критерий сортировки.
                        SortStudents(manager);
                        break;
                    case "0":
                        // Пользователь выбрал завершение программы.
                        Console.WriteLine("Завершение программы.");
                        break;
                    default:
                        // Выполняется, если введён неизвестный пункт меню.
                        Console.WriteLine(
                            "Такого пункта нет. Попробуйте снова.");
                        break;
                }
                // Пока пользователь не ввёл "0", цикл повторяется.
            } while (choice != "0");
            // Перед выходом дополнительно сохраняем список.
            // При этом добавление, редактирование и удаление уже сохраняют данные сразу после выполнения каждой операции.
            manager.SaveToFile();
        }
        // Метод выводит список студентов в консоль.
        // Принимает List<Student>, поэтому может отображать как весь список, так и результаты поиска или сортировки.
        static void ShowStudents(List<Student> students)
        {
            // Count возвращает количество элементов в списке.
            // Если оно равно нулю, отображать нечего.
            if (students.Count == 0)
            {
                Console.WriteLine("Список студентов пуст.");
                return; // Завершаем метод, чтобы не выводить пустую таблицу.
            }
            // Выводим заголовки столбцов.
            Console.WriteLine(
                "\nID | ФИО | Группа | Возраст | Средний балл");
            // Повторяем символ '-' 65 раз для разделения заголовка и данных.
            Console.WriteLine(new string('-', 65));
            // foreach перебирает каждый объект в списке.
            // На каждой итерации переменная student обозначает текущий объект.
            foreach (Student student in students)
            {
                // Интерполяция строк ($"...") позволяет вставлять значения свойств объекта прямо в текст с помощью фигурных скобок.
                // F2 у среднего балла означает вывод двух знаков после запятой.
                Console.WriteLine(
                    $"{student.Id} | {student.FullName} | " +
                    $"{student.Group} | {student.Age} | " +
                    $"{student.AverageGrade:F2}");
            }
        }
        // Метод запрашивает данные и добавляет нового студента.
        static void AddStudent(StudentManager manager)
        {
            // try позволяет перехватить ожидаемые ошибки ввода и проверки данных.
            try
            {
                // Запрашиваем ФИО.
                Console.Write("Введите ФИО: ");
                string name = Console.ReadLine();
                // Запрашиваем название группы.
                Console.Write("Введите группу: ");
                string group = Console.ReadLine();
                // Запрашиваем возраст.
                Console.Write("Введите возраст: ");
                // TryParse пытается преобразовать текст в целое число.
                // Если преобразование не удалось, метод возвращает false.
                // В этом случае не продолжаем добавление.
                if (!int.TryParse(Console.ReadLine(), out int age))
                {
                    Console.WriteLine(
                        "Возраст должен быть целым числом.");
                    return;
                }
                // Запрашиваем средний балл.
                Console.Write("Введите средний балл (0–10): ");
                // Преобразуем текст в число double.
                // CurrentCulture позволяет вводить число с десятичным разделителем, принятым в настройках текущей системы.
                if (!double.TryParse(
                    Console.ReadLine(),
                    NumberStyles.Float,
                    CultureInfo.CurrentCulture,
                    out double grade))
                {
                    Console.WriteLine("Некорректный средний балл.");
                    return;
                }
                // Передаём введённые значения менеджеру.
                // Менеджер проверяет данные, создаёт объект и сохраняет файл.
                manager.AddStudent(name, group, age, grade);
                // Сообщаем об успешном выполнении операции.
                Console.WriteLine("Студент успешно добавлен.");
            }
            catch (ArgumentException ex)
            {
                // Если менеджер отклонил данные, выводим причину.
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
        // Метод редактирует существующую запись.
        static void EditStudent(StudentManager manager)
        {
            try
            {
                // Запрашиваем ID студента.
                Console.Write("Введите ID студента: ");
                // Проверяем, что ID введён как целое число.
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("ID должен быть целым числом.");
                    return;
                }
                // Получаем новые данные для выбранной записи.
                Console.Write("Введите новое ФИО: ");
                string name = Console.ReadLine();
                Console.Write("Введите новую группу: ");
                string group = Console.ReadLine();
                Console.Write("Введите новый возраст: ");
                // Проверяем формат возраста.
                if (!int.TryParse(Console.ReadLine(), out int age))
                {
                    Console.WriteLine(
                        "Возраст должен быть целым числом.");
                    return;
                }
                Console.Write("Введите новый средний балл (0–10): ");
                // Проверяем формат среднего балла.
                if (!double.TryParse(
                    Console.ReadLine(),
                    NumberStyles.Float,
                    CultureInfo.CurrentCulture,
                    out double grade))
                {
                    Console.WriteLine("Некорректный средний балл.");
                    return;
                }
                // Передаём ID и новые значения менеджеру.
                // Он найдёт запись, проверит данные и сохранит изменения.
                manager.UpdateStudent(id, name, group, age, grade);
                Console.WriteLine("Данные студента обновлены.");
            }
            catch (ArgumentException ex)
            {
                // Например, такой ID может отсутствовать в списке.
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
        // Метод удаляет студента по ID.
        static void DeleteStudent(StudentManager manager)
        {
            try
            {
                Console.Write("Введите ID студента для удаления: ");
                // Проверяем, что пользователь ввёл целое число.
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("ID должен быть целым числом.");
                    return;
                }
                // Вызываем удаление.
                // Менеджер проверит наличие записи и сохранит список в файл.
                manager.DeleteStudent(id);
                Console.WriteLine("Студент удалён.");
            }
            catch (ArgumentException ex)
            {
                // Выводим сообщение, если студент не найден.
                Console.WriteLine("Ошибка: " + ex.Message);
            }
        }
        // Метод выполняет поиск по имени или части ФИО.
        static void SearchStudents(StudentManager manager)
        {
            // Получаем поисковую строку.
            Console.Write("Введите имя или часть ФИО: ");
            string name = Console.ReadLine();
            // Менеджер возвращает список совпадений.
            // Если строка пустая, вернётся весь список.
            List<Student> result = manager.SearchByName(name);
            // Используем общий метод отображения.
            ShowStudents(result);
        }
        // Метод предлагает выбрать способ сортировки.
        static void SortStudents(StudentManager manager)
        {
            // Показываем доступные критерии сортировки.
            Console.WriteLine("\nСортировка:");
            Console.WriteLine("1. По ФИО");
            Console.WriteLine("2. По возрасту");
            Console.WriteLine("3. По среднему баллу");
            Console.WriteLine("4. По группе");
            Console.Write("Выберите критерий: ");
            // Считываем номер критерия и преобразуем его в int.
            if (!int.TryParse(Console.ReadLine(), out int criterion))
            {
                Console.WriteLine(
                    "Введите номер критерия числом.");
                return;
            }
            // Получаем отсортированный список.
            // Сортировка возвращает новый список и не меняет исходный порядок элементов внутри коллекции менеджера.
            List<Student> sorted = manager.SortStudents(criterion);
            // Показываем результат сортировки.
            ShowStudents(sorted);
        }
    }
}
