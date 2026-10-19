using System.Collections.ObjectModel;
using System.Linq;
namespace TaskManagerWPF
{
    // Класс отвечает за логику работы с задачами.
    // Он не зависит от элементов интерфейса WPF.
    public class TaskManagerLogic
    {
        // Коллекция всех задач.
        public ObservableCollection<TaskItem> Tasks { get; }
            = new ObservableCollection<TaskItem>();
        // Добавление новой задачи.
        public void AddTask(string taskText)
        {
            if (string.IsNullOrWhiteSpace(taskText))
                return;
            Tasks.Add(new TaskItem(taskText.Trim()));
        }
        // Удаление выбранной задачи.
        public bool DeleteTask(TaskItem task)
        {
            if (task == null || !Tasks.Contains(task))
                return false;
            Tasks.Remove(task);
            return true;
        }
        // Удаление всех выполненных задач.
        public void DeleteCompletedTasks()
        {
            // Создаём копию списка, чтобы безопасно удалять элементы.
            var completedTasks = Tasks
                .Where(task => task.IsCompleted)
                .ToList();
            foreach (TaskItem task in completedTasks)
            {
                Tasks.Remove(task);
            }
        }
        // Удаление всех задач.
        public void ClearTasks()
        {
            Tasks.Clear();
        }
        // Общее количество задач.
        public int GetTotalCount()
        {
            return Tasks.Count;
        }
        // Количество выполненных задач.
        public int GetCompletedCount()
        {
            return Tasks.Count(task => task.IsCompleted);
        }
        // Количество невыполненных задач.
        public int GetRemainingCount()
        {
            return GetTotalCount() - GetCompletedCount();
        }
    }
}
