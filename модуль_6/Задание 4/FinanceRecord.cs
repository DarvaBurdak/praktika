
namespace WpfApp1
{
    // Описывает одну финансовую операцию.
    public class FinanceRecord
    {
        // Уникальный номер записи в базе данных.
        public int Id { get; set; }

        // Тип операции: Доход или Расход.
        public string Type { get; set; }

        // Название или категория операции.
        public string Category { get; set; }

        // Сумма операции.
        public decimal Amount { get; set; }

        // Дата операции.
        public System.DateTime Date { get; set; }

        // Дополнительный комментарий.
        public string Description { get; set; }
    }
}
