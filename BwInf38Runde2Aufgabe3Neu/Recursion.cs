using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BwInf38Runde2Aufgabe3Neu
{
    class Recursion
    {
        private static long[] Statistics;
        private static double StatisticsPercent;
        private static Stopwatch stopwatch = new Stopwatch();

        private static double Epsilon = Math.Pow(10.0, -12.0);

        private static int MaxTurns;
        private static double MaxPathLength;
        private static double BestPathLength;

        private static Vertex StartPoint;
        private static Vertex EndPoint;
        private static RecursionVertexInfo RecommendedPathInfo;

        public static Vertex[] FindRecommendedPath(double Percent)
        {
            stopwatch.Restart();

            //Set boundaries
            MaxPathLength = Dijkstra.GetMinPathLength() * (1 + Percent / 100);
            BestPathLength = MaxPathLength * (1 + Epsilon);
            MaxTurns = Data.CalculateMaxTurns();
            StartPoint = Data.ArrayVertices[0];
            EndPoint = Data.ArrayVertices[1];
            StatisticsPercent = Percent;

            //Prepare datastructure for recursion
            Vertex NeighboorPoint;
            RecursionVertexInfo PriorPointInfo = new RecursionVertexInfo();
            PriorPointInfo.ListOfPriorPoints = new List<int>();
            PriorPointInfo.ListOfPriorPoints.Add(0);
            Data.ArrayVertices[0].Visited = true;
            Statistics = new long[150];

            //Start RecursionMethod
            for (int i = 0; i < StartPoint.NeighboorsIndices.Count; i++)
            {
                Statistics[0]++;
                NeighboorPoint = Data.ArrayVertices[StartPoint.NeighboorsIndices[i]];
                PriorPointInfo.Angle = Data.CalculateAngle(StartPoint, NeighboorPoint);
                RecursionMethod(PriorPointInfo, StartPoint, NeighboorPoint, 1);
            }

            stopwatch.Stop();
            SaveStatistics();

            //Create recommended path
            List<Vertex> ListRecommendedPath = new List<Vertex>();
            for (int i = 0; i < RecommendedPathInfo.ListOfPriorPoints.Count; i++)
            {
                ListRecommendedPath.Add(Data.ArrayVertices[RecommendedPathInfo.ListOfPriorPoints[i]]);
            }
            return ListRecommendedPath.ToArray();
        }
        private static void RecursionMethod(RecursionVertexInfo PriorPointInfo, Vertex PriorPoint, Vertex CurrentPoint, int Depth)
        {
            if (Depth >= 20)
            {
                return;
            }

            //Get data from PriorPoint
            int Turns = PriorPointInfo.Turns;
            double Distance = PriorPointInfo.Distance;
            double AngleOld = PriorPointInfo.Angle;

            //Check new distance
            Distance += Data.CalculateLength(PriorPoint, CurrentPoint);
            if (Distance > MaxPathLength)
            {
                return;
            }

            //Check angle and may adjust turns
            double AngleNew = Data.CalculateAngle(PriorPoint, CurrentPoint);
            //if (AngleNew != AngleOld)
            //{
            //    Turns++;
            //}
            if (Math.Abs(AngleNew - AngleOld) > Epsilon && ++Turns > MaxTurns)
            {
                return;
            }

            //Set current point
            PriorPointInfo.ListOfPriorPoints.Add(CurrentPoint.ElementNumber);
            PriorPointInfo.Distance = Distance;
            PriorPointInfo.Turns = Turns;
            PriorPointInfo.Angle = AngleNew;

            //May update recommended path
            if (Distance < MaxPathLength * (1 + Epsilon))
            {
                if ((CurrentPoint == EndPoint) && (Turns < MaxTurns || (Turns == MaxTurns && Distance < BestPathLength)))
                {
                    MaxTurns = Turns;
                    BestPathLength = Distance;
                    RecommendedPathInfo = new RecursionVertexInfo(PriorPointInfo);
                }
            }


            //Mark current point as visited
            Data.ArrayVertices[CurrentPoint.ElementNumber].Visited = true;

            //Calls itself if unvisited neighboors exist
            for (int i = 0; i < CurrentPoint.NeighboorsIndices.Count; i++)
            {
                int IndexNeighboor = CurrentPoint.NeighboorsIndices[i];
                Vertex Neighboor = Data.ArrayVertices[IndexNeighboor];
                if (!Neighboor.Visited)
                {
                    Statistics[Depth]++;
                    RecursionMethod(PriorPointInfo, CurrentPoint, Neighboor, ++Depth);
                    Depth--;
                }
            }

            //Reset referance changes
            PriorPointInfo.ListOfPriorPoints.Remove(CurrentPoint.ElementNumber);
            Data.ArrayVertices[CurrentPoint.ElementNumber].Visited = false;
        }

        public static int GetNumberOfTurns()
        {
            return MaxTurns;
        }
        public static double GetPathLength()
        {
            return Math.Round(BestPathLength * 1000) / 1000;
        }
        private static void SaveStatistics()
        {
            string FileString = GetStatisticsString();
            StreamWriter WriterStatistics = File.AppendText("Statistics.csf");
            try
            {
                WriterStatistics.WriteLine(FileString);
            }
            finally
            {
                WriterStatistics.Close();
            }
        }
        private static string GetStatisticsString()
        {
            string S = string.Empty;

            //Filename
            //string[] FileDirecories = Data.FileName.Split('\\');
            //S += FileDirecories[FileDirecories.Length - 1] + ",";

            S += Data.FileName + ",";
            S += StatisticsPercent.ToString() + ",";
            S += stopwatch.ElapsedMilliseconds.ToString() + ",";

            for (int i = 0; i < Statistics.Length - 1; i++)
            {
                S += Statistics[i].ToString();
                if (Statistics[i + 1] != 0)
                {
                    S += ",";
                }
                else
                {
                    break;
                }
            }
            return S;
        }
    }
}
