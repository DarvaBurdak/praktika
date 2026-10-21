
using EmployeesWPF;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        // Объект для работы с базой данных.
        private readonly EmployeeDatabase database;

        // Список сотрудников, который отображается в DataGrid.
        private readonly ObservableCollection<Employee> employees =
            new ObservableCollection<Employee>();

        public MainWindow()
        {
            // Создаём интерфейс, описанный в MainWindow.xaml.
            InitializeComponent();

            // Подключаемся к базе данных.
            database = new EmployeeDatabase();

            // Загружаем сохранённые записи.
            LoadEmployees();
        }

        // Загружает сотрудников и обновляет таблицу.
        private void LoadEmployees()
        {
            try
            {
                // Получаем актуальные данные из базы.
                var employeeList = database.GetAllEmployees();

                // Обновляем коллекцию, не создавая новую.
                employees.Clear();

                foreach (Employee employee in employeeList)
                {
                    employees.Add(employee);
                }

                // Подключаем коллекцию к таблице только один раз.
                if (EmployeesGrid.ItemsSource == null)
                {
                    EmployeesGrid.ItemsSource = employees;
                }

                txtStatus.Text =
                    "Количество сотрудников: " + employees.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Ошибка загрузки сотрудников:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Проверяет поля формы и создаёт объект сотрудника.
        private bool TryReadEmployee(out Employee employee)
        {
            employee = new Employee();

            string lastName = txtLastName.Text.Trim();
            string firstName = txtFirstName.Text.Trim();
            string position = txtPosition.Text.Trim();
            string department = txtDepartment.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string salaryText = txtSalary.Text.Trim();

            // Все поля должны быть заполнены.
            if (string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(position) ||
                string.IsNullOrWhiteSpace(department) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(salaryText))
            {
                MessageBox.Show(
                    "Заполните все поля.",
                    "Проверка данных",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            // Разрешаем ввод зарплаты через точку или запятую.
            string normalizedSalary = salaryText.Replace(',', '.');

            decimal salary;

            if (!decimal.TryParse(
                normalizedSalary,
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out salary) || salary < 0)
            {
                MessageBox.Show(
                    "Введите корректную зарплату, например 45000 или 45000,50.",
                    "Ошибка ввода",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return false;
            }

            // Переносим введённые значения в объект.
            employee.LastName = lastName;
            employee.FirstName = firstName;
            employee.Position = position;
            employee.Department = department;
            employee.Phone = phone;
            employee.Salary = salary;

            return true;
        }

        // Кнопка «Добавить».
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            Employee employee;

            if (!TryReadEmployee(out employee))
                return;

            try
            {
                database.AddEmployee(employee);

                LoadEmployees();
                ClearFields();

                txtStatus.Text = "Сотрудник успешно добавлен.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось добавить сотрудника:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Кнопка «Сохранить изменения».
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            // Получаем сотрудника, выбранного в таблице.
            Employee selectedEmployee =
                EmployeesGrid.SelectedItem as Employee;

            if (selectedEmployee == null)
            {
                MessageBox.Show(
                    "Сначала выберите сотрудника в таблице.",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            Employee editedEmployee;

            if (!TryReadEmployee(out editedEmployee))
                return;

            try
            {
                // Сохраняем ID, чтобы обновить правильную запись.
                editedEmployee.Id = selectedEmployee.Id;

                database.UpdateEmployee(editedEmployee);

                LoadEmployees();
                ClearFields();

                txtStatus.Text = "Изменения сохранены.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось сохранить изменения:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // Кнопка «Удалить».
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Employee selectedEmployee =
                EmployeesGrid.SelectedItem as Employee;

            if (selectedEmployee == null)
            {
                MessageBox.Show(
                    "Выберите сотрудника в таблице.",
                    "Внимание",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            MessageBoxResult result = MessageBox.Show(
                "Удалить сотрудника " +
                selectedEmployee.LastName + " " +
                selectedEmployee.FirstName + "?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                database.DeleteEmployee(selectedEmployee.Id);

                LoadEmployees();
                ClearFields();

                txtStatus.Text = "Сотрудник удалён.";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось удалить сотрудника:\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        // При выборе строки таблицы заполняем поля формы.
        private void EmployeesGrid_SelectionChanged(
            object sender, SelectionChangedEventArgs e)
        {
            Employee employee =
                EmployeesGrid.SelectedItem as Employee;

            if (employee == null)
                return;

            txtLastName.Text = employee.LastName;
            txtFirstName.Text = employee.FirstName;
            txtPosition.Text = employee.Position;
            txtDepartment.Text = employee.Department;
            txtPhone.Text = employee.Phone;

            txtSalary.Text = employee.Salary.ToString(
                "0.##", CultureInfo.CurrentCulture);
        }

        // Кнопка «Очистить поля».
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearFields();
            txtStatus.Text = "Поля очищены.";
        }

        // Очищает форму и снимает выделение с таблицы.
        private void ClearFields()
        {
            txtLastName.Clear();
            txtFirstName.Clear();
            txtPosition.Clear();
            txtDepartment.Clear();
            txtPhone.Clear();
            txtSalary.Clear();

            EmployeesGrid.SelectedItem = null;
        }
    }
}
