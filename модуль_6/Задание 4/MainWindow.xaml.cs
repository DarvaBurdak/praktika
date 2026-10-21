
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        // Объект для работы с базой данных.
        private readonly FinanceDatabase database;

        // Конструктор главного окна.
        public MainWindow()
        {
            InitializeComponent();

            // Создаём объект базы данных.
            database = new FinanceDatabase();

            // По умолчанию выбираем сегодняшнюю дату.
            dpDate.SelectedDate = DateTime.Today;

            // Загружаем записи и рассчитываем баланс.
            LoadRecords();
        }

        // Загружает записи и обновляет общие суммы.
        private void LoadRecords()
        {
            try
            {
                // Получаем все записи из базы данных.
                var records = database.GetAllRecords();

                // Считаем все доходы.
                decimal income = records
                    .Where(r => r.Type == "Доход")
                    .Sum(r => r.Amount);

                // Считаем все расходы.
                decimal expense = records
                    .Where(r => r.Type == "Расход")
                    .Sum(r => r.Amount);

                // Обновляем карточки с финансовыми показателями.
                // Отображаем доходы в белорусских рублях.
                txtIncome.Text = income.ToString("N2") + " Br";

                // Отображаем расходы в белорусских рублях.
                txtExpense.Text = expense.ToString("N2") + " Br";

                // Рассчитываем и отображаем баланс в белорусских рублях.
                txtBalance.Text = (income - expense).ToString("N2") + " Br";

                // Выполняем поиск по категории и описанию.
                string search = txtSearch.Text.Trim();

                var filteredRecords = records
                    .Where(r =>
                        r.Category.IndexOf(
                            search, StringComparison.OrdinalIgnoreCase) >= 0
                        ||
                        r.Description.IndexOf(
                            search, StringComparison.OrdinalIgnoreCase) >= 0)
                    .ToList();

                // Показываем отфильтрованные записи в таблице.
                FinanceGrid.ItemsSource = null;
                FinanceGrid.ItemsSource = filteredRecords;

                txtMessage.Text = "Записей в списке: " +
                    filteredRecords.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось загрузить финансовые записи.\n" + ex.Message,
                    "Ошибка базы данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Получает данные из полей формы и проверяет их.
        private bool TryGetRecordFromFields(out FinanceRecord record)
        {
            record = null;

            // Проверяем категорию.
            if (string.IsNullOrWhiteSpace(txtCategory.Text))
            {
                MessageBox.Show("Введите категорию операции.");
                txtCategory.Focus();
                return false;
            }

            // Проверяем сумму. Поддерживаются запятая и точка.
            decimal amount;
            string amountText = txtAmount.Text.Trim();

            bool validAmount =
                decimal.TryParse(
                    amountText,
                    NumberStyles.Number,
                    CultureInfo.CurrentCulture,
                    out amount)
                ||
                decimal.TryParse(
                    amountText,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out amount);

            if (!validAmount || amount <= 0)
            {
                MessageBox.Show(
                    "Введите корректную сумму больше нуля.");
                txtAmount.Focus();
                return false;
            }

            // Проверяем дату.
            if (!dpDate.SelectedDate.HasValue)
            {
                MessageBox.Show("Выберите дату операции.");
                return false;
            }

            // Получаем выбранный тип операции.
            ComboBoxItem selectedType =
                cmbType.SelectedItem as ComboBoxItem;

            if (selectedType == null)
            {
                MessageBox.Show("Выберите тип операции.");
                return false;
            }

            // Создаём объект финансовой записи.
            record = new FinanceRecord
            {
                Type = selectedType.Content.ToString(),
                Category = txtCategory.Text.Trim(),
                Amount = amount,
                Date = dpDate.SelectedDate.Value,
                Description = txtDescription.Text.Trim()
            };

            return true;
        }

        // Добавляет новую запись.
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            FinanceRecord record;

            if (!TryGetRecordFromFields(out record))
                return;

            try
            {
                // Сохраняем запись в базе данных.
                database.AddRecord(record);

                // Обновляем таблицу и баланс.
                LoadRecords();

                // Очищаем форму после добавления.
                ClearFields();

                txtMessage.Text = "Финансовая запись добавлена.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось добавить запись.\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Редактирует выбранную запись.
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            // Получаем выбранную строку таблицы.
            FinanceRecord selected =
                FinanceGrid.SelectedItem as FinanceRecord;

            if (selected == null)
            {
                MessageBox.Show(
                    "Выберите запись в таблице для редактирования.");
                return;
            }

            FinanceRecord updatedRecord;

            if (!TryGetRecordFromFields(out updatedRecord))
                return;

            try
            {
                // Сохраняем идентификатор существующей записи.
                updatedRecord.Id = selected.Id;

                // Записываем изменения в базу.
                database.UpdateRecord(updatedRecord);

                LoadRecords();
                ClearFields();

                txtMessage.Text = "Финансовая запись обновлена.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось изменить запись.\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Удаляет выбранную запись.
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            FinanceRecord selected =
                FinanceGrid.SelectedItem as FinanceRecord;

            if (selected == null)
            {
                MessageBox.Show("Выберите запись для удаления.");
                return;
            }

            // Запрашиваем подтверждение удаления.
            MessageBoxResult answer = MessageBox.Show(
                "Удалить выбранную финансовую запись?",
                "Подтверждение",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (answer != MessageBoxResult.Yes)
                return;

            try
            {
                database.DeleteRecord(selected.Id);

                LoadRecords();
                ClearFields();

                txtMessage.Text = "Запись удалена.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось удалить запись.\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Заполняет поля при выборе записи в таблице.
        private void FinanceGrid_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            FinanceRecord selected =
                FinanceGrid.SelectedItem as FinanceRecord;

            if (selected == null)
                return;

            txtCategory.Text = selected.Category;
            txtAmount.Text = selected.Amount.ToString(
                CultureInfo.CurrentCulture);
            txtDescription.Text = selected.Description;
            dpDate.SelectedDate = selected.Date;

            // Выбираем тип операции из списка.
            foreach (ComboBoxItem item in cmbType.Items)
            {
                if (item.Content.ToString() == selected.Type)
                {
                    cmbType.SelectedItem = item;
                    break;
                }
            }
        }

        // Выполняет поиск по категории и описанию.
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            LoadRecords();
        }

        // Очищает поля формы.
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            txtMessage.Text = "Поля очищены.";
        }

        // Общий метод очистки формы.
        private void ClearFields()
        {
            txtCategory.Clear();
            txtAmount.Clear();
            txtDescription.Clear();

            dpDate.SelectedDate = DateTime.Today;

            // Возвращаем тип «Доход».
            cmbType.SelectedIndex = 0;

            // Снимаем выделение с таблицы.
            FinanceGrid.SelectedItem = null;
        }
    }
}
