using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
namespace TextEditorWPF
{
    // Главный класс окна текстового редактора. partial связывает код C# с разметкой MainWindow.xaml.
    public partial class MainWindow : Window
    {
        // Объект, который управляет документом и файлами.
        private readonly TextEditorLogic editorLogic = new TextEditorLogic();
        // Флаг нужен, чтобы не считать программную загрузку файла обычным редактированием текста пользователем.
        private bool isLoadingFile = false;
        // Флаг показывает, что закрытие уже проверено через Exit_Click.
        private bool closeAlreadyConfirmed = false;
        public MainWindow()
        {
            InitializeComponent();
            UpdateWindowTitle();
        }
        // Проверяем, есть ли несохранённые изменения.
        private bool IsTextChanged()
        {
            return editorLogic.HasUnsavedChanges(txtEditor.Text);
        }
        // Метод обновляет заголовок окна и строку состояния.
        private void UpdateWindowTitle()
        {
            // Получаем путь к текущему файлу из логического класса.
            string currentFilePath = editorLogic.CurrentFilePath;
            // Если файл не сохранён, показываем "Новый документ".
            string fileName = string.IsNullOrWhiteSpace(currentFilePath)
                ? "Новый документ"
                : Path.GetFileName(currentFilePath);
            // Звёздочка показывает несохранённые изменения.
            string changedMark = IsTextChanged() ? "*" : "";
            // Обновляем заголовок окна.
            Title = fileName + changedMark + " — Текстовый редактор";
            // Обновляем строку состояния.
            txtStatus.Text = string.IsNullOrWhiteSpace(currentFilePath)
                ? "Новый документ" + (IsTextChanged() ? " — есть изменения" : "")
                : currentFilePath + (IsTextChanged() ? " — есть изменения" : " — сохранён");
        }
        // Обработчик события изменения текста.
        private void txtEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            // При загрузке файла не обновляем состояние документа.
            if (isLoadingFile)
                return;
            UpdateWindowTitle();
        }
        // Спрашиваем пользователя, что делать с несохранёнными изменениями.
        // true означает, что можно продолжать действие.
        // false означает, что пользователь отменил действие.
        private bool ConfirmSaveChanges()
        {
            // Если текст совпадает с сохранённым, подтверждение не требуется.
            if (!IsTextChanged())
                return true;
            MessageBoxResult result = MessageBox.Show(
                "Документ был изменён. Сохранить изменения?",
                "Несохранённые изменения",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);
            // Если пользователь выбрал Yes, сохраняем документ.
            if (result == MessageBoxResult.Yes)
                return SaveDocument();
            // Если выбран No, продолжаем без сохранения.
            if (result == MessageBoxResult.No)
                return true;
            // Cancel: остаёмся в текущем документе.
            return false;
        }
        // Обработчик пункта меню "Создать".
        private void New_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmSaveChanges())
                return;
            // Создаём новый пустой документ в логическом классе.
            editorLogic.NewDocument();
            // Очищаем поле редактора, не считая это пользовательским вводом.
            isLoadingFile = true;
            txtEditor.Clear();
            isLoadingFile = false;
            UpdateWindowTitle();
        }
        // Обработчик пункта меню "Открыть".
        private void Open_Click(object sender, RoutedEventArgs e)
        {
            if (!ConfirmSaveChanges())
                return;
            OpenFileDialog dialog = new OpenFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                Title = "Открыть текстовый файл"
            };
            if (dialog.ShowDialog() == true)
            {
                try
                {
                    // Читаем файл через логический класс.
                    string text = editorLogic.OpenDocument(dialog.FileName);
                    // Показываем содержимое файла в редакторе.
                    isLoadingFile = true;
                    txtEditor.Text = text;
                    isLoadingFile = false;
                    UpdateWindowTitle();
                }
                catch (Exception ex)
                {
                    isLoadingFile = false;
                    MessageBox.Show(
                        "Не удалось открыть файл.\n" + ex.Message,
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                }
            }
        }
        // Обработчик пункта меню "Сохранить".
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveDocument();
        }
        // Обработчик пункта меню "Сохранить как".
        private void SaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveDocumentAs();
        }
        // Сохраняем документ в текущий файл.
        // Возвращаем true, если сохранение выполнено успешно.
        private bool SaveDocument()
        {
            // Если путь ещё не задан, открываем диалог "Сохранить как".
            if (string.IsNullOrWhiteSpace(editorLogic.CurrentFilePath))
                return SaveDocumentAs();
            try
            {
                // Сохраняем текст через логический класс.
                editorLogic.SaveDocument(txtEditor.Text);

                UpdateWindowTitle();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось сохранить файл.\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }
        }
        // Сохраняем документ, предварительно предлагая выбрать путь.
        private bool SaveDocumentAs()
        {
            SaveFileDialog dialog = new SaveFileDialog
            {
                Filter = "Текстовые файлы (*.txt)|*.txt|Все файлы (*.*)|*.*",
                FileName = string.IsNullOrWhiteSpace(editorLogic.CurrentFilePath)
                    ? "Документ.txt"
                    : Path.GetFileName(editorLogic.CurrentFilePath),
                Title = "Сохранить текстовый файл"
            };
            // Если пользователь отменил диалог, ничего не сохраняем.
            if (dialog.ShowDialog() != true)
                return false;
            try
            {
                // Сохраняем текст по выбранному пути через логический класс.
                editorLogic.SaveDocumentAs(dialog.FileName, txtEditor.Text);
                UpdateWindowTitle();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Не удалось сохранить файл.\n" + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return false;
            }
        }
        // Обработчик пункта меню "Выход".
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем изменения только здесь.
            if (!ConfirmSaveChanges())
                return;
            // OnClosing не должен повторно задавать тот же вопрос.
            closeAlreadyConfirmed = true;
            Close();
        }
        // Обработчик закрытия окна, например крестиком в заголовке.
        protected override void OnClosing(
            System.ComponentModel.CancelEventArgs e)
        {
            // При закрытии через крестик проверяем изменения.
            if (!closeAlreadyConfirmed && !ConfirmSaveChanges())
            {
                e.Cancel = true;
            }
            base.OnClosing(e);
        }
    }
}
