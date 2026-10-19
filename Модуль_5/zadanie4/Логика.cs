using System;
using System.IO;
namespace ImageViewerWPF 
{
    // Класс содержит логику работы с изображениями.
    // Он отделяет вычисления от интерфейса MainWindow.
    public class ImageViewerLogic
    {
        // Минимальный допустимый масштаб изображения — 10%.
        // Значение 0.1 означает, что изображение отображается в десять раз меньше исходного.
        // const означает, что значение является постоянным и не изменяется во время работы программы.
        public const double MinZoom = 0.1;
        // Максимальный допустимый масштаб изображения — 500%.
        // Значение 5.0 означает увеличение в пять раз.
        public const double MaxZoom = 5.0;
        // Хранит текущий масштаб изображения.
        // private set означает, что прочитать свойство можно извне,
        // но изменять его значение можно только внутри этого класса.
        public double CurrentZoom { get; private set; } = 1.0;
        // Метод увеличения изображения.
        // Возвращает новый коэффициент масштабирования.
        public double ZoomIn()
        {
            // Умножаем текущий масштаб на 1.2, увеличивая его на 20%.
            // Math.Min выбирает меньшее из двух значений: рассчитанный масштаб или максимальный допустимый масштаб.
            CurrentZoom = Math.Min(CurrentZoom * 1.2, MaxZoom);
            // Возвращаем полученное значение масштаба.
            return CurrentZoom;
        }
        // Метод уменьшения изображения.
        public double ZoomOut()
        {
            // Делим текущий масштаб на 1.2.
            // Math.Max выбирает большее значение: рассчитанный масштаб или минимальный допустимый масштаб.
            CurrentZoom = Math.Max(CurrentZoom / 1.2, MinZoom);
            // Возвращаем новый масштаб.
            return CurrentZoom;
        }
        // Метод возвращения изображения к исходному размеру.
        public double ActualSize()
        {
            // Устанавливаем коэффициент 1.0, то есть 100%.
            CurrentZoom = 1.0;
            // Возвращаем установленный масштаб.
            return CurrentZoom;
        }
        // Метод расчёта масштаба, при котором изображение помещается в окно.
        // imageWidth — исходная ширина изображения.
        // imageHeight — исходная высота изображения.
        // availableWidth — ширина доступной области просмотра.
        // availableHeight — высота доступной области просмотра.
        public double FitToWindow(
            double imageWidth,
            double imageHeight,
            double availableWidth,
            double availableHeight)
        {
            // Проверяем, что размеры изображения и области просмотра положительные.
            // Если хотя бы один размер равен нулю или меньше нуля, правильный масштаб рассчитать нельзя.
            if (imageWidth <= 0 || imageHeight <= 0 ||
                availableWidth <= 0 || availableHeight <= 0)
            {
                // Возвращаем прежний масштаб, не изменяя его.
                return CurrentZoom;
            }
            // Вычисляем масштаб, при котором изображение поместится по ширине.
            // Например, если ширина окна вдвое меньше изображения, коэффициент будет равен 0.5.
            double zoomWidth = availableWidth / imageWidth;
            // Вычисляем масштаб, при котором изображение поместится по высоте.
            double zoomHeight = availableHeight / imageHeight;
            // Выбираем меньший масштаб, чтобы изображение целиком поместилось одновременно по ширине и высоте.
            // Math.Min(..., 1.0) не позволяет автоматически увеличить маленькое изображение выше исходного размера.
            CurrentZoom = Math.Min(
                Math.Min(zoomWidth, zoomHeight), 1.0);
            // Не допускаем масштаб меньше 10%.
            CurrentZoom = Math.Max(CurrentZoom, MinZoom);
            // Возвращаем рассчитанный масштаб.
            return CurrentZoom;
        }
        // Метод проверки выбранного файла изображения.
        // Принимает полный путь к файлу и возвращает true или false.
        public bool IsImageFile(string filePath)
        {
            // Проверяем, не является ли путь пустым или состоящим из пробелов.
            // File.Exists проверяет, существует ли файл по указанному пути.
            // Если путь пустой или файл отсутствует, возвращаем false.
            if (string.IsNullOrWhiteSpace(filePath) ||
                !File.Exists(filePath))
            {
                return false;
            }
            // Получаем расширение файла, например ".jpg" или ".png".
            // Path.GetExtension извлекает расширение из пути.
            // ToLowerInvariant переводит буквы расширения в нижний регистр, чтобы ".JPG" и ".jpg" обрабатывались одинаково.
            string extension = Path.GetExtension(filePath).ToLowerInvariant();
            // Сравниваем расширение с поддерживаемыми форматами.
            // Оператор || означает «ИЛИ».
            // Если хотя бы одно условие истинно, метод возвращает true.
            // Иначе возвращается false.
            return extension == ".jpg" ||
                   extension == ".jpeg" ||
                   extension == ".png" ||
                   extension == ".bmp" ||
                   extension == ".gif" ||
                   extension == ".tif" ||
                   extension == ".tiff";
        }
    }
}
