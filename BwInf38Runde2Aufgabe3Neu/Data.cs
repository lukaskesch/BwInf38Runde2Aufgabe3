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
        public static Vertex[] ArrayVertices;
        public static Edge[] ArrayEdges;
        private static List<Vertex> ListVertices = new List<Vertex>();
        private static List<Edge> ListEdges = new List<Edge>();

        public static bool NewParameters = false;
        public static string FileName;
        public static bool ReadDataFromFile()
        {
            try
            {
                //Reset
                NewParameters = true;
                ListEdges = new List<Edge>();
                ListVertices = new List<Vertex>();
                ArrayEdges = null;
                ArrayVertices = null;
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

                //FileName = Dlg.FileName;
                FileName = Dlg.SafeFileName;

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
                FillStartAndEndPoint(InputArray);
                FillLists(InputArray);
                ArrayVertices = ListVertices.ToArray();
                ArrayEdges = ListEdges.ToArray();
                AddNeighboorPoints();
                return true;
            }
            catch
            {
                //throw;
                ArrayVertices = null;
                ArrayEdges = null;
                return false;
            }
        }
        private static void FillLists(string[] InputArray)
        {
            //Fill all lines in List
            Vertex vertex1, vertex2;
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

                vertex1 = new Vertex(double.Parse(Column1[0]), double.Parse(Column1[1]), -1);
                vertex2 = new Vertex(double.Parse(Column2[0]), double.Parse(Column2[1]), -1);

                ListEdges.Add(new Edge(vertex1, vertex2));

                bool CheckVertex1 = false;
                bool CheckVertex2 = false;
                foreach (Vertex vertex in ListVertices)
                {
                    if (vertex.X == vertex1.X && vertex.Y == vertex1.Y)
                    {
                        CheckVertex1 = true;
                    }
                    if (vertex.X == vertex2.X && vertex.Y == vertex2.Y)
                    {
                        CheckVertex2 = true;
                    }
                }
                if (!CheckVertex1)
                {
                    vertex1.ElementNumber = ListVertices.Count;
                    ListVertices.Add(vertex1);
                }
                if (!CheckVertex2)
                {
                    vertex2.ElementNumber = ListVertices.Count;
                    ListVertices.Add(vertex2);
                }
            }
        }
        private static void FillStartAndEndPoint(string[] InputArray)
        {
            //Add start- and endpoint to list
            char[] CharTrim = { ' ', '(', ')' };

            InputArray[1] = InputArray[1].Trim(CharTrim);
            string[] Column = InputArray[1].Split(',');
            ListVertices.Add(new Vertex(double.Parse(Column[0]), double.Parse(Column[1]), 0));

            InputArray[2] = InputArray[2].Trim(CharTrim);
            Column = InputArray[2].Split(',');
            ListVertices.Add(new Vertex(double.Parse(Column[0]), double.Parse(Column[1]), 1));
        }

        private static void AddNeighboorPoints()
        {
            //Adds all neighboor indices to all points
            Edge line;
            Vertex point;
            int Index1 = -1;
            int Index2 = -1;
            for (int i = 0; i < ArrayEdges.Length; i++)
            {
                line = ArrayEdges[i];

                for (int j = 0; j < ArrayVertices.Length; j++)
                {
                    point = ArrayVertices[j];
                    if (line.X1 == point.X && line.Y1 == point.Y)
                    {
                        Index1 = j;
                    }
                    else if (line.X2 == point.X && line.Y2 == point.Y)
                    {
                        Index2 = j;
                    }
                }
                ArrayVertices[Index1].NeighboorsIndices.Add(Index2);
                ArrayVertices[Index2].NeighboorsIndices.Add(Index1);
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

            for (int i = 0; i < ArrayVertices.Length; i++)
            {
                point = ArrayVertices[i];

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
