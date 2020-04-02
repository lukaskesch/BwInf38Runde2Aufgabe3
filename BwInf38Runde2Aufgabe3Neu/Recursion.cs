using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BwInf38Runde2Aufgabe3Neu
{
    class Recursion
    {
        private static int MaxTurns;
        private static double MaxPathLength;
        private static double BestPathLength;

        private static Vertex StartPoint;
        private static Vertex EndPoint;
        private static RecursionVertexInfo RecommendedPathInfo;

        public static Vertex[] FindRecommendedPath(double Percent)
        {
            //Set boundaries
            MaxPathLength = Dijkstra.GetMinPathLength() * (1 + Percent / 100);
            BestPathLength = MaxPathLength;
            MaxTurns = Data.CalculateMaxTurns();
            StartPoint = Data.ArrayVertices[0];
            EndPoint = Data.ArrayVertices[1];

            //Prepare datastructure for recursion
            Vertex NeighboorPoint;
            RecursionVertexInfo PriorPointInfo = new RecursionVertexInfo();
            PriorPointInfo.ListOfPriorPoints = new List<int>();
            PriorPointInfo.ListOfPriorPoints.Add(0);
            Data.ArrayVertices[0].Visited = true;

            //Start RecursionMethod
            for (int i = 0; i < StartPoint.NeighboorsIndices.Count; i++)
            {
                NeighboorPoint = Data.ArrayVertices[StartPoint.NeighboorsIndices[i]];
                PriorPointInfo.Angle = Data.CalculateAngle(StartPoint, NeighboorPoint);
                RecursionMethod(PriorPointInfo, StartPoint, NeighboorPoint);
            }

            //Create recommended path
            List<Vertex> ListRecommendedPath = new List<Vertex>();
            for (int i = 0; i < RecommendedPathInfo.ListOfPriorPoints.Count; i++)
            {
                ListRecommendedPath.Add(Data.ArrayVertices[RecommendedPathInfo.ListOfPriorPoints[i]]);
            }
            return ListRecommendedPath.ToArray();
        }
        private static void RecursionMethod(RecursionVertexInfo PriorPointInfo, Vertex PriorPoint, Vertex CurrentPoint)
        {
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
            if (AngleNew != AngleOld && ++Turns > MaxTurns)
            {
                return;
            }

            //Set current point
            PriorPointInfo.ListOfPriorPoints.Add(CurrentPoint.ElementNumber);
            PriorPointInfo.Distance = Distance;
            PriorPointInfo.Turns = Turns;
            PriorPointInfo.Angle = AngleNew;

            //May update recommended path
            if ((CurrentPoint == EndPoint) && (Turns < MaxTurns || (Turns == MaxTurns && Distance < BestPathLength)))
            {
                MaxTurns = Turns;
                BestPathLength = Distance;
                RecommendedPathInfo = new RecursionVertexInfo(PriorPointInfo);
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
                    RecursionMethod(PriorPointInfo, CurrentPoint, Neighboor);
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
    }
}
