using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace BwInf38Runde2Aufgabe3Neu
{
    class Data
    {
        //Datastructures
        public static Vertex[] ArrayPoints;
        public static Edge[] ArrayLines;
        private static List<Vertex> ListPoints = new List<Vertex>();
        private static List<Edge> ListLines = new List<Edge>();

        public static bool NewParameters = false;
        public static bool ReadDataFromFile()
        {
            try
            {
                //Reset
                NewParameters = true;
                ListLines = new List<Edge>();
                ListPoints = new List<Vertex>();
                ArrayLines = null;
                ArrayPoints = null;
                Vertex.ResetIndex();

                //FileContent
                string FileString = string.Empty;

                //Open File Explorer
                Microsoft.Win32.OpenFileDialog Dlg = new Microsoft.Win32.OpenFileDialog()
                {
                    Filter = "\"Abbiegen\"-Datei (*.txt)|*.txt|Alle Dateien (*.*)|*.*",
                    FilterIndex = 0
                };
                string SamplePath = AppDomain.CurrentDomain.BaseDirectory + "Material";
                Dlg.InitialDirectory = SamplePath;

                //If Dialog is closed
                if (Dlg.ShowDialog() != true)
                {
                    throw new ArgumentNullException();
                }

                //Set Streams
                FileStream File = new FileStream(Dlg.FileName, FileMode.Open, FileAccess.Read);
                StreamReader Reader = new StreamReader(File);

                //Read File
                try
                {
                    FileString = Reader.ReadToEnd();
                }
                catch
                {
                    MessageBox.Show("Invalid File");
                    throw new ArgumentNullException();
                }
                finally
                {
                    File.Close();
                    Reader.Close();
                }

                //Data Processing
                string[] InputArray = Regex.Split(FileString, Environment.NewLine);
                FillLineList(InputArray);
                FillStartAndEndPoint(InputArray);
                FillPointList();
                ArrayPoints = ListPoints.ToArray();
                ArrayLines = ListLines.ToArray();
                AddNeighboorPoints();
                return true;
            }
            catch
            {
                //throw;
                ArrayPoints = null;
                ArrayLines = null;
                return false;
            }
        }
        private static void FillLineList(string[] InputArray)
        {
            //Fill all lines in List
            int Length = int.Parse(InputArray[0]);
            char[] CharTrim = { ' ', '(', ')' };
            string[] Row;
            string[] Column1;
            string[] Column2;
            for (int i = 0; i < Length; i++)
            {
                Row = InputArray[i + 3].Split(' ');
                Row[0] = Row[0].Trim(CharTrim);
                Column1 = Row[0].Split(',');
                Row[1] = Row[1].Trim(CharTrim);
                Column2 = Row[1].Split(',');

                ListLines.Add(new Edge(double.Parse(Column1[0]), double.Parse(Column1[1]), double.Parse(Column2[0]), double.Parse(Column2[1])));
            }
        }
        private static void FillStartAndEndPoint(string[] InputArray)
        {
            //Add start- and endpoint to list
            char[] CharTrim = { ' ', '(', ')' };

            InputArray[1] = InputArray[1].Trim(CharTrim);
            string[] Column = InputArray[1].Split(',');
            ListPoints.Add(new Vertex(int.Parse(Column[0]), int.Parse(Column[1])));

            InputArray[2] = InputArray[2].Trim(CharTrim);
            Column = InputArray[2].Split(',');
            ListPoints.Add(new Vertex(int.Parse(Column[0]), int.Parse(Column[1])));
        }
        private static void FillPointList()
        {
            //Add all points to list
            Edge line;
            for (int i = 0; i < ListLines.Count; i++)
            {
                line = ListLines[i];
                AddPoints(line);
            }
        }
        private static void AddPoints(Edge line)
        {
            //Adds point to list, if it isn't already in it
            Vertex point;
            bool Check = false;
            for (int i = 0; i < ListPoints.Count; i++)
            {
                point = ListPoints[i];
                if (line.X1 == point.X && line.Y1 == point.Y)
                {
                    Check = true;
                }
            }
            if (!Check)
            {
                ListPoints.Add(new Vertex(line.X1, line.Y1));
            }

            Check = false;
            for (int i = 0; i < ListPoints.Count; i++)
            {
                point = ListPoints[i];
                if (line.X2 == point.X && line.Y2 == point.Y)
                {
                    Check = true;
                }
            }
            if (!Check)
            {
                ListPoints.Add(new Vertex(line.X2, line.Y2));
            }
        }
        private static void AddNeighboorPoints()
        {
            //Adds all neighboor indices to all points
            Edge line;
            Vertex point;
            int Index1 = -1;
            int Index2 = -1;
            for (int i = 0; i < ArrayLines.Length; i++)
            {
                line = ArrayLines[i];

                for (int j = 0; j < ArrayPoints.Length; j++)
                {
                    point = ArrayPoints[j];
                    if (line.X1 == point.X && line.Y1 == point.Y)
                    {
                        Index1 = j;
                    }
                    else if (line.X2 == point.X && line.Y2 == point.Y)
                    {
                        Index2 = j;
                    }
                }
                ArrayPoints[Index1].NeighboorsIndices.Add(Index2);
                ArrayPoints[Index2].NeighboorsIndices.Add(Index1);
            }
        }
        public static void FindBoundaries(ref double _MinX, ref double _MaxX, ref double _MinY, ref double _MaxY)
        {
            //Find the boundaries for the coordinate system
            Vertex point;
            double MinX = double.MaxValue;
            double MaxX = double.MinValue;
            double MinY = double.MaxValue;
            double MaxY = double.MinValue;

            for (int i = 0; i < ArrayPoints.Length; i++)
            {
                point = ArrayPoints[i];

                if (point.X < MinX)
                {
                    MinX = point.X;
                }
                else if (point.X > MaxX)
                {
                    MaxX = point.X;
                }
                if (point.Y < MinY)
                {
                    MinY = point.Y;
                }
                else if (point.Y > MaxY)
                {
                    MaxY = point.Y;
                }
            }

            _MinX = MinX;
            _MaxX = MaxX;
            _MinY = MinY;
            _MaxY = MaxY;
        }
        public static double CalculateLength(Vertex P1, Vertex P2)
        {
            //Calculates the length between to points
            double dx = P1.X - P2.X;
            double dy = P1.Y - P2.Y;

            return Math.Sqrt(dx * dx + dy * dy);
        }
        public static double CalculateAngle(Vertex P1, Vertex P2)
        {
            //Calculates the angle between the line that connects the two points and the x-axis
            double dx = P1.X - P2.X;
            double dy = P1.Y - P2.Y;

            double angle = Math.Atan(dy / dx) * 360 / (2 * Math.PI);
            if (angle == -90)
            {
                angle = 90;
            }

            return angle;
        }
        public static int CalculateMaxTurns()
        {
            //Calculates the amount of turns in the shortes path
            int Turns = 0;
            double angle1, angle2;
            for (int i = 0; i < Dijkstra.ShortestPath.Length - 2; i++)
            {
                angle1 = CalculateAngle(Dijkstra.ShortestPath[i], Dijkstra.ShortestPath[i + 1]);
                angle2 = CalculateAngle(Dijkstra.ShortestPath[i + 1], Dijkstra.ShortestPath[i + 2]);

                if (angle1 != angle2)
                {
                    Turns++;
                }
            }
            return Turns;
        }
    }




}
