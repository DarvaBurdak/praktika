using System; 
using System.IO; 
using System.Windows; 
using System.Windows.Media.Imaging; 
namespace ImageViewerWPF 
{
    public partial class MainWindow : Window 
    {
        // Создаём объект класса ImageViewerLogic.
        // Этот класс отвечает за расчёт масштаба и проверку файлов.
        // readonly означает, что ссылку на объект нельзя заменить после инициализации.
        private readonly ImageViewerLogic imageLogic =
            new ImageViewerLogic();
        public MainWindow()
        {
            InitializeComponent();
            // Показываем сообщение, пока пользователь не открыл изображение.
            txtFileInfo.Text = "Изображение не выбрано";
            // Показываем прочерк вместо размеров, так как изображения пока нет.
            txtImageSize.Text = "Размер: —";
            // Выводим текущий масштаб изображения.
            // По умолчанию он равен 1.0, то есть 100%.
            UpdateZoomText();
        }
        // Метод обработки нажатия кнопки открытия изображения.
        private void OpenImage_Click(object sender, RoutedEventArgs e)
        {
            // Создаём стандартное диалоговое окно Windows для выбора файла.
            Microsoft.Win32.OpenFileDialog openDialog =
                new Microsoft.Win32.OpenFileDialog();
            // Устанавливаем заголовок диалогового окна.
            openDialog.Title = "Выберите изображение";
            // Указываем форматы файлов, которые будут показаны в списке.
            openDialog.Filter =
                "Файлы изображений|*.jpg;*.jpeg;*.png;*.bmp;*.gif;*.tif;*.tiff|" +
                "Все файлы|*.*";
            // Показываем диалоговое окно и сохраняем результат.
            // true означает, что пользователь выбрал файл и подтвердил выбор.
            // false или null означает, что выбор не был подтверждён.
            bool? result = openDialog.ShowDialog();
            // Если пользователь отменил выбор, завершаем метод.
            // Прежнее изображение при этом остаётся на экране.
            if (result != true)
                return;
            // Получаем полный путь к выбранному файлу.
            string filePath = openDialog.FileName;
            // Начинаем блок обработки возможных ошибок.
            try
            {
                // Проверяем существование файла и допустимость его расширения.
                if (!imageLogic.IsImageFile(filePath))
                {
                    // Выводим предупреждение, если файл не подходит.
                    MessageBox.Show(
                        "Выберите существующий файл изображения.",
                        "Ошибка",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    // Прекращаем открытие неподходящего файла.
                    return;
                }
                // Создаём объект BitmapImage для загрузки изображения.
                BitmapImage bitmap = new BitmapImage();
                // Начинаем настройку загрузки изображения.
                bitmap.BeginInit();
                // Загружаем изображение целиком в память.
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                // Указываем полный путь к выбранному файлу.
                // UriKind.Absolute означает, что путь является абсолютным.
                bitmap.UriSource = new Uri(filePath, UriKind.Absolute);
                // Завершаем загрузку и применяем заданные параметры.
                bitmap.EndInit();
                // Запрещаем дальнейшее изменение объекта BitmapImage.
                bitmap.Freeze();
                // Устанавливаем загруженное изображение в элемент imgPreview.
                // Этот элемент отображает картинку в главном окне.
                imgPreview.Source = bitmap;
                // Сбрасываем масштаб на 100% при открытии нового изображения.
                imageLogic.ActualSize();
                // Применяем установленный масштаб к изображению.
                ApplyZoom();
                // Обновляем имя файла, размеры изображения и заголовок окна.
                UpdateImageInfo(filePath, bitmap);
                // Автоматически подбираем масштаб, чтобы изображение целиком помещалось в доступную область просмотра.
                FitToWindow_Click(null, null);
            }
            catch (Exception ex) // Выполняется, если при открытии произошла ошибка.
            {
                // Показываем сообщение об ошибке.
                MessageBox.Show(
                    "Не удалось открыть изображение.\n\n" +
                    // Добавляем описание причины ошибки.
                    "Проверьте, что файл существует и не повреждён.\n\n" +
                    "Подробности: " + ex.Message,
                    "Ошибка",
                    MessageBoxButton.OK, 
                    MessageBoxImage.Error); 
            }
        }
        // Метод обработки нажатия кнопки приближения изображения.
        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            // Увеличиваем масштаб на 20%.
            // Метод ZoomIn сам ограничивает масштаб значением MaxZoom.
            imageLogic.ZoomIn();
            // Применяем новый масштаб и обновляем его отображение.
            ApplyZoom();
        }
        // Метод обработки нажатия кнопки отдаления изображения.
        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            // Уменьшаем масштаб на один шаг.
            // Метод ZoomOut не позволяет опуститься ниже MinZoom.
            imageLogic.ZoomOut();
            // Применяем рассчитанный масштаб.
            ApplyZoom();
        }
        // Метод обработки кнопки возврата к исходному размеру.
        private void ActualSize_Click(object sender, RoutedEventArgs e)
        {
            // Устанавливаем масштаб 1.0, то есть 100%.
            imageLogic.ActualSize();
            // Обновляем отображение изображения.
            ApplyZoom();
        }
        // Метод подгонки изображения под видимую область окна.
        private void FitToWindow_Click(object sender, RoutedEventArgs e)
        {
            // Проверяем, загружено ли изображение.
            // Если источник пустой, выполнять расчёты не нужно.
            if (imgPreview.Source == null)
                return;
            // Обновляем разметку WPF, чтобы получить актуальные размеры области прокрутки после изменения интерфейса.
            imageScrollViewer.UpdateLayout();
            // Получаем ширину исходного изображения.
            // Это размер изображения в единицах WPF, а не обязательно в пикселях.
            double imageWidth = imgPreview.Source.Width;
            // Получаем высоту исходного изображения.
            double imageHeight = imgPreview.Source.Height;
            // Получаем ширину видимой области прокрутки.
            double availableWidth = imageScrollViewer.ViewportWidth;
            // Получаем высоту видимой области прокрутки.
            double availableHeight = imageScrollViewer.ViewportHeight;
            // Передаём размеры изображения и области просмотра в отдельный класс для расчёта подходящего масштаба.
            imageLogic.FitToWindow(
                imageWidth,
                imageHeight,
                availableWidth,
                availableHeight);
            // Применяем рассчитанный масштаб к изображению.
            ApplyZoom();
        }
        // Метод применения масштаба к отображаемому изображению.
        private void ApplyZoom()
        {
            // Изменяем масштаб по горизонтали.
            // Значение берётся из логического класса.
            imageScale.ScaleX = imageLogic.CurrentZoom;
            // Изменяем масштаб по вертикали.
            // Используем такое же значение, чтобы сохранить пропорции.
            imageScale.ScaleY = imageLogic.CurrentZoom;
            // Обновляем надпись с процентом увеличения.
            UpdateZoomText();
            // Пересчитываем размеры содержимого и полос прокрутки.
            // Это необходимо, если изображение стало больше области просмотра.
            imageScrollViewer.UpdateLayout();
        }
        // Метод обновления текста, показывающего масштаб изображения.
        private void UpdateZoomText()
        {
            // Умножаем коэффициент масштаба на 100.
            // Например, 1.5 превращается в 150%.
            // Math.Round округляет значение до ближайшего целого.
            int percent = (int)Math.Round(
                imageLogic.CurrentZoom * 100);
            // Выводим рассчитанный процент на экран.
            txtZoom.Text = $"Масштаб: {percent}%";
        }
        // Метод отображения информации об открытом изображении.
        // Принимает путь к файлу и загруженный объект изображения.
        private void UpdateImageInfo(
            string filePath,
            BitmapImage bitmap)
        {
            // Из полного пути извлекаем только имя файла с расширением.
            txtFileInfo.Text = Path.GetFileName(filePath);
            // Показываем исходную ширину и высоту изображения в пикселях.
            // Символ × используется для разделения ширины и высоты.
            txtImageSize.Text =
                $"Размер: {bitmap.PixelWidth} × {bitmap.PixelHeight} пикс.";
            // Меняем заголовок главного окна.
            // В заголовке показывается название программы и имя файла.
            Title =
                $"Просмотр изображений — {Path.GetFileName(filePath)}";
        }
        // Метод обработки команды выхода из программы.
        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            // Закрываем главное окно приложения.
            Close();
        }
    }
}
