using System.Text.RegularExpressions;
using System.Globalization;
using ProjectTasksWPF.Data;
using ProjectTasksWPF.Models;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ProjectTasksWPF
{
    // Главный класс окна программы.
    public partial class MainWindow : Window
    {
        // Объект для взаимодействия с базой данных SQLite.
        private readonly ProjectDatabase database;

        // Конструктор вызывается при запуске главного окна.
        public MainWindow()
        {
            // Создаём элементы интерфейса, описанные в MainWindow.xaml.
            InitializeComponent();

            // Создаём объект базы данных.
            // В его конструкторе автоматически создаются таблицы.
            database = new ProjectDatabase();

            // Устанавливаем начальные значения дат.
            ProjectStartDate.SelectedDate = DateTime.Today;
            ProjectEndDate.SelectedDate = DateTime.Today.AddDays(30);
            TaskDueDate.SelectedDate = DateTime.Today.AddDays(7);

            // Устанавливаем начальные статусы в выпадающих списках.
            ProjectStatusBox.SelectedIndex = 0;
            TaskStatusBox.SelectedIndex = 0;

            // Загружаем данные из базы в таблицы интерфейса.
            RefreshAllData();
        }

        // =========================================================
        // ОБНОВЛЕНИЕ ДАННЫХ НА ЭКРАНЕ
        // =========================================================

        // Обновляет все таблицы после добавления, изменения или удаления.
        private void RefreshAllData()
        {
            RefreshProjects();
            RefreshEmployees();
            RefreshTasks();
            RefreshReports();
        }

        // Получает проекты из базы и показывает их в таблице.
        private void RefreshProjects()
        {
            ProjectsGrid.ItemsSource = null;
            ProjectsGrid.ItemsSource = database.GetProjects();

            // Список проектов в форме задачи нужен для назначения задачи
            // конкретному проекту.
            TaskProjectBox.ItemsSource = database.GetProjects();
        }

        // Получает сотрудников из базы и показывает их в таблице.
        private void RefreshEmployees()
        {
            EmployeesGrid.ItemsSource = null;
            EmployeesGrid.ItemsSource = database.GetEmployees();

            // Список сотрудников используется при назначении исполнителя.
            TaskEmployeeBox.ItemsSource = database.GetEmployees();

            // Первый элемент — пустое назначение.
            // Это позволяет оставить задачу без исполнителя.
            TaskEmployeeBox.SelectedIndex = -1;
        }

        // Получает задачи и показывает их в таблице.
        private void RefreshTasks()
        {
            TasksGrid.ItemsSource = null;
            TasksGrid.ItemsSource = database.GetTasks();
        }

        // Получает сводные отчёты и показывает их в таблице.
        private void RefreshReports()
        {
            ReportsGrid.ItemsSource = null;
            ReportsGrid.ItemsSource = database.GetProjectReports();
        }

        // =========================================================
        // ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ
        // =========================================================

        // Возвращает текст выбранного пункта ComboBox.
        // Если пользователь ничего не выбрал, возвращается пустая строка.
        private string GetComboBoxText(ComboBox comboBox)
        {
            ComboBoxItem item = comboBox.SelectedItem as ComboBoxItem;

            if (item == null)
            {
                return "";
            }

            return item.Content == null ? "" : item.Content.ToString();
        }

        // Показывает информационное сообщение пользователю.
        private void ShowInfo(string message)
        {
            MessageBox.Show(
                message,
                "Управление проектами",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        // Показывает сообщение об ошибке.
        private void ShowError(string message)
        {
            MessageBox.Show(
                message,
                "Ошибка",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        // Спрашивает подтверждение перед удалением записи.
        private bool ConfirmDelete(string message)
        {
            MessageBoxResult result = MessageBox.Show(
                message,
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            // Возвращаем true только тогда, когда нажата кнопка «Да».
            return result == MessageBoxResult.Yes;
        }

        // =========================================================
        // ПРОЕКТЫ: ДОБАВЛЕНИЕ
        // =========================================================

        private void AddProject_Click(object sender, RoutedEventArgs e)
        {
            // Название проекта обязательно для заполнения.
            if (string.IsNullOrWhiteSpace(ProjectNameBox.Text))
            {
                ShowError("Введите название проекта.");
                return;
            }

            // Проверяем, указаны ли обе даты.
            if (!ProjectStartDate.SelectedDate.HasValue ||
                !ProjectEndDate.SelectedDate.HasValue)
            {
                ShowError("Укажите даты начала и завершения проекта.");
                return;
            }

            // Дата завершения не должна быть раньше даты начала.
            if (ProjectEndDate.SelectedDate.Value <
                ProjectStartDate.SelectedDate.Value)
            {
                ShowError("Дата завершения не может быть раньше даты начала.");
                return;
            }

            string status = GetComboBoxText(ProjectStatusBox);

            if (string.IsNullOrWhiteSpace(status))
            {
                ShowError("Выберите статус проекта.");
                return;
            }

            // Создаём объект с введёнными пользователем данными.
            Project project = new Project
            {
                Name = ProjectNameBox.Text.Trim(),
                Description = ProjectDescriptionBox.Text.Trim(),
                StartDate = ProjectStartDate.SelectedDate.Value,
                EndDate = ProjectEndDate.SelectedDate.Value,
                Status = status
            };

            // Сохраняем проект в базе данных.
            database.AddProject(project);

            // Обновляем таблицы и выпадающие списки.
            RefreshAllData();

            // Очищаем поля для ввода следующего проекта.
            ClearProjectFields();

            ShowInfo("Проект успешно добавлен.");
        }

        // =========================================================
        // ПРОЕКТЫ: РЕДАКТИРОВАНИЕ
        // =========================================================

        private void UpdateProject_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выделенную строку таблицы.
            Project selectedProject = ProjectsGrid.SelectedItem as Project;

            // Без выделенной строки непонятно, какой проект изменять.
            if (selectedProject == null)
            {
                ShowError("Сначала выберите проект в таблице.");
                return;
            }

            if (string.IsNullOrWhiteSpace(ProjectNameBox.Text))
            {
                ShowError("Введите название проекта.");
                return;
            }

            if (!ProjectStartDate.SelectedDate.HasValue ||
                !ProjectEndDate.SelectedDate.HasValue)
            {
                ShowError("Укажите даты начала и завершения проекта.");
                return;
            }

            if (ProjectEndDate.SelectedDate.Value <
                ProjectStartDate.SelectedDate.Value)
            {
                ShowError("Дата завершения не может быть раньше даты начала.");
                return;
            }

            string status = GetComboBoxText(ProjectStatusBox);

            if (string.IsNullOrWhiteSpace(status))
            {
                ShowError("Выберите статус проекта.");
                return;
            }

            // Сохраняем идентификатор выбранного проекта,
            // а остальные свойства заменяем данными из полей.
            selectedProject.Name = ProjectNameBox.Text.Trim();
            selectedProject.Description = ProjectDescriptionBox.Text.Trim();
            selectedProject.StartDate = ProjectStartDate.SelectedDate.Value;
            selectedProject.EndDate = ProjectEndDate.SelectedDate.Value;
            selectedProject.Status = status;

            database.UpdateProject(selectedProject);

            RefreshAllData();
            ClearProjectFields();

            ShowInfo("Проект успешно изменён.");
        }

        // =========================================================
        // ПРОЕКТЫ: УДАЛЕНИЕ
        // =========================================================

        private void DeleteProject_Click(object sender, RoutedEventArgs e)
        {
            Project selectedProject = ProjectsGrid.SelectedItem as Project;

            if (selectedProject == null)
            {
                ShowError("Выберите проект, который нужно удалить.");
                return;
            }

            // Предупреждаем пользователя, что вместе с проектом
            // будут удалены связанные с ним задачи.
            if (!ConfirmDelete(
                "Удалить проект «" + selectedProject.Name +
                "» и все его задачи?"))
            {
                return;
            }

            database.DeleteProject(selectedProject.Id);

            RefreshAllData();
            ClearProjectFields();

            ShowInfo("Проект удалён.");
        }

        // Очищает поля проекта по нажатию кнопки.
        private void ClearProjectFields_Click(
            object sender, RoutedEventArgs e)
        {
            ClearProjectFields();
        }

        // Общий метод очистки полей проекта.
        private void ClearProjectFields()
        {
            ProjectNameBox.Clear();
            ProjectDescriptionBox.Clear();

            ProjectStartDate.SelectedDate = DateTime.Today;
            ProjectEndDate.SelectedDate = DateTime.Today.AddDays(30);

            ProjectStatusBox.SelectedIndex = 0;
            ProjectsGrid.SelectedItem = null;
        }

        // =========================================================
        // СОТРУДНИКИ: ДОБАВЛЕНИЕ
        // =========================================================

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EmployeeNameBox.Text))
            {
                ShowError("Введите ФИО сотрудника.");
                return;
            }

            // Создаём объект сотрудника на основании заполненных полей.
            Employee employee = new Employee
            {
                FullName = EmployeeNameBox.Text.Trim(),
                Position = EmployeePositionBox.Text.Trim(),
                Email = EmployeeEmailBox.Text.Trim()
            };
            // Получаем ФИО и электронную почту из полей ввода.
            string fullName = EmployeeNameBox.Text.Trim();
            string email = EmployeeEmailBox.Text.Trim();

            // Проверяем правильность ФИО.
            if (!IsValidFullName(fullName))
            {
                ShowError(
                    "Введите ФИО из трёх слов. " +
                    "Каждое слово должно начинаться с заглавной буквы.\n" +
                    "Пример: Иванов Иван Иванович");

                return; // Прерываем сохранение при ошибке.
            }

            // Проверяем правильность электронной почты.
            if (!IsValidEmail(email))
            {
                ShowError(
                    "Введите корректный адрес электронной почты.\n" +
                    "Пример: ivanov@mail.ru");

                return; // Прерываем сохранение при ошибке.
            }

            database.AddEmployee(employee);

            RefreshAllData();
            ClearEmployeeFields();

            ShowInfo("Сотрудник успешно добавлен.");
        }

        // =========================================================
        // СОТРУДНИКИ: РЕДАКТИРОВАНИЕ
        // =========================================================

        private void UpdateEmployee_Click(
            object sender, RoutedEventArgs e)
        {
            Employee selectedEmployee =
                EmployeesGrid.SelectedItem as Employee;

            if (selectedEmployee == null)
            {
                ShowError("Сначала выберите сотрудника в таблице.");
                return;
            }

            if (string.IsNullOrWhiteSpace(EmployeeNameBox.Text))
            {
                ShowError("Введите ФИО сотрудника.");
                return;
            }

            selectedEmployee.FullName = EmployeeNameBox.Text.Trim();
            selectedEmployee.Position = EmployeePositionBox.Text.Trim();
            selectedEmployee.Email = EmployeeEmailBox.Text.Trim();
            // Получаем ФИО и электронную почту из полей ввода.
            string fullName = EmployeeNameBox.Text.Trim();
            string email = EmployeeEmailBox.Text.Trim();

            // Проверяем правильность ФИО.
            if (!IsValidFullName(fullName))
            {
                ShowError(
                    "Введите ФИО из трёх слов. " +
                    "Каждое слово должно начинаться с заглавной буквы.\n" +
                    "Пример: Иванов Иван Иванович");

                return; // Прерываем сохранение при ошибке.
            }

            // Проверяем правильность электронной почты.
            if (!IsValidEmail(email))
            {
                ShowError(
                    "Введите корректный адрес электронной почты.\n" +
                    "Пример: ivanov@mail.ru");

                return; // Прерываем сохранение при ошибке.
            }

            database.UpdateEmployee(selectedEmployee);

            RefreshAllData();
            ClearEmployeeFields();

            ShowInfo("Данные сотрудника изменены.");
        }

        // =========================================================
        // СОТРУДНИКИ: УДАЛЕНИЕ
        // =========================================================

        private void DeleteEmployee_Click(
            object sender, RoutedEventArgs e)
        {
            Employee selectedEmployee =
                EmployeesGrid.SelectedItem as Employee;

            if (selectedEmployee == null)
            {
                ShowError("Выберите сотрудника, которого нужно удалить.");
                return;
            }

            if (!ConfirmDelete(
                "Удалить сотрудника «" +
                selectedEmployee.FullName + "»?"))
            {
                return;
            }

            database.DeleteEmployee(selectedEmployee.Id);

            RefreshAllData();
            ClearEmployeeFields();

            ShowInfo("Сотрудник удалён.");
        }

        // Очищает поля сотрудника по нажатию кнопки.
        private void ClearEmployeeFields_Click(
            object sender, RoutedEventArgs e)
        {
            ClearEmployeeFields();
        }

        // Общий метод очистки полей сотрудника.
        private void ClearEmployeeFields()
        {
            EmployeeNameBox.Clear();
            EmployeePositionBox.Clear();
            EmployeeEmailBox.Clear();

            EmployeesGrid.SelectedItem = null;
        }

        // =========================================================
        // ЗАДАЧИ: ПРОВЕРКА ВВЕДЁННЫХ ДАННЫХ
        // =========================================================

        // Проверяет данные задачи и создаёт её объект.
        // При ошибке возвращает false, чтобы отменить сохранение.
        private bool TryReadTaskFields(out TaskItem task)
        {
            task = null;

            // Проверяем название задачи.
            if (string.IsNullOrWhiteSpace(TaskTitleBox.Text))
            {
                ShowError("Введите название задачи.");
                return false;
            }

            // Для каждой задачи должен быть выбран проект.
            Project selectedProject = TaskProjectBox.SelectedItem as Project;

            if (selectedProject == null)
            {
                ShowError("Сначала создайте проект и выберите его для задачи.");
                return false;
            }

            // Проверяем срок выполнения.
            if (!TaskDueDate.SelectedDate.HasValue)
            {
                ShowError("Укажите срок выполнения задачи.");
                return false;
            }

            // Процент выполнения должен быть целым числом от 0 до 100.
            int progress;

            if (!int.TryParse(TaskProgressBox.Text, out progress) ||
                progress < 0 || progress > 100)
            {
                ShowError("Процент выполнения должен быть числом от 0 до 100.");
                return false;
            }

            string status = GetComboBoxText(TaskStatusBox);

            if (string.IsNullOrWhiteSpace(status))
            {
                ShowError("Выберите статус задачи.");
                return false;
            }

            // Если исполнитель не выбран, EmployeeId останется равным null.
            Employee selectedEmployee =
                TaskEmployeeBox.SelectedItem as Employee;

            // Создаём объект задачи.
            task = new TaskItem
            {
                Title = TaskTitleBox.Text.Trim(),
                Description = TaskDescriptionBox.Text.Trim(),
                ProjectId = selectedProject.Id,
                EmployeeId = selectedEmployee == null
                    ? (int?)null
                    : selectedEmployee.Id,
                DueDate = TaskDueDate.SelectedDate.Value,
                Progress = progress,
                Status = status
            };

            // Все проверки пройдены.
            return true;
        }

        // =========================================================
        // ЗАДАЧИ: ДОБАВЛЕНИЕ
        // =========================================================

        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            TaskItem task;

            // Если проверка не прошла, ничего не сохраняем.
            if (!TryReadTaskFields(out task))
            {
                return;
            }

            database.AddTask(task);

            RefreshAllData();
            ClearTaskFields();

            ShowInfo("Задача успешно добавлена.");
        }

        // =========================================================
        // ЗАДАЧИ: РЕДАКТИРОВАНИЕ
        // =========================================================

        private void UpdateTask_Click(object sender, RoutedEventArgs e)
        {
            TaskItem selectedTask = TasksGrid.SelectedItem as TaskItem;

            if (selectedTask == null)
            {
                ShowError("Сначала выберите задачу в таблице.");
                return;
            }

            TaskItem editedTask;

            if (!TryReadTaskFields(out editedTask))
            {
                return;
            }

            // Сохраняем исходный идентификатор задачи.
            editedTask.Id = selectedTask.Id;

            database.UpdateTask(editedTask);

            RefreshAllData();
            ClearTaskFields();

            ShowInfo("Задача успешно изменена.");
        }

        // =========================================================
        // ЗАДАЧИ: УДАЛЕНИЕ
        // =========================================================

        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            TaskItem selectedTask = TasksGrid.SelectedItem as TaskItem;

            if (selectedTask == null)
            {
                ShowError("Выберите задачу, которую нужно удалить.");
                return;
            }

            if (!ConfirmDelete(
                "Удалить задачу «" + selectedTask.Title + "»?"))
            {
                return;
            }

            database.DeleteTask(selectedTask.Id);

            RefreshAllData();
            ClearTaskFields();

            ShowInfo("Задача удалена.");
        }

        // Очищает поля задачи.
        private void ClearTaskFields_Click(
            object sender, RoutedEventArgs e)
        {
            ClearTaskFields();
        }

        // Общий метод очистки полей задачи.
        private void ClearTaskFields()
        {
            TaskTitleBox.Clear();
            TaskDescriptionBox.Clear();
            TaskProgressBox.Text = "0";

            TaskProjectBox.SelectedIndex = -1;
            TaskEmployeeBox.SelectedIndex = -1;

            TaskDueDate.SelectedDate = DateTime.Today.AddDays(7);
            TaskStatusBox.SelectedIndex = 0;

            TasksGrid.SelectedItem = null;
        }

        // =========================================================
        // ОТЧЁТЫ
        // =========================================================

        // Обновляет таблицу отчётов по нажатию кнопки.
        private void RefreshReports_Click(
            object sender, RoutedEventArgs e)
        {
            RefreshReports();

            ShowInfo("Отчёт обновлён.");
        }

        // =========================================================
        // АВТОМАТИЧЕСКОЕ ЗАПОЛНЕНИЕ ПОЛЕЙ ПРИ ВЫБОРЕ СТРОКИ
        // =========================================================

        // Когда пользователь выбирает проект, переносим его данные
        // из таблицы в поля редактирования.
        private void ProjectsGrid_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            Project project = ProjectsGrid.SelectedItem as Project;

            // Если выбор отсутствует, ничего не делаем.
            if (project == null)
            {
                return;
            }

            ProjectNameBox.Text = project.Name;
            ProjectDescriptionBox.Text = project.Description;
            ProjectStartDate.SelectedDate = project.StartDate;
            ProjectEndDate.SelectedDate = project.EndDate;

            // Ищем в списке пункт, текст которого совпадает со статусом.
            foreach (ComboBoxItem item in ProjectStatusBox.Items)
            {
                if (item.Content.ToString() == project.Status)
                {
                    ProjectStatusBox.SelectedItem = item;
                    break;
                }
            }
        }

        // При выборе сотрудника заполняем поля его данными.
        private void EmployeesGrid_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            Employee employee = EmployeesGrid.SelectedItem as Employee;

            if (employee == null)
            {
                return;
            }

            EmployeeNameBox.Text = employee.FullName;
            EmployeePositionBox.Text = employee.Position;
            EmployeeEmailBox.Text = employee.Email;
        }

        // При выборе задачи заполняем все поля её данными.
        private void TasksGrid_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            TaskItem task = TasksGrid.SelectedItem as TaskItem;

            if (task == null)
            {
                return;
            }

            TaskTitleBox.Text = task.Title;
            TaskDescriptionBox.Text = task.Description;
            TaskProgressBox.Text = task.Progress.ToString();
            TaskDueDate.SelectedDate = task.DueDate;

            // Выбираем проект по его идентификатору.
            TaskProjectBox.SelectedValue = task.ProjectId;

            // Выбираем сотрудника, если он назначен.
            // Если EmployeeId равен null, поле останется пустым.
            TaskEmployeeBox.SelectedValue = task.EmployeeId;

            // Устанавливаем статус задачи.
            foreach (ComboBoxItem item in TaskStatusBox.Items)
            {
                if (item.Content.ToString() == task.Status)
                {
                    TaskStatusBox.SelectedItem = item;
                    break;
                }
            }
        }
    // Проверяет, что ФИО состоит ровно из трёх слов.
// Каждое слово должно начинаться с заглавной буквы.
private bool IsValidFullName(string fullName)
        {
            // Убираем пробелы в начале и конце строки.
            fullName = fullName.Trim();

            // Разбиваем строку на слова по пробелам.
            // Несколько пробелов подряд не создают пустые слова.
            string[] words = Regex.Split(fullName, @"\s+");

            // ФИО должно содержать исключительно три слова.
            if (words.Length != 3)
            {
                return false;
            }

            // Проверяем каждое слово отдельно.
            foreach (string word in words)
            {
                // Первая буква должна быть заглавной,
                // остальные буквы — строчными.
                // Допускаем дефис в двойных именах и фамилиях.
                if (!Regex.IsMatch(
                    word,
                    @"^[А-ЯЁ][а-яё]*(?:-[А-ЯЁ][а-яё]*)*$"))
                {
                    return false;
                }
            }

            return true;
        }

        // Проверяет правильность формата электронной почты.
        private bool IsValidEmail(string email)
        {
            // Убираем лишние пробелы по краям.
            email = email.Trim();

            // Проверяем наличие имени, символа @ и домена.
            // Например: ivanov@mail.ru или ivan.petrov@example.com.
            return Regex.IsMatch(
                email,
                @"^[A-Za-z0-9.!#$%&'*+/=?^_`{|}~-]+@[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?(?:\.[A-Za-z0-9](?:[A-Za-z0-9-]*[A-Za-z0-9])?)+$");
        }
    }
}
