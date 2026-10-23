
using System;

namespace ProjectTasksWPF.Models
{
    // Класс описывает одну задачу проекта.
    public class TaskItem
    {
        // Уникальный номер задачи.
        public int Id { get; set; }

        // Название задачи.
        public string Title { get; set; } = "";

        // Подробное описание того, что нужно сделать.
        public string Description { get; set; } = "";

        // Идентификатор проекта, к которому относится задача.
        // По этому значению база данных связывает задачу с проектом.
        public int ProjectId { get; set; }

        // Идентификатор сотрудника, назначенного на задачу.
        // Значение null означает, что исполнитель пока не назначен.
        public int? EmployeeId { get; set; }

        // Плановая дата завершения задачи.
        public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);

        // Процент выполнения задачи: от 0 до 100.
        public int Progress { get; set; }

        // Статус задачи.
        // Например: «Не начата», «В работе», «Завершена».
        public string Status { get; set; } = "Не начата";

        // Название проекта для отображения в таблице.
        // Это поле заполняется при чтении задачи из базы данных.
        public string ProjectName { get; set; } = "";

        // Имя исполнителя для отображения в таблице.
        // Если сотрудник не назначен, поле будет содержать поясняющий текст.
        public string EmployeeName { get; set; } = "Не назначен";
    }
}
