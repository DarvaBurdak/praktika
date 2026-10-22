using System;
using System.Windows;
using System.Windows.Controls;
namespace ContactsWPF
{
    public partial class MainWindow : Window
    {
        // Создаём менеджер, который хранит контакты и выполняет операции добавления, изменения, удаления и поиска.
        private ContactManager manager = new ContactManager();
        // Хранит ID контакта, выбранного в таблице.
        // Nullable int (int?) позволяет хранить число или null.
        // null означает, что пользователь ещё не выбрал контакт.
        private int? selectedContactId = null;
        public MainWindow()
        {
            InitializeComponent();
            RefreshContacts();
        }
        // Обновляет содержимое таблицы.
        // Метод используется после добавления, изменения, удаления и поиска.
        private void RefreshContacts()
        {
            // Передаём таблице список контактов, подходящих под текущий поиск.
            // ItemsSource определяет источник данных для отображения строк.
            ContactsGrid.ItemsSource =
                manager.SearchContacts(txtSearch.Text);
        }
        // Обработчик нажатия кнопки «Добавить контакт».
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Получаем значения из текстовых полей.
                // Передаём их менеджеру, который создаёт новый контакт.
                manager.AddContact(
                    txtName.Text,
                    txtPhone.Text,
                    txtEmail.Text);
                // Перечитываем список и показываем новый контакт в таблице.
                RefreshContacts();
                // Очищаем поля для ввода следующего контакта.
                ClearFields();
                // Показываем пользователю сообщение
                txtMessage.Text = "Контакт успешно добавлен.";
            }
            catch (ArgumentException ex)
            {
                // Если имя не заполнено или возникла другая проверяемая ошибка, выводим понятное сообщение вместо аварийного завершения программы.
                MessageBox.Show(
                    ex.Message,
                    "Проверка данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
        // Обработчик кнопки «Редактировать».
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            // Сначала проверяем, выбран ли контакт для редактирования.
            if (selectedContactId == null)
            {
                // Предупреждаем пользователя, если строка не выбрана.
                MessageBox.Show(
                    "Сначала выберите контакт в таблице.",
                    "Контакт не выбран",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                // return прекращает выполнение текущего метода.
                return;
            }
            try
            {
                // Передаём ID выбранного контакта и новые данные менеджеру.
                // Value извлекает число из int?, предварительно проверенного на null.
                manager.UpdateContact(
                    selectedContactId.Value,
                    txtName.Text,
                    txtPhone.Text,
                    txtEmail.Text);
                // Обновляем таблицу, чтобы отобразить новые данные.
                RefreshContacts();
                // Очищаем поля и сбрасываем выбор.
                ClearFields();
                // Сообщаем об успешном изменении.
                txtMessage.Text = "Данные контакта обновлены.";
            }
            catch (ArgumentException ex)
            {
                // Выводим сообщение, если данные не прошли проверку.
                MessageBox.Show(
                    ex.Message,
                    "Ошибка редактирования",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }
        // Обработчик кнопки «Удалить».
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            // Нельзя удалить контакт, если пользователь ничего не выбрал.
            if (selectedContactId == null)
            {
                MessageBox.Show(
                    "Сначала выберите контакт в таблице.",
                    "Контакт не выбран",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
                return;
            }
            // Перед удалением запрашиваем подтверждение.
            MessageBoxResult result = MessageBox.Show(
                "Вы действительно хотите удалить этот контакт?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);
            // Если пользователь нажал «Нет», ничего не удаляем.
            if (result != MessageBoxResult.Yes)
            {
                return;
            }
            // Удаляем выбранный контакт по его уникальному ID.
            manager.DeleteContact(selectedContactId.Value);
            // Обновляем таблицу после удаления.
            RefreshContacts();
            // Убираем данные удалённого контакта из полей.
            ClearFields();
            // Показываем результат операции.
            txtMessage.Text = "Контакт удалён.";
        }
        // Обработчик кнопки «Очистить поля».
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            // Очищаем форму, не изменяя список контактов.
            ClearFields();
            // Отображаем информационное сообщение.
            txtMessage.Text = "Поля очищены.";
        }
        // Общий метод очистки полей.
        // Его вызывают несколько обработчиков, чтобы не дублировать код.
        private void ClearFields()
        {
            // Удаляем текст из поля идентификатора.
            txtId.Clear();
            // Удаляем имя.
            txtName.Clear();
            // Удаляем телефон.
            txtPhone.Clear();
            // Удаляем электронную почту.
            txtEmail.Clear();
            // Сбрасываем ID выбранного контакта.
            selectedContactId = null;
            // Снимаем выделение строки таблицы.
            ContactsGrid.SelectedItem = null;
        }
        // Обработчик изменения текста в поле поиска.
        // Срабатывает каждый раз, когда пользователь вводит или удаляет символ.
        private void txtSearch_TextChanged(
            object sender, TextChangedEventArgs e)
        {
            // Повторно фильтруем контакты и обновляем таблицу.
            RefreshContacts();
        }
        // Обработчик выбора строки в таблице контактов.
        private void ContactsGrid_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            // Получаем выбранный объект и преобразуем его к типу Contact.
            // Если ничего не выбрано, результатом будет null.
            Contact contact = ContactsGrid.SelectedItem as Contact;
            // Если строка не выбрана, заканчиваем обработчик.
            if (contact == null)
            {
                return;
            }
            // Запоминаем ID, чтобы знать, какую запись изменять или удалять.
            selectedContactId = contact.Id;
            // Переносим данные выбранного контакта в поля ввода.
            // Теперь пользователь может посмотреть и отредактировать их.
            txtId.Text = contact.Id.ToString();
            txtName.Text = contact.Name;
            txtPhone.Text = contact.Phone;
            txtEmail.Text = contact.Email;
            // Сообщаем, что контакт выбран.
            txtMessage.Text = "Выбран контакт: " + contact.Name;
        }
    }
}
