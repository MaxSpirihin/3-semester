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

namespace CitiesAndWays
{
    /// <summary>
    /// Задача о приватизации
    /// дата 15.10.14
    /// Строк - 400
    /// Время написания - 6 часов.
    /// </summary>
    public partial class MainWindow : Window
    {
        Graph graph = null;
        private int WIDTH;
        private int HEIGHT;
        private int RADIUS;
        private int LINE_THICKNESS = 2;
        private int SMALL_RADIUS = 10;
        private Brush COLOR_LINE = Brushes.Black;
        private Brush COLOR_CIRCLE = Brushes.LightBlue;
        private Brush COLOR_FIRST_COMPANY = Brushes.Green;
        private Brush COLOR_SECOND_COMPANY = Brushes.Red;

        public MainWindow()
        {
            this.InitializeComponent();
            WIDTH = (int)Math.Round(Window.Width);
            HEIGHT = (int)Math.Round(Country.Height);
            RADIUS = (HEIGHT / 2 - SMALL_RADIUS);
        }

        private void PrivatizateSysytem(object sender, System.Windows.RoutedEventArgs e)
        {
            if (graph == null)
            {
                MessageBox.Show("Сначала постройте систему");
                return;
            }
            if (!graph.isPrivatizate)
            {
                graph.Privatizate();
                DrawGraph();
            }
        }

        private void ShowSystem(object sender, System.Windows.RoutedEventArgs e)
        {
            try
            {
                int countOfCities = Convert.ToInt32(numberOfCities.Text);
                graph = new Graph(countOfCities);
                DrawGraph();
            }
            catch (Exception)
            {
                MessageBox.Show("Номер должен быть четный и в пределах от " +
                    Graph.COUNT_OF_EDGE.ToString() + " до " +
                    Graph.MAX_COUNT_OF_TOPS.ToString() + ".");
            }
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
            Country.Children.Add(el);
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
            Country.Children.Add(ln);
        }


        private void DrawGraph()
        {
            //чистим
            Country.Children.RemoveRange(0, Country.Children.Count);

            //рисуем ребра
            foreach (Edge edge in graph.Edges)
            {
                double angle1 = Math.PI * 2 * edge.first
                    / graph.Size;
                int x1 = Convert.ToInt32(WIDTH / 2 + RADIUS * Math.Cos(angle1));
                int y1 = Convert.ToInt32(HEIGHT / 2 + RADIUS * Math.Sin(angle1));

                double angle2 = Math.PI * 2 * edge.second
                    / graph.Size;
                int x2 = Convert.ToInt32(WIDTH / 2 + RADIUS * Math.Cos(angle2));
                int y2 = Convert.ToInt32(HEIGHT / 2 + RADIUS * Math.Sin(angle2));

                //цвет зависит от владельца
                switch (edge.Owner)
                {
                    case 0:
                        DrawLine(x1, y1, x2, y2, edge.isFict,COLOR_LINE);
                        break;
                    case 1:
                        DrawLine(x1, y1, x2, y2, edge.isFict, COLOR_FIRST_COMPANY);
                        break;
                    case 2:
                        DrawLine(x1, y1, x2, y2, edge.isFict, COLOR_SECOND_COMPANY);
                        break;
                }
            }


            //рисуем вершины
            for (int top = 0; top < graph.Size; top++)
            {
                double angle = Math.PI * 2 * top / graph.Size;

                int x = Convert.ToInt32(WIDTH / 2 + RADIUS * Math.Cos(angle));
                int y = Convert.ToInt32(HEIGHT / 2 + RADIUS * Math.Sin(angle));

                DrawCircle(x, y, SMALL_RADIUS);
            }
        }

    }
}