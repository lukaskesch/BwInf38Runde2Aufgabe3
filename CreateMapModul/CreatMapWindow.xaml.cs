using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;

namespace BwInf38Runde2Aufgabe3Neu
{
    /// <summary>
    /// Interaction logic for CreatMapWindow.xaml
    /// </summary>
    public partial class CreatMapWindow : Window
    {
        double MarginCanvas = 50;
        double MaxX, MaxY;
        double dx, dy;

        Point Startpoint;
        Point Endpoint;
        Ellipse EllipseStartpoint, EllipseEndpoint;

        private bool BoolNewLine;
        private bool BoolDeleteLine;
        Line TemporaryLine;
        Point Point1;
        Point Point2;

        bool BoolStartpoint, BoolEndpoint;

        List<Point> PointList = new List<Point>();
        List<Line> LineList = new List<Line>();
        public CreatMapWindow()
        {
            InitializeComponent();
            BoolNewLine = false;
            BoolDeleteLine = false;
            Endpoint = new Point(-10, -10);
        }
        private void ButtonStartpoint_Click(object sender, RoutedEventArgs e)
        {
            BoolStartpoint = true;
        }
        private void ButtonEndpoint_Click(object sender, RoutedEventArgs e)
        {
            BoolEndpoint = true;
        }
        private void NewLine_Click(object sender, RoutedEventArgs e)
        {
            BoolNewLine = true;
        }
        private void DeleteLine_Click(object sender, RoutedEventArgs e)
        {
            BoolDeleteLine = true;
        }
        private void SaveMap_Click(object sender, RoutedEventArgs e)
        {
            string s = string.Empty;

            s += LineList.Count.ToString() + Environment.NewLine;
            s += "(" + Startpoint.X.ToString() + "," + Startpoint.Y.ToString() + ")" + Environment.NewLine;
            s += "(" + Endpoint.X.ToString() + "," + Endpoint.Y.ToString() + ")";

            foreach (Line line in LineList)
            {
                s += Environment.NewLine;
                s += "(" + line.X1.ToString() + "," + line.Y1.ToString() + ") ";
                s += "(" + line.X2.ToString() + "," + line.Y2.ToString() + ")";
            }

            //System.Windows.MessageBox.Show(s);

            SaveFileDialog Dlg = new SaveFileDialog();
            Dlg.Title = "Save Map";
            Dlg.DefaultExt = ".txt";
            Dlg.Filter = "Text documents (.txt)|*.txt";
            Dlg.FileName = "Beispiel.txt";
            Dlg.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Material";

            Nullable<bool> result = Dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                StreamWriter streamWriter = new StreamWriter(Dlg.FileName, false);
                try
                {
                    streamWriter.Write(s);
                }
                finally
                {
                    streamWriter.Close();
                }
                //StreamWriter streamWriter = File.Create(Dlg.FileName);
            }

        }
        private void Close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private Point PlotPointToCanvas(Point P)
        {
            return new Point(P.X * dx + MarginCanvas, (MaxY - P.Y) * dy + MarginCanvas);
        }
        private Point PlotPointToCoordinateSystem(Point P)
        {
            return new Point(Math.Round((P.X - MarginCanvas) / dx), Math.Round(MaxY - (P.Y - MarginCanvas) / dy));
        }
        private void SetScale_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MaxX = int.Parse(TextBoxMaxX.Text);
                dx = (CanvasMap.ActualWidth - 2 * MarginCanvas) / MaxX;
                MaxY = int.Parse(TextBoxMaxY.Text);
                dy = (CanvasMap.ActualHeight - 2 * MarginCanvas) / MaxY;
            }
            catch (Exception)
            {
                return;
            }
            CanvasMap.Children.Clear();
            DrawCoordinateSystem();
        }
        private void DrawLine(Point P1, Point P2, double Opacity, int StrokeThicknes, Brush B)
        {
            P1 = PlotPointToCanvas(P1);
            P2 = PlotPointToCanvas(P2);

            Line line = new Line()
            {
                X1 = P1.X,
                Y1 = P1.Y,
                X2 = P2.X,
                Y2 = P2.Y,
                Stroke = B,
                StrokeThickness = StrokeThicknes,
                Opacity = Opacity
            };
            CanvasMap.Children.Add(line);
        }
        private void DrawPoint(Point P, Brush B)
        {
            double radius = 8;
            P = PlotPointToCanvas(P);
            Ellipse ellipse = new Ellipse()
            {
                Width = radius * 2,
                Height = radius * 2,
                Fill = B
            };
            CanvasMap.Children.Add(ellipse);
            Canvas.SetLeft(ellipse, P.X - radius);
            Canvas.SetTop(ellipse, P.Y - radius);
        }



        private void DrawCoordinateSystem()
        {
            double Opacity = 0.5;
            int StrokeThickness = 3;
            Brush B = Brushes.Gray;
            Point P1 = new Point();
            Point P2 = new Point();

            P1.Y = 0;
            P2.Y = MaxY;
            for (int i = 0; i <= MaxX; i++)
            {
                P1.X = i;
                P2.X = i;
                DrawLine(P1, P2, Opacity, StrokeThickness, B);
            }

            P1.X = 0;
            P2.X = MaxX;
            for (int i = 0; i <= MaxY; i++)
            {
                P1.Y = i;
                P2.Y = i;
                DrawLine(P1, P2, Opacity, StrokeThickness, B);
            }


        }
        private void CanvasMap_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (BoolNewLine || BoolDeleteLine)
            {
                Point point = Mouse.GetPosition(GridCanvas);
                point = PlotPointToCoordinateSystem(point);
                if (point.X > MaxX || point.Y > MaxY || point.X < 0 || point.Y < 0)
                {
                    BoolNewLine = false;
                    BoolDeleteLine = false;
                    return;
                }
                Point1 = point;
            }
        }



        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            //Check if mouse position is in bound
            Point point = Mouse.GetPosition(GridCanvas);
            point = PlotPointToCoordinateSystem(point);
            if (point.X > MaxX || point.Y > MaxY || point.X < 0 || point.Y < 0)
            {
                LabelCurrentPosition.Content = "out of bounds";
                return;
            }

            //Draw new line
            if ((BoolNewLine || BoolDeleteLine) && e.LeftButton == MouseButtonState.Pressed)
            {
                Brush B;
                if (BoolNewLine)
                {
                    B = Brushes.Green;
                }
                else
                {
                    B = Brushes.Red;
                }
                CanvasMap.Children.Remove(TemporaryLine);
                TemporaryLine = new Line()
                {
                    X1 = PlotPointToCanvas(Point1).X,
                    Y1 = PlotPointToCanvas(Point1).Y,
                    X2 = PlotPointToCanvas(point).X,
                    Y2 = PlotPointToCanvas(point).Y,
                    Stroke = B,
                    StrokeThickness = 7,
                    Opacity = 0.8
                };
                CanvasMap.Children.Add(TemporaryLine);
            }
            LabelCurrentPosition.Content = "(" + point.X + ";" + point.Y + ")";
        }
        private void CanvasMap_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            CanvasMap.Children.Remove(TemporaryLine);

            //Check if point is in bound
            Point point = Mouse.GetPosition(GridCanvas);
            point = PlotPointToCoordinateSystem(point);
            if ((point.X > MaxX || point.Y > MaxY || point.X < 0 || point.Y < 0) && (BoolNewLine || BoolDeleteLine || BoolStartpoint || BoolEndpoint))
            {
                LabelCurrentPosition.Content = "out of bounds";
                System.Windows.MessageBox.Show("Point is out of bounds. Start over");
                BoolStartpoint = false;
                BoolEndpoint = false;
                BoolNewLine = false;
                BoolDeleteLine = false;
                return;
            }

            if (BoolNewLine || BoolDeleteLine)
            {
                if (point == Point1)
                {
                    BoolDeleteLine = false;
                    BoolNewLine = false;
                    System.Windows.MessageBox.Show("Invalid point");
                    return;
                }
                Point2 = point;

                //Create line
                Line line = new Line()
                {
                    X1 = Point1.X,
                    Y1 = Point1.Y,
                    X2 = Point2.X,
                    Y2 = Point2.Y,
                };
                Line LineCanvas = new Line()
                {
                    X1 = PlotPointToCanvas(Point1).X,
                    Y1 = PlotPointToCanvas(Point1).Y,
                    X2 = PlotPointToCanvas(Point2).X,
                    Y2 = PlotPointToCanvas(Point2).Y,
                    Stroke = Brushes.Black,
                    StrokeThickness = 7,
                    Opacity = 0.8
                };
                //Check if Line already exist
                if (BoolDeleteLine)
                {
                    foreach (Line l in LineList)
                    {
                        bool Check1 = line.X1 == l.X1 && line.Y1 == l.Y1 && line.X2 == l.X2 && line.Y2 == l.Y2;
                        bool Check2 = line.X1 == l.X2 && line.Y1 == l.Y2 && line.X2 == l.X1 && line.Y2 == l.Y1;

                        if (Check1 || Check2)
                        {
                            MessageBox.Show("Line dot");
                            LineList.Remove(l);
                            break;
                        }
                    }
                    bool BoolP1 = false;
                    bool BoolP2 = false;
                    for (int i = 0; i < CanvasMap.Children.Count; i++)
                    {
                        var Var = CanvasMap.Children[i];
                        Line l;
                        Ellipse E = new Ellipse();
                        if (Var.GetType() == line.GetType())
                        {
                            l = (Line)Var;
                            Point p1 = PlotPointToCoordinateSystem(new Point(l.X1, l.Y1));
                            Point p2 = PlotPointToCoordinateSystem(new Point(l.X2, l.Y2));
                            bool Check1 = line.X1 == p1.X && line.Y1 == p1.Y && line.X2 == p2.X && line.Y2 == p2.Y;
                            bool Check2 = line.X1 == p2.X && line.Y1 == p2.Y && line.X2 == p1.X && line.Y2 == p1.Y;

                            if (Check1 || Check2)
                            {
                                MessageBox.Show("Line Canvas dot");
                                CanvasMap.Children.RemoveAt(i);
                                break;
                            }
                        }
                        else if (Var.GetType() == E.GetType())
                        {

                        }
                    }
                    CanvasMap.Children.Remove(LineCanvas);
                    BoolDeleteLine = false;
                    try
                    {
                        LineList.Remove(line);

                        return;
                    }
                    catch (Exception)
                    {
                        return;
                        //throw;
                    }

                }
                else
                {
                    if (BoolDeleteLine)
                    {
                        BoolDeleteLine = false;
                        System.Windows.MessageBox.Show("Line doesn't exist");
                    }
                    else
                    {
                        BoolNewLine = false;
                        LineList.Add(line);
                        PointList.Add(Point1);
                        PointList.Add(Point2);
                        DrawPoint(Point1, Brushes.Black);
                        DrawPoint(Point2, Brushes.Black);
                        CanvasMap.Children.Add(LineCanvas);
                    }
                }
            }
            else if (BoolStartpoint)
            {
                Startpoint = point;

                if (Endpoint == Startpoint)
                {
                    System.Windows.MessageBox.Show("Start- and Endpoint can't be the same");
                    return;
                }

                double radius = 8;
                point = PlotPointToCanvas(point);
                CanvasMap.Children.Remove(EllipseStartpoint);
                EllipseStartpoint = new Ellipse()
                {
                    Width = radius * 2,
                    Height = radius * 2,
                    Fill = Brushes.Green
                };
                CanvasMap.Children.Add(EllipseStartpoint);
                Canvas.SetLeft(EllipseStartpoint, point.X - radius);
                Canvas.SetTop(EllipseStartpoint, point.Y - radius);

                BoolStartpoint = false;
            }
            else if (BoolEndpoint)
            {
                BoolEndpoint = false;
                Endpoint = point;

                if (Endpoint == Startpoint)
                {
                    System.Windows.MessageBox.Show("Start- and Endpoint can't be the same");
                    return;
                }

                double radius = 8;
                point = PlotPointToCanvas(point);
                CanvasMap.Children.Remove(EllipseEndpoint);
                EllipseEndpoint = new Ellipse()
                {
                    Width = radius * 2,
                    Height = radius * 2,
                    Fill = Brushes.Red
                };
                CanvasMap.Children.Add(EllipseEndpoint);
                Canvas.SetLeft(EllipseEndpoint, point.X - radius);
                Canvas.SetTop(EllipseEndpoint, point.Y - radius);
            }
        }
        private void Window_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            NewLine_Click(null, null);
        }


    }
}

