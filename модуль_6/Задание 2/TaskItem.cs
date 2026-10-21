using System;
namespace WpfApp1 // Замени на пространство имён своего нового проекта
{
    // Класс описывает одну задачу, которую хранит база данных.
    public class TaskItem
    {
        // Уникальный номер задачи в базе данных.
        public int Id { get; set; }

        // Название задачи.
        public string Title { get; set; }

        // Подробное описание задачи.
        public string Description { get; set; }

        // Срок выполнения задачи.
        public DateTime DueDate { get; set; }

        // Статус задачи: Новая, В работе или Выполнена.
        public string Status { get; set; }
    }
}
