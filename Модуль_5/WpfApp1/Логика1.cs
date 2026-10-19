using System;
using System.Windows;
// Логика выбора инструмента и расчёта размеров фигур.
namespace DrawingAppWPF
{
    public class DrawingLogic
    {
        // Название инструмента, выбранного пользователем.
        // По умолчанию рисуем линию.
        public string SelectedTool { get; private set; } = "Line";
        // Запоминаем начальную точку рисования.
        public Point StartPoint { get; private set; }
        // Устанавливаем инструмент: линия, круг или квадрат.
        public void SelectTool(string tool)
        {
            SelectedTool = tool;
        }
        // Запоминаем место, в котором пользователь начал рисовать.
        public void SetStartPoint(Point point)
        {
            StartPoint = point;
        }
        // Рассчитываем геометрию фигуры по начальной и текущей точкам.
        // Возвращаем координаты и размеры в виде массива: [X, Y, Width, Height].
        public double[] CalculateBounds(Point currentPoint)
        {
            double x = Math.Min(StartPoint.X, currentPoint.X);
            double y = Math.Min(StartPoint.Y, currentPoint.Y);
            double width = Math.Abs(currentPoint.X - StartPoint.X);
            double height = Math.Abs(currentPoint.Y - StartPoint.Y);
            // Для квадрата делаем ширину и высоту одинаковыми.
            if (SelectedTool == "Square")
            {
                double side = Math.Min(width, height);
                width = side;
                height = side;
                // Корректируем начальную позицию, чтобы квадрат располагался в направлении движения мыши.
                if (currentPoint.X < StartPoint.X)
                    x = StartPoint.X - side;
                if (currentPoint.Y < StartPoint.Y)
                    y = StartPoint.Y - side;
            }
            // Для круга также используем одинаковую ширину и высоту.
            if (SelectedTool == "Circle")
            {
                double diameter = Math.Min(width, height);
                width = diameter;
                height = diameter;
                if (currentPoint.X < StartPoint.X)
                    x = StartPoint.X - diameter;

                if (currentPoint.Y < StartPoint.Y)
                    y = StartPoint.Y - diameter;
            }
            return new double[] { x, y, width, height };
        }
    }
}
