using System;
// Создаём интерфейс "Студент".
// Он определяет два обязательных действия: получить средний балл, получить курс студента.
interface IStudent
{
    // Метод возвращает средний балл.
    double GetAverageGrade();
    // Метод возвращает номер курса.
    int GetCourse();
}
// Создаём класс студента первого курса.
class FirstYearStudent : IStudent
{
    // Массив с оценками студента.
    private double[] grades;
    // Конструктор.
    // При создании студента передаём массив с его оценками.
    public FirstYearStudent(double[] grades)
    {
        this.grades = grades;
    }
    // Вычисляем средний балл.
    public double GetAverageGrade()
    {
        // Переменная для хранения суммы оценок.
        double sum = 0;
        // Перебираем все оценки в массиве.
        foreach (double grade in grades)
        {
            // Добавляем текущую оценку к общей сумме.
            sum += grade;
        }
        // Делим сумму всех оценок на количество оценок.
        return sum / grades.Length;
    }
    // Возвращаем курс студента.
    public int GetCourse()
    {
        // Этот класс предназначен для студентов первого курса.
        return 1;
    }
}
// Создаём класс студента второго курса.
class SecondYearStudent : IStudent
{
    // Массив оценок студента.
    private double[] grades;
    // Конструктор студента.
    public SecondYearStudent(double[] grades)
    {
        this.grades = grades;
    }
    // Вычисляем средний балл.
    public double GetAverageGrade()
    {
        // Переменная для суммы оценок.
        double sum = 0;
        // По очереди берём каждую оценку.
        foreach (double grade in grades)
        {
            // Прибавляем оценку к сумме.
            sum += grade;
        }
        // Находим среднее значение.
        return sum / grades.Length;
    }
    // Возвращаем курс.
    public int GetCourse()
    {
        // Этот класс представляет второго курса.
        return 2;
    }
}
// Главный класс программы.
class Program
{
    static void Main()
    {
        // Создаём студента первого курса.
        IStudent student1 =
            new FirstYearStudent(
                new double[] { 7, 8, 8 }
            );
        // Создаём студента второго курса.
        IStudent student2 =
            new SecondYearStudent(
                new double[] { 5, 7, 6 }
            );
        // Выводим информацию о первом студенте.
        Console.WriteLine("СТУДЕНТ 1");
        // Получаем номер курса.
        Console.WriteLine(
            $"Курс: {student1.GetCourse()}"
        );
        // Получаем средний балл.
        Console.WriteLine(
            $"Средний балл: {student1.GetAverageGrade():F2}"
        );
        // Выводим информацию о втором студенте.
        Console.WriteLine("\nСТУДЕНТ 2");
        // Получаем курс.
        Console.WriteLine(
            $"Курс: {student2.GetCourse()}"
        );
        // Получаем средний балл.
        Console.WriteLine(
            $"Средний балл: {student2.GetAverageGrade():F2}"
        );
    }
}
