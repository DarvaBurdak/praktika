using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
namespace DrawingAppWPF
{
    // Класс главного окна приложения.
    public partial class MainWindow : Window
    {
        // Объект, отвечающий за логику рисования.
        private DrawingLogic drawingLogic = new DrawingLogic();
        // Координаты точки, в которой пользователь начал рисование.
        private Point startPoint;
        // Фигура, которую пользователь рисует в данный момент.
        private Shape currentShape;
        // Показывает, выполняется ли сейчас рисование.
        private bool isDrawing = false;
        // Конструктор главного окна.
        public MainWindow()
        {
            InitializeComponent();
        }
        // Выбор инструмента "Линия".
        private void btnLine_Click(object sender, RoutedEventArgs e)
        {
            drawingLogic.SelectTool("Line");
        }
        // Выбор инструмента "Круг".
        private void btnCircle_Click(object sender, RoutedEventArgs e)
        {
            drawingLogic.SelectTool("Circle");
        }
        // Выбор инструмента "Квадрат".
        private void btnSquare_Click(object sender, RoutedEventArgs e)
        {
            drawingLogic.SelectTool("Square");
        }
        // Очистка холста.
        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            // Удаляем все фигуры с холста.
            canvasDrawing.Children.Clear();

            // Сбрасываем текущую фигуру.
            currentShape = null;

            // Завершаем рисование.
            isDrawing = false;

            // Освобождаем мышь.
            canvasDrawing.ReleaseMouseCapture();
        }
        // Событие нажатия левой кнопки мыши на холсте.
        private void canvasDrawing_MouseLeftButtonDown(
            object sender, MouseButtonEventArgs e)
        {
            // Получаем координаты начала рисования.
            startPoint = e.GetPosition(canvasDrawing);
            // Сохраняем начальную точку в логике программы.
            drawingLogic.SetStartPoint(startPoint);
            // Создаём фигуру в зависимости от выбранного инструмента.
            if (drawingLogic.SelectedTool == "Line")
            {
                // Создаём линию.
                currentShape = new Line
                {
                    X1 = startPoint.X,
                    Y1 = startPoint.Y,
                    X2 = startPoint.X,
                    Y2 = startPoint.Y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2
                };
            }
            else if (drawingLogic.SelectedTool == "Circle")
            {
                // Создаём круг с прозрачной заливкой.
                currentShape = new Ellipse
                {
                    Width = 0,
                    Height = 0,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    Fill = Brushes.Transparent
                };
            }
            else if (drawingLogic.SelectedTool == "Square")
            {
                // Создаём квадрат с прозрачной заливкой.
                currentShape = new Rectangle
                {
                    Width = 0,
                    Height = 0,
                    Stroke = Brushes.Black,
                    StrokeThickness = 2,
                    Fill = Brushes.Transparent
                };
            }

            // Если фигура успешно создана, добавляем её на холст.
            if (currentShape != null)
            {
                canvasDrawing.Children.Add(currentShape);
                // Включаем режим рисования.
                isDrawing = true;
                // Захватываем мышь, чтобы отслеживать её движение.
                canvasDrawing.CaptureMouse();
                // Обновляем фигуру по текущим координатам мыши.
                UpdateShape(e.GetPosition(canvasDrawing));
                // Помечаем событие как обработанное.
                e.Handled = true;
            }
        }
        // Событие перемещения мыши.
        private void canvasDrawing_MouseMove(
            object sender, MouseEventArgs e)
        {
            // Обновляем фигуру, только если рисование началось.
            if (isDrawing && currentShape != null)
            {
                Point currentPoint = e.GetPosition(canvasDrawing);

                UpdateShape(currentPoint);
            }
        }
        // Событие отпускания левой кнопки мыши.
        private void canvasDrawing_MouseLeftButtonUp(
            object sender, MouseButtonEventArgs e)
        {
            // Если рисование не выполняется, ничего не делаем.
            if (!isDrawing)
                return;
            // Устанавливаем окончательные размеры фигуры.
            if (currentShape != null)
            {
                UpdateShape(e.GetPosition(canvasDrawing));
            }
            // Завершаем рисование текущей фигуры.
            isDrawing = false;
            currentShape = null;
            // Освобождаем мышь.
            canvasDrawing.ReleaseMouseCapture();
            e.Handled = true;
        }
        // Изменение размеров и положения создаваемой фигуры.
        private void UpdateShape(Point currentPoint)
        {
            // Если фигура не создана, обновлять нечего.
            if (currentShape == null)
                return;
            // Рассчитываем расстояние от начальной точки до курсора.
            double dx = currentPoint.X - startPoint.X;
            double dy = currentPoint.Y - startPoint.Y;
            // Если рисуется линия, изменяем её конечную точку.
            if (currentShape is Line line)
            {
                line.X1 = startPoint.X;
                line.Y1 = startPoint.Y;
                line.X2 = currentPoint.X;
                line.Y2 = currentPoint.Y;
            }
            // Если рисуется круг, рассчитываем его диаметр.
            else if (currentShape is Ellipse circle)
            {
                // Используем меньшее расстояние, чтобы получить круг,
                // а не эллипс.
                double diameter = Math.Min(
                    Math.Abs(dx),
                    Math.Abs(dy));
                // Определяем левый верхний угол круга.
                double left = dx < 0
                    ? startPoint.X - diameter
                    : startPoint.X;
                double top = dy < 0
                    ? startPoint.Y - diameter
                    : startPoint.Y;
                // Устанавливаем одинаковые размеры.
                circle.Width = diameter;
                circle.Height = diameter;
                // Устанавливаем положение круга на холсте.
                Canvas.SetLeft(circle, left);
                Canvas.SetTop(circle, top);
            }
            // Если рисуется квадрат, рассчитываем длину его стороны.
            else if (currentShape is Rectangle square)
            {
                // Выбираем большее расстояние, чтобы квадрат сохранял одинаковую ширину и высоту.
                double side = Math.Max(
                    Math.Abs(dx),
                    Math.Abs(dy));
                // Определяем положение квадрата.
                double left = dx < 0
                    ? startPoint.X - side
                    : startPoint.X;
                double top = dy < 0
                    ? startPoint.Y - side
                    : startPoint.Y;
                // Задаём одинаковую ширину и высоту.
                square.Width = side;
                square.Height = side;
                // Устанавливаем квадрат на холсте.
                Canvas.SetLeft(square, left);
                Canvas.SetTop(square, top);
            }
        }
    }
}