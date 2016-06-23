using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Factorie
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int LINE_THICKNESS = 2;
        private Brush COLOR_LINE = Brushes.Black;
        private Brush COLOR_CIRCLE = Brushes.LightBlue;
        private Brush COLOR_FIRST_COMPANY = Brushes.Green;
        private Brush COLOR_SECOND_COMPANY = Brushes.Red;


        public MainWindow()
        {
            this.InitializeComponent();

            // Insert code required on object creation below this point.
        }

        private void Count_Clicked(object sender, System.Windows.RoutedEventArgs e)
        {
            Factories f = new Factories();
            f.AssignDelivery();
            field.Children.RemoveRange(0, field.Children.Count);

            for (int row = 0; row < f.LengthRow; row++)
                for (int column = 0; column < f.LengthColumn; column++)
                    if (f.MatchingMtx[row, column])
                        DrawLine(1250 * row / (f.LengthRow - 1) + 45, 500,
                            1250 * column / (f.LengthColumn - 1) + 45, 100,
                            false, Brushes.Black);



            for (int row = 0; row < f.LengthRow; row++)
                DrawCircle(1250 * row / (f.LengthRow - 1) + 45, 500, 10);



            for (int column = 0; column < f.LengthColumn; column++)
                DrawCircle(1250 * column / (f.LengthColumn - 1) + 45, 100, 10);

            textblock.Text = "Время доставки = " + f.TotalTime().ToString();

        }


        private void DrawCircle(int cX, int cY, int radius)
        {
            Ellipse el = new Ellipse();
            el.Width = 2 * radius;
            el.Height = 2 * radius;
            el.VerticalAlignment = VerticalAlignment.Top;
            el.Fill = COLOR_CIRCLE;
            el.Stroke = COLOR_LINE;
            el.StrokeThickness = LINE_THICKNESS;
            Canvas.SetLeft(el, cX - radius);
            Canvas.SetTop(el, cY - radius);
            field.Children.Add(el);
        }


        private void DrawLine(int x1, int y1, int x2, int y2, bool fict, Brush color)
        {
            Line ln = new Line();
            ln.X1 = x1;
            ln.Y1 = y1;
            ln.X2 = x2;
            ln.Y2 = y2;
            ln.Stroke = color;
            ln.StrokeThickness = LINE_THICKNESS;
            if (fict)
                ln.StrokeDashArray = new DoubleCollection() { 2, 0, 2 };
            field.Children.Add(ln);
        }



    }
}