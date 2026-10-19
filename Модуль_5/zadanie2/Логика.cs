using System.IO;
using System.Text;
// Логика работы с текстом и файлами.
namespace TextEditorWPF
{
    public class TextEditorLogic
    {
        // Храним путь к текущему открытому файлу.
        // Если файл ещё не сохранён, значение равно null.
        public string CurrentFilePath { get; private set; } = null;
        // Храним текст, с которым сейчас работает редактор.
        public string CurrentText { get; private set; } = "";
        // Создаём новый пустой документ.
        public void NewDocument()
        {
            CurrentText = "";
            CurrentFilePath = null;
        }
        // Открываем файл и читаем его содержимое.
        public string OpenDocument(string path)
        {
            CurrentText = File.ReadAllText(path, Encoding.UTF8);
            CurrentFilePath = path;

            return CurrentText;
        }
        // Сохраняем документ по уже известному пути.
        public void SaveDocument(string text)
        {
            // Если путь неизвестен, сначала нужно выбрать имя файла.
            if (string.IsNullOrWhiteSpace(CurrentFilePath))
            {
                throw new System.InvalidOperationException(
                    "Сначала укажите путь для сохранения файла.");
            }
            // Записываем текст в файл в кодировке UTF-8.
            File.WriteAllText(CurrentFilePath, text, Encoding.UTF8);
            // Обновляем сохранённую копию текста.
            CurrentText = text;
        }
        // Сохраняем документ по новому пути.
        // Этот метод используется для команды «Сохранить как».
        public void SaveDocumentAs(string path, string text)
        {
            File.WriteAllText(path, text, Encoding.UTF8);
            // Запоминаем новый путь и сохранённый текст.
            CurrentFilePath = path;
            CurrentText = text;
        }
        // Проверяем, отличается ли текущий текст от сохранённого.
        public bool HasUnsavedChanges(string text)
        {
            return text != CurrentText;
        }
    }
}
