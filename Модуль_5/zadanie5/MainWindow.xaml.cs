using System;
using System.Windows;
using System.Windows.Controls;
namespace CalculatorWPF
{
    public partial class MainWindow : Window
    {
        // Создаём объект отдельного класса, в котором выполняются вычисления.
        // Все операции будут выполняться через этот объект.
        private CalculatorLogic calculator = new CalculatorLogic();
        public MainWindow()
        {
            InitializeComponent();
            // Показываем начальное значение калькулятора.
            UpdateDisplay();
        }
        // Обработчик нажатия цифровых кнопок.
        // Один метод используется для всех кнопок с цифрами.
        private void Digit_Click(object sender, RoutedEventArgs e)
        {
            // Получаем кнопку, на которую нажал пользователь.
            Button button = (Button)sender;
            // Content содержит цифру, написанную на кнопке.
            string digit = button.Content.ToString();
            // Передаём цифру классу логики.
            calculator.EnterDigit(digit);
            // Обновляем экран калькулятора.
            UpdateDisplay();
        }
        // Обработчик арифметических операций.
        private void Operator_Click(object sender, RoutedEventArgs e)
        {
            // Получаем нажатую кнопку.
            Button button = (Button)sender;
            // Считываем знак операции с кнопки.
            string operation = button.Content.ToString();
            // Переводим символы интерфейса в символы, которые использует класс CalculatorLogic.
            switch (operation)
            {
                case "+":
                    operation = "+";
                    break;
                case "−":
                    operation = "-";
                    break;
                case "×":
                    operation = "*";
                    break;
                case "÷":
                    operation = "/";
                    break;
            }
            // Передаём операцию отдельному классу.
            calculator.SetOperator(operation);
            // Обновляем экран.
            UpdateDisplay();
        }
        // Обработчик кнопки "=".
        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            // Просим класс логики выполнить вычисление.
            calculator.Calculate();
            // Показываем полученный результат.
            UpdateDisplay();
        }
        // Обработчик кнопки десятичного разделителя.
        private void Decimal_Click(object sender, RoutedEventArgs e)
        {
            // Передаём команду в класс логики.
            // Он сам проверит, можно ли добавить разделитель.
            calculator.EnterDecimalSeparator();
            // Обновляем экран.
            UpdateDisplay();
        }
        // Обработчик кнопки смены знака.
        private void Sign_Click(object sender, RoutedEventArgs e)
        {
            // Меняем знак текущего числа.
            calculator.ChangeSign();
            // Показываем обновлённое значение.
            UpdateDisplay();
        }
        // Обработчик кнопки очистки.
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            // Сбрасываем состояние калькулятора.
            calculator.Clear();
            // Показываем ноль.
            UpdateDisplay();
        }
        // Обработчик кнопки удаления последнего символа.
        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            // Удаляем последнюю цифру через класс логики.
            calculator.Backspace();
            // Обновляем отображение.
            UpdateDisplay();
        }
        // Единый метод обновления экрана.
        private void UpdateDisplay()
        {
            // Получаем текст, подготовленный классом логики, и отображаем его в поле txtDisplay.
            txtDisplay.Text = calculator.DisplayText;
        }
    }
}
