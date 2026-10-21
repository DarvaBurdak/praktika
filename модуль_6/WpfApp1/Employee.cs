namespace EmployeesWPF
{
    // Класс описывает одного сотрудника.
    // Объект этого класса будет соответствовать одной записи в базе данных.
    public class Employee
    {
        // Уникальный номер сотрудника в базе данных.
        public int Id { get; set; }

        // Фамилия сотрудника.
        public string LastName { get; set; } = "";

        // Имя сотрудника.
        public string FirstName { get; set; } = "";

        // Должность сотрудника.
        public string Position { get; set; } = "";

        // Отдел, в котором работает сотрудник.
        public string Department { get; set; } = "";

        // Контактный телефон.
        public string Phone { get; set; } = "";

        // Размер заработной платы.
        public decimal Salary { get; set; }
    }
}
