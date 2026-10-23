
using System;

namespace ProjectTasksWPF.Models
{
    // Класс описывает один проект.
    // Каждый объект Project соответствует одной записи в таблице Projects.
    public class Project
    {
        // Уникальный номер проекта в базе данных.
        // Он назначается автоматически при добавлении проекта.
        public int Id { get; set; }

        // Название проекта.
        public string Name { get; set; } = "";

        // Подробное описание проекта.
        public string Description { get; set; } = "";

        // Дата начала проекта.
        public DateTime StartDate { get; set; } = DateTime.Today;

        // Плановая дата завершения проекта.
        public DateTime EndDate { get; set; } = DateTime.Today.AddDays(30);

        // Текущий статус проекта:
        // например, «Не начат», «В работе» или «Завершён».
        public string Status { get; set; } = "Не начат";

        // Этот текст будет отображаться в ComboBox,
        // когда пользователь выбирает проект для задачи.
        public override string ToString()
        {
            return Name;
        }
    }
}
