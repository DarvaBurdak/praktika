using System; 
using System.Globalization;
namespace CalculatorWPF 
{
    // Класс содержит логику вычислений калькулятора.
    // Он отвечает за ввод чисел, выполнение операций и формирование результата.
    public class CalculatorLogic
    {
        // Хранит текущее число, введённое пользователем или полученное после вычисления.
        // Тип decimal подходит для арифметических операций с десятичными числами.
        private decimal currentValue = 0;
        // Хранит первое число, участвующее в арифметической операции.
        private decimal firstValue = 0;
        // Хранит знак выбранной операции: "+", "-", "*" или "/".
        // Пока операция не выбрана, строка пустая.
        private string currentOperator = "";
        // Определяет, нужно ли начинать ввод нового числа.
        // true означает, что следующая цифра заменит содержимое дисплея.
        // false означает, что следующая цифра добавится к текущему числу.
        private bool startNewNumber = true;
        // Хранит текст, который отображается на экране калькулятора.
        // Начальное значение — "0".
        // private set позволяет читать свойство снаружи класса, но изменять его можно только внутри CalculatorLogic.
        public string DisplayText { get; private set; } = "0";
        // Свойство для получения десятичного разделителя.
        private string DecimalSeparator
        {
            get
            {
                // Получаем символ десятичного разделителя из текущих региональных настроек системы.
                return CultureInfo.CurrentCulture.NumberFormat
                    .NumberDecimalSeparator;
            }
        }
        // Метод ввода одной цифры.
        // Параметр digit содержит цифру, переданную из обработчика кнопки.
        public void EnterDigit(string digit)
        {
            // Проверяем, нужно ли начинать ввод нового числа.
            // Например, это происходит после выбора операции.
            if (startNewNumber)
            {
                // Заменяем содержимое дисплея введённой цифрой.
                DisplayText = digit;
                // Сообщаем, что новое число уже начало вводиться.
                startNewNumber = false;
            }
            else
            {
                // Если на дисплее находится только ноль,
                // заменяем его новой цифрой, чтобы не появлялось "05".
                if (DisplayText == "0")
                {
                    DisplayText = digit;
                }
                else
                {
                    // Если число уже введено, добавляем цифру справа.
                    // Например, после "1" и "2" получится "12".
                    DisplayText += digit;
                }
            }
            // Обновляем числовое значение на основе текста дисплея.
            UpdateCurrentValue();
        }
        // Метод ввода десятичного разделителя.
        public void EnterDecimalSeparator()
        {
            // Если начинается ввод нового числа, сразу отображаем ноль и десятичный разделитель.
            if (startNewNumber)
            {
                DisplayText = "0" + DecimalSeparator;
                // Следующие цифры будут добавляться к этому числу.
                startNewNumber = false;
                // Обновляем числовое значение.
                UpdateCurrentValue();

                // Завершаем метод, чтобы не выполнять следующие проверки.
                return;
            }

            // Проверяем, есть ли уже десятичный разделитель в числе.
            // Оператор ! означает отрицание: условие выполняется,
            // если разделитель ещё отсутствует.
            if (!DisplayText.Contains(DecimalSeparator))
            {
                // Добавляем разделитель в конец отображаемого числа.
                DisplayText += DecimalSeparator;
            }

            // Синхронизируем числовое значение с текстом дисплея.
            UpdateCurrentValue();
        }

        // Метод выбора арифметической операции.
        // Например, пользователь нажимает кнопку "+".
        public void SetOperator(string operation)
        {
            // Сохраняем текущее число как первое число операции.
            firstValue = currentValue;
            // Запоминаем выбранный знак операции.
            currentOperator = operation;
            // Следующая введённая цифра начнёт новое число.
            startNewNumber = true;
        }

        // Метод выполнения арифметического вычисления.
        public void Calculate()
        {
            // Проверяем, выбрана ли операция.
            // Если currentOperator пустая, вычисление не выполняется.
            if (string.IsNullOrEmpty(currentOperator))
            {
                return;
            }

            // Выбираем действие в зависимости от сохранённого оператора.
            switch (currentOperator)
            {
                case "+":
                    // Складываем первое число и текущее число.
                    currentValue = firstValue + currentValue;
                    break;
                case "-":
                    // Вычитаем текущее число из первого.
                    currentValue = firstValue - currentValue;
                    break;
                case "*":
                    // Умножаем первое число на текущее.
                    currentValue = firstValue * currentValue;
                    break;

                case "/":
                    // Перед делением проверяем, не равен ли делитель нулю.
                    if (currentValue == 0)
                    {
                        // Вместо результата выводим сообщение об ошибке.
                        DisplayText = "Ошибка: деление на 0";
                        // Сбрасываем сохранённую операцию.
                        currentOperator = "";
                        // Следующий ввод должен начать новое число.
                        startNewNumber = true;
                        // Сбрасываем текущее числовое значение.
                        currentValue = 0;
                        // Прерываем метод, чтобы деление не выполнялось.
                        return;
                    }
                    // Если делитель не равен нулю, выполняем деление.
                    currentValue = firstValue / currentValue;
                    break;
            }
            // Преобразуем результат в строку и выводим на дисплей.
            // Форматирование выполняется отдельным методом FormatNumber.
            DisplayText = FormatNumber(currentValue);
            // Сохраняем результат в качестве первого числа следующей операции.
            // Это позволяет продолжить вычисления с полученным результатом.
            firstValue = currentValue;
            // После вычисления следующий ввод начнёт новое число.
            startNewNumber = true;
            // Очищаем оператор, так как текущая операция завершена.
            currentOperator = "";
        }
        // Метод изменения знака текущего числа.
        // Например, 5 превращается в -5, а -5 — в 5.
        public void ChangeSign()
        {
            // Умножение на -1 меняет знак числа.
            currentValue = -currentValue;
            // Обновляем текст дисплея с учётом нового знака.
            DisplayText = FormatNumber(currentValue);
            // Разрешаем продолжить ввод или редактирование числа.
            startNewNumber = false;
        }
        // Метод удаления последнего символа на дисплее.
        public void Backspace()
        {
            // Если ожидается ввод нового числа, ничего не удаляем.
            if (startNewNumber)
            {
                return;
            }
            // Если строка содержит не больше одного символа
            // или на экране отображается сообщение об ошибке,
            // возвращаем дисплей к нулю.
            if (DisplayText.Length <= 1 ||
                DisplayText.StartsWith("Ошибка", StringComparison.Ordinal))
            {
                DisplayText = "0";
            }
            else
            {
                // Удаляем последний символ строки.
                // Первый аргумент — начало фрагмента, второй — его длина.
                DisplayText = DisplayText.Substring(
                    0,
                    DisplayText.Length - 1);
                // Если после удаления остался только знак минуса,
                // заменяем его нулём, чтобы не оставлять некорректное число.
                if (DisplayText == "-")
                {
                    DisplayText = "0";
                }
            }
            // Пересчитываем числовое значение после удаления символа.
            UpdateCurrentValue();
        }
        // Метод полной очистки калькулятора.
        public void Clear()
        {
            // Сбрасываем текущее число.
            currentValue = 0;
            // Сбрасываем первое число операции.
            firstValue = 0;
            // Удаляем сохранённый знак операции.
            currentOperator = "";
            // Устанавливаем режим ввода нового числа.
            startNewNumber = true;
            // Возвращаем дисплей к начальному значению.
            DisplayText = "0";
        }
        // Вспомогательный метод преобразования текста дисплея в число.
        // Он нужен, чтобы currentValue соответствовало введённому тексту.
        private void UpdateCurrentValue()
        {
            // TryParse пытается преобразовать строку в число decimal.
            // NumberStyles.Number разрешает обычную запись числа, включая десятичный разделитель и знак.
            // CurrentCulture учитывает региональные настройки компьютера.
            // Если преобразование успешно, результат сохраняется в parsedValue.
            if (decimal.TryParse(
                DisplayText,
                NumberStyles.Number,
                CultureInfo.CurrentCulture,
                out decimal parsedValue))
            {
                // Обновляем текущее число только при успешном преобразовании.
                currentValue = parsedValue;
            }
        }
        // Вспомогательный метод форматирования числа для дисплея.
        private string FormatNumber(decimal number)
        {
            // Преобразуем число в строку с учётом региональных настроек.
            // Формат "G" выводит число в компактном виде,
            // не добавляя ненужные нули в конце дробной части.
            return number.ToString("G", CultureInfo.CurrentCulture);
        }
    }
}
