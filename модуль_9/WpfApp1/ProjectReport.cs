
namespace ProjectTasksWPF.Models
{
    // Класс содержит итоговые показатели выполнения проекта.
    // Эти данные используются для формирования отчёта.
    public class ProjectReport
    {
        // Название проекта.
        public string ProjectName { get; set; } = "";

        // Текущий статус проекта.
        public string Status { get; set; } = "";

        // Общее количество задач проекта.
        public int TotalTasks { get; set; }

        // Количество завершённых задач.
        public int CompletedTasks { get; set; }

        // Количество задач, которые ещё не завершены.
        public int RemainingTasks { get; set; }

        // Средний процент выполнения задач проекта.
        public double AverageProgress { get; set; }
    }
}
