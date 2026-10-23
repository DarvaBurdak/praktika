using System;
namespace StudentsApp
{
    // Класс Student описывает одного студента.
    // Каждый объект этого класса содержит информацию об отдельном человеке.
    public class Student
    {
        // Уникальный номер студента в списке.
        // Он позволяет находить нужную запись для редактирования и удаления.
        public int Id { get; set; }
        // Имя и фамилия студента.
        public string FullName { get; set; }
        // Название группы, в которой учится студент.
        public string Group { get; set; }
        // Возраст студента.
        public int Age { get; set; }
        // Средний балл студента.
        public double AverageGrade { get; set; }
        // Конструктор создаёт нового студента и записывает переданные данные.
        public Student(int id, string fullName, string group,
                       int age, double averageGrade)
        {
            // Сохраняем уникальный номер.
            Id = id;
            // Сохраняем ФИО.
            FullName = fullName;
            // Сохраняем учебную группу.
            Group = group;
            // Сохраняем возраст.
            Age = age;
            // Сохраняем средний балл.
            AverageGrade = averageGrade;
        }
    }
}
