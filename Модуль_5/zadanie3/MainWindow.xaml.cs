using System.Collections.Specialized;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
namespace TaskManagerWPF
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        // Объект, который управляет логикой задач.
        private readonly TaskManagerLogic taskLogic = new TaskManagerLogic();
        // Коллекция задач для привязки к интерфейсу.
        public System.Collections.ObjectModel.ObservableCollection<TaskItem> Tasks
        {
            get { return taskLogic.Tasks; }
        }
        // Хранит задачу, выбранную пользователем.
        private TaskItem selectedTask;
        public TaskItem SelectedTask
        {
            get { return selectedTask; }
            set
            {
                if (selectedTask == value)
                    return;

                selectedTask = value;
                OnPropertyChanged();
            }
        }
        // Конструктор главного окна.
        public MainWindow()
        {
            InitializeComponent();
            // Передаём данные в разметку XAML.
            DataContext = this;
            // Подписываемся на изменения списка задач.
            Tasks.CollectionChanged += Tasks_CollectionChanged;
            // Показываем начальную статистику.
            UpdateStatistics();
        }
        // Выполняется при добавлении или удалении задач.
        private void Tasks_CollectionChanged(
            object sender,
            NotifyCollectionChangedEventArgs e)
        {
            // Подписываемся на изменения новых задач.
            if (e.NewItems != null)
            {
                foreach (TaskItem task in e.NewItems)
                {
                    task.PropertyChanged += Task_PropertyChanged;
                }
            }
            // Отписываемся от удалённых задач.
            if (e.OldItems != null)
            {
                foreach (TaskItem task in e.OldItems)
                {
                    task.PropertyChanged -= Task_PropertyChanged;
                }
            }
            // Обновляем статистику.
            UpdateStatistics();
        }
        // Выполняется при изменении отдельной задачи.
        private void Task_PropertyChanged(
            object sender,
            PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(TaskItem.IsCompleted))
            {
                UpdateStatistics();
            }
        }
        // Добавление новой задачи.
        private void AddTask_Click(object sender, RoutedEventArgs e)
        {
            string taskText = txtTask.Text.Trim();
            // Проверяем, введён ли текст задачи.
            if (string.IsNullOrWhiteSpace(taskText))
            {
                MessageBox.Show(
                    "Введите текст задачи.",
                    "Менеджер задач",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }
            // Передаём добавление задачи классу логики.
            taskLogic.AddTask(taskText);
            // Очищаем поле ввода.
            txtTask.Clear();
            // Возвращаем курсор в поле ввода.
            txtTask.Focus();
        }
        // Удаление выбранной задачи.
        private void DeleteTask_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedTask == null)
            {
                MessageBox.Show(
                    "Сначала выберите задачу в списке.",
                    "Менеджер задач",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                return;
            }
            // Передаём удаление задачи классу логики.
            taskLogic.DeleteTask(SelectedTask);
            // Сбрасываем выбор.
            SelectedTask = null;
        }
        // Удаление всех выполненных задач.
        private void DeleteCompleted_Click(object sender, RoutedEventArgs e)
        {
            // Передаём удаление выполненных задач классу логики.
            taskLogic.DeleteCompletedTasks();
            // Сбрасываем выбор, если выбранная задача была удалена.
            if (SelectedTask != null && !Tasks.Contains(SelectedTask))
            {
                SelectedTask = null;
            }
        }
        // Полная очистка списка задач.
        private void ClearTasks_Click(object sender, RoutedEventArgs e)
        {
            if (Tasks.Count == 0)
            {
                MessageBox.Show(
                    "Список задач уже пуст.",
                    "Менеджер задач",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }
            MessageBoxResult result = MessageBox.Show(
                "Удалить все задачи?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Передаём очистку списка классу логики.
                taskLogic.ClearTasks();
                // Сбрасываем выбранную задачу.
                SelectedTask = null;
            }
        }
        // Обновление статистики в интерфейсе.
        private void UpdateStatistics()
        {
            int total = taskLogic.GetTotalCount();
            int completed = taskLogic.GetCompletedCount();
            int remaining = taskLogic.GetRemainingCount();
            txtStatistics.Text =
                $"Всего задач: {total}\n" +
                $"Выполнено: {completed}\n" +
                $"Осталось: {remaining}";
        }
        // Уведомляет интерфейс об изменении свойства.
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
    // Класс описывает одну задачу.
    public class TaskItem : INotifyPropertyChanged
    {
        // Текст задачи.
        public string TaskText { get; set; }
        // Состояние выполнения задачи.
        private bool isCompleted;
        public bool IsCompleted
        {
            get { return isCompleted; }
            set
            {
                if (isCompleted == value)
                    return;
                isCompleted = value;
                OnPropertyChanged();
            }
        }
        // Конструктор задачи.
        public TaskItem(string taskText)
        {
            TaskText = taskText;
            IsCompleted = false;
        }
        // Уведомление интерфейса об изменениях.
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(
            [CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
