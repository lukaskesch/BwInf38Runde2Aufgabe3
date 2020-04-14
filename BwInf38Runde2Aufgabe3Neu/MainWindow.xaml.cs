using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BwInf38Runde2Aufgabe3Neu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Drawing
        double DX, DY;
        double X0, Y0;
        double MinX, MaxX, MinY, MaxY;
        const int CanvasGridMargin = 20;
        const int PointRadius = 10;

        //Paths
        Vertex[] ShortestPath;
        Vertex[] RecommendedPath;

        //Statistics
        Stopwatch stopwatch = new Stopwatch();

        public MainWindow()
        {
            InitializeComponent();

            //Change UI
            GridPathInfo.Visibility = Visibility.Hidden;
            TabPanelYellow.Visibility = Visibility.Hidden;
            SliderPercentage.Visibility = Visibility.Hidden;
            LabelPercentage.Visibility = Visibility.Hidden;
            ButtonCalculate.Visibility = Visibility.Hidden;
        }

        private void ButtonLoad_Click(object sender, RoutedEventArgs e)
        {
            //Stopwatch
            long TimeDijkstra, TimeAlgorithm;

            //Change UI
            CanvasGrid.Children.Clear();
            TabPanelYellow.Visibility = Visibility.Hidden;
            GridPathInfo.Visibility = Visibility.Hidden;
            SliderPercentage.Visibility = Visibility.Hidden;
            LabelPercentage.Visibility = Visibility.Hidden;
            ButtonCalculate.Visibility = Visibility.Hidden;

            //Reset paths
            RecommendedPath = null;
            LabelRecommendedPath.Content = string.Empty;
            LabelRecommendedPathLenght.Content = string.Empty;
            LabelRecommendedPathPercentage.Content = string.Empty;

            //Data Preprocessing
            if (!Data.ReadDataFromFile())
            {
                MessageBox.Show("Something went wrong");
                CanvasGrid.Children.Clear();
                return;
            }

            try
            {
                //Find shortest path length
                stopwatch.Restart();
                Dijkstra.FindShortestPathLength();
                stopwatch.Stop();
                TimeDijkstra = stopwatch.ElapsedMilliseconds;


                //Get shortest path
                stopwatch.Restart();
                ShortestPath = Recursion.FindRecommendedPath(0);
                stopwatch.Stop();
                TimeAlgorithm = stopwatch.ElapsedMilliseconds;

                //Change UI
                LabelRuntimeDijkstra.Content = "Dijkstra: " + TimeDijkstra.ToString() + " ms";
                LabelRuntimeAlgorithm.Content = "Algorithm: " + TimeAlgorithm.ToString() + " ms";
                LabelShortestPath.Content = "Shortest Path (" + Recursion.GetNumberOfTurns().ToString() + " Turns):";
                LabelShortestPathLenght.Content = Recursion.GetPathLength().ToString();
                Draw();
                GridPathInfo.Visibility = Visibility.Visible;
                SliderPercentage.Visibility = Visibility.Visible;
                LabelPercentage.Visibility = Visibility.Visible;
                ButtonCalculate.Visibility = Visibility.Visible;
            }
            catch (Exception)
            {
                MessageBox.Show("Graph is not complete");
                //throw;
            }

        }

        private void ButtonCalculate_Click(object sender, RoutedEventArgs e)
        {
            //Stopwatch
            long TimeAlgorithm;

            //Change UI
            GridPathInfo.Visibility = Visibility.Visible;
            TabPanelYellow.Visibility = Visibility.Visible;

            //Get recommended path
            stopwatch.Restart();
            int percent = (int)SliderPercentage.Value;
            RecommendedPath = Recursion.FindRecommendedPath(percent);
            //RecommendedPath = Recursion.FindRecommendedPath(SliderPercentage.Value);
            stopwatch.Stop();
            TimeAlgorithm = stopwatch.ElapsedMilliseconds;

            //Change UI
            LabelRuntimeAlgorithm.Content = "Algorithm: " + TimeAlgorithm.ToString() + " ms";
            LabelRecommendedPath.Content = "Recommended Path (" + Recursion.GetNumberOfTurns().ToString() + " Turns):";
            //LabelRecommendedPathLenght.Content = Recursion.GetPathLength().ToString();
            double RecommendedPathLength = Recursion.GetPathLength();
            double ShortestPathLength = double.Parse(LabelShortestPathLenght.Content.ToString());
            LabelRecommendedPathLenght.Content = RecommendedPathLength.ToString();
            LabelRecommendedPathPercentage.Content = (Math.Round((RecommendedPathLength - ShortestPathLength) / ShortestPathLength * 10000) / 100).ToString() + "%";
            Draw();
        }

        private void SliderPercentage_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            try
            {
                //Adjust Percentage Label
                int value = (int)(SliderPercentage.Value);
                LabelPercentage.Content = value.ToString() + "%";
            }
            catch
            {
                return;
            }
        }
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Draw new scaled map
            if (Data.ArrayEdges != null)
            {
                Draw();
            }
        }
        private void Draw()
        {
            //Draw the entire map
            CanvasGrid.Children.Clear();
            SetParameters();
            SetScale();
            DrawAllLines();
            DrawShortestPath();
            DrawRecommendedPath();
            DrawAllPoints();
        }
        private Point PlotToCanvas(Point P)
        {
            //Calculate mathmatical coordinates to canvas coordinates
            return new Point(DX * P.X + X0, DY * P.Y + Y0);
        }
        private void SetParameters()
        {
            //Set parameters for new map
            if (Data.NewParameters)
            {
                Data.NewParameters = false;
                Data.FindBoundaries(ref MinX, ref MaxX, ref MinY, ref MaxY);
            }
        }
        private void SetScale()
        {
            //Set custom scale
            DX = (CanvasGrid.ActualWidth - 2 * CanvasGridMargin) / (MaxX - MinX);
            X0 = CanvasGridMargin - DX * MinX;
            DY = (CanvasGrid.ActualHeight - 2 * CanvasGridMargin) / (MinY - MaxY);
            Y0 = CanvasGridMargin - DY * MaxY;
        }
        private void DrawShortestPath()
        {
            //Draw shortest path
            if (ShortestPath == null)
            {
                return;
            }

            int StrockeThickness = 3;
            Brush brush = Brushes.Green;
            for (int i = 0; i < ShortestPath.Length - 1; i++)
            {
                DrawLine(new Edge(ShortestPath[i], ShortestPath[i + 1]), brush, StrockeThickness);
            }
        }

        private void DrawRecommendedPath()
        {
            //Draw recommended path
            if (RecommendedPath == null)
            {
                return;
            }

            int StrockeThickness = 4;
            Brush brush = Brushes.Yellow;
            for (int i = 0; i < RecommendedPath.Length - 1; i++)
            {
                DrawLine(new Edge(RecommendedPath[i], RecommendedPath[i + 1]), brush, StrockeThickness);
            }
        }
        private void DrawAllLines()
        {
            //Draw all lines
            Brush brush = Brushes.Gray;
            int StrokeThicknes = 2;

            for (int i = 0; i < Data.ArrayEdges.Length; i++)
            {
                DrawLine(Data.ArrayEdges[i], brush, StrokeThicknes);
            }
        }
        private void DrawLine(Edge tline, Brush brush, int StrokeThicknes)
        {
            //Draw line
            Point point1, point2;
            point1 = PlotToCanvas(new Point(tline.X1, tline.Y1));
            point2 = PlotToCanvas(new Point(tline.X2, tline.Y2));

            Line line = new Line()
            {
                X1 = point1.X,
                Y1 = point1.Y,
                X2 = point2.X,
                Y2 = point2.Y,
                Stroke = brush,
                StrokeThickness = StrokeThicknes
            };
            //line.X1 = point1.X;
            //line.Y1 = point1.Y;
            //line.X2 = point2.X;
            //line.Y2 = point2.Y;
            //line.Stroke = brush;
            //line.StrokeThickness = StrokeThicknes;

            CanvasGrid.Children.Add(line);
        }
        private void DrawAllPoints()
        {
            //Draw all points
            DrawPoint(Data.ArrayVertices[0], Brushes.Green);
            DrawPoint(Data.ArrayVertices[1], Brushes.Red);
            for (int i = 2; i < Data.ArrayVertices.Length; i++)
            {
                DrawPoint(Data.ArrayVertices[i], Brushes.Gray);
            }
        }
        private void DrawPoint(Vertex tpoint, Brush brush)
        {
            //Draw point
            Point point = PlotToCanvas(new Point(tpoint.X, tpoint.Y));
            Ellipse ellipse = new Ellipse()
            {
                Width = PointRadius,
                Height = PointRadius,
                Fill = brush
            };
            CanvasGrid.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, point.X - PointRadius * 0.5);
            Canvas.SetTop(ellipse, point.Y - PointRadius * 0.5);
        }



        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            CreatMapWindow Window = new CreatMapWindow();
            Window.ShowDialog();

        }

    }
}
