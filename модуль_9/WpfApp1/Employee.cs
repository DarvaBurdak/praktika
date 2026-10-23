
namespace ProjectTasksWPF.Models
{
    // Класс описывает сотрудника, который может выполнять задачи.
    public class Employee
    {
        // Уникальный идентификатор сотрудника.
        public int Id { get; set; }

        // Фамилия, имя или полное имя сотрудника.
        public string FullName { get; set; } = "";

        // Должность сотрудника.
        public string Position { get; set; } = "";

        // Электронная почта сотрудника.
        public string Email { get; set; } = "";

        // При выборе сотрудника в ComboBox
        // будет отображаться его имя, а не название класса.
        public override string ToString()
        {
            return FullName;
        }
    }
}
