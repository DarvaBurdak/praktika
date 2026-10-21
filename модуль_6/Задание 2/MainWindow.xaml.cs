using System;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1 // Замени, если у нового проекта другое пространство имён
{
    public partial class MainWindow : Window
    {
        // Объект для взаимодействия с базой данных SQLite.
        private TaskDatabase database;

        // Конструктор главного окна.
        public MainWindow()
        {
            InitializeComponent();

            // Создаём объект базы данных.
            // Если файл базы отсутствует, он будет создан автоматически.
            database = new TaskDatabase();

            // По умолчанию устанавливаем сегодняшнюю дату.
            dpDueDate.SelectedDate = DateTime.Today;

            // Загружаем задачи при открытии программы.
            LoadTasks();
        }

        // Загружает задачи из базы и отображает их в таблице.
        private void LoadTasks()
        {
            try
            {
                TasksGrid.ItemsSource = null;
                TasksGrid.ItemsSource = database.GetAllTasks();

                // Обновляем количество задач в заголовке.
                txtTaskCount.Text = "Задач: " +
                    TasksGrid.Items.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось загрузить задачи.\n" + ex.Message,
                    "Ошибка базы данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Проверяет поля и формирует объект новой задачи.
        private bool TryGetTaskFromFields(out TaskItem task)
        {
            task = null;

            // Название обязательно для заполнения.
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show(
                    "Введите название задачи.",
                    "Проверка данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                txtTitle.Focus();
                return false;
            }

            // Проверяем, выбрана ли дата.
            if (!dpDueDate.SelectedDate.HasValue)
            {
                MessageBox.Show(
                    "Выберите срок выполнения задачи.",
                    "Проверка данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            // Получаем выбранный статус из выпадающего списка.
            ComboBoxItem selectedItem =
                cmbStatus.SelectedItem as ComboBoxItem;

            if (selectedItem == null)
            {
                MessageBox.Show("Выберите статус задачи.");
                return false;
            }

            // Создаём объект задачи из введённых данных.
            task = new TaskItem
            {
                Title = txtTitle.Text.Trim(),
                Description = txtDescription.Text.Trim(),
                DueDate = dpDueDate.SelectedDate.Value,
                Status = selectedItem.Content.ToString()
            };

            return true;
        }

        // Добавляет новую задачу в базу данных.
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            TaskItem task;

            // Если данные заполнены неправильно, ничего не сохраняем.
            if (!TryGetTaskFromFields(out task))
                return;

            try
            {
                // Сохраняем задачу в SQLite.
                database.AddTask(task);

                // Обновляем таблицу.
                LoadTasks();

                // Очищаем поля после успешного добавления.
                ClearFields();

                txtMessage.Text = "Задача успешно добавлена.";

                MessageBox.Show(
                    "Задача добавлена в базу данных.",
                    "Успешно",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось добавить задачу.\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Изменяет данные выбранной задачи.
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранную строку таблицы.
            TaskItem selectedTask = TasksGrid.SelectedItem as TaskItem;

            if (selectedTask == null)
            {
                MessageBox.Show(
                    "Сначала выберите задачу в таблице.",
                    "Изменение задачи",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            TaskItem updatedTask;

            if (!TryGetTaskFromFields(out updatedTask))
                return;

            try
            {
                // Сохраняем идентификатор выбранной задачи.
                updatedTask.Id = selectedTask.Id;

                // Записываем изменения в базу данных.
                database.UpdateTask(updatedTask);

                // Перезагружаем таблицу.
                LoadTasks();
                ClearFields();

                txtMessage.Text = "Изменения задачи сохранены.";

                MessageBox.Show(
                    "Задача успешно обновлена.",
                    "Успешно",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось обновить задачу.\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Удаляет выбранную задачу.
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            TaskItem selectedTask = TasksGrid.SelectedItem as TaskItem;

            if (selectedTask == null)
            {
                MessageBox.Show(
                    "Выберите задачу, которую нужно удалить.",
                    "Удаление задачи",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            // Спрашиваем подтверждение перед удалением.
            MessageBoxResult result = MessageBox.Show(
                "Удалить задачу «" + selectedTask.Title + "»?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            // Если пользователь передумал, прекращаем удаление.
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                // Удаляем запись по уникальному номеру.
                database.DeleteTask(selectedTask.Id);

                // Обновляем отображение.
                LoadTasks();
                ClearFields();

                txtMessage.Text = "Задача удалена.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось удалить задачу.\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Заполняет поля данными задачи при выборе строки таблицы.
        private void TasksGrid_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            TaskItem selectedTask = TasksGrid.SelectedItem as TaskItem;

            if (selectedTask == null)
                return;

            // Переносим данные из таблицы в поля формы.
            txtTitle.Text = selectedTask.Title;
            txtDescription.Text = selectedTask.Description;
            dpDueDate.SelectedDate = selectedTask.DueDate;

            // Находим и выбираем соответствующий статус.
            foreach (ComboBoxItem item in cmbStatus.Items)
            {
                if (item.Content.ToString() == selectedTask.Status)
                {
                    cmbStatus.SelectedItem = item;
                    break;
                }
            }
        }

        // Очищает поля ввода.
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            txtMessage.Text = "Поля очищены.";
        }

        // Общий метод очистки формы.
        private void ClearFields()
        {
            txtTitle.Clear();
            txtDescription.Clear();

            // Возвращаем сегодняшнюю дату.
            dpDueDate.SelectedDate = DateTime.Today;

            // Устанавливаем статус «Новая».
            cmbStatus.SelectedIndex = 0;

            // Снимаем выделение с таблицы.
            TasksGrid.SelectedItem = null;
        }
    }
}
