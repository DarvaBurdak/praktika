
using System;
using System.Windows;
using WpfApp1;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        // Объект для выполнения операций с базой библиотеки.
        private readonly LibraryDatabase database;

        public MainWindow()
        {
            InitializeComponent();

            // Создаём базу и загружаем каталог при запуске.
            database = new LibraryDatabase();
            LoadBooks();
        }

        // Показывает книги в таблице.
        private void LoadBooks(string search = "")
        {
            try
            {
                BooksGrid.ItemsSource = null;
                BooksGrid.ItemsSource = database.GetBooks(search);

                txtMessage.Text = "Книги загружены.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки книг:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Ищет книги по названию или автору.
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            LoadBooks(txtSearch.Text.Trim());
        }

        // Добавляет новую книгу в каталог.
        private void AddBook_Click(object sender, RoutedEventArgs e)
        {
            string title = txtTitle.Text.Trim();
            string author = txtAuthor.Text.Trim();

            int year;
            int copies;

            // Проверяем обязательные поля.
            if (string.IsNullOrWhiteSpace(title) ||
                string.IsNullOrWhiteSpace(author))
            {
                MessageBox.Show("Введите название книги и автора.");
                return;
            }

            if (!int.TryParse(txtYear.Text, out year) ||
                year < 0 || year > DateTime.Today.Year)
            {
                MessageBox.Show("Введите корректный год издания.");
                return;
            }

            if (!int.TryParse(txtCopies.Text, out copies) || copies < 1)
            {
                MessageBox.Show("Количество экземпляров должно быть больше нуля.");
                return;
            }

            try
            {
                // Создаём объект книги из данных формы.
                var book = new Book
                {
                    Title = title,
                    Author = author,
                    Year = year,
                    Genre = txtGenre.Text.Trim(),
                    TotalCopies = copies,
                    AvailableCopies = copies
                };

                database.AddBook(book);

                LoadBooks(txtSearch.Text.Trim());

                // Очищаем поля после добавления.
                txtTitle.Clear();
                txtAuthor.Clear();
                txtYear.Clear();
                txtGenre.Clear();
                txtCopies.Text = "1";

                txtMessage.Text = "Книга добавлена в каталог.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось добавить книгу:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Арендует один экземпляр выбранной книги.
        private void Rent_Click(object sender, RoutedEventArgs e)
        {
            Book selectedBook = BooksGrid.SelectedItem as Book;

            if (selectedBook == null)
            {
                MessageBox.Show("Сначала выберите книгу в таблице.");
                return;
            }

            try
            {
                if (database.RentBook(selectedBook.Id))
                {
                    LoadBooks(txtSearch.Text.Trim());
                    txtMessage.Text = "Аренда зарегистрирована.";
                }
                else
                {
                    MessageBox.Show(
                        "Нет доступных экземпляров этой книги.",
                        "Аренда невозможна",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка аренды:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Возвращает ранее арендованный экземпляр.
        private void Return_Click(object sender, RoutedEventArgs e)
        {
            Book selectedBook = BooksGrid.SelectedItem as Book;

            if (selectedBook == null)
            {
                MessageBox.Show("Сначала выберите книгу в таблице.");
                return;
            }

            try
            {
                if (database.ReturnBook(selectedBook.Id))
                {
                    LoadBooks(txtSearch.Text.Trim());
                    txtMessage.Text = "Возврат книги зарегистрирован.";
                }
                else
                {
                    MessageBox.Show(
                        "Для этой книги нет активной аренды.",
                        "Возврат невозможен",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка возврата:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Повторно загружает весь каталог.
        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            txtSearch.Clear();
            LoadBooks();
        }
    }
}
