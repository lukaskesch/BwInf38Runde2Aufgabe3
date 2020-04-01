using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BwInf38Runde2Aufgabe3Neu
{
    class Dijkstra
    {
        //Datastructures
        private static double MinPathLength;
        private static VertexInfo[] ArrayPointInfo = null;
        public static Vertex[] ShortestPath = null;

        public static void FindShortestPathLength()
        {
            //Preparation
            Prepare();
            Vertex CurrentPoint = Data.ArrayPoints[0];
            Vertex StartPoint = Data.ArrayPoints[0];
            Vertex EndPoint = Data.ArrayPoints[1];

            //Dijkstra
            do
            {
                FindNeighboorsOfCurrentPointAndUpdateDistance(CurrentPoint);
                CurrentPoint = FindNewShortestPointToStartpoint();
                ArrayPointInfo[CurrentPoint.ElementNumber].Visited = true;
            }
            while (CurrentPoint != EndPoint);

            //Set lowest path length
            MinPathLength = ArrayPointInfo[1].DistanceToStartpoint;

            //Create ShortestPath
            int IndexNewPoint;
            List<Vertex> ListShortestPath = new List<Vertex>();
            ListShortestPath.Add(EndPoint);
            CurrentPoint = EndPoint;
            do
            {
                IndexNewPoint = ArrayPointInfo[CurrentPoint.ElementNumber].IndexOfPriorPoint;
                CurrentPoint = Data.ArrayPoints[IndexNewPoint];
                ListShortestPath.Add(CurrentPoint);

            }
            while (CurrentPoint != StartPoint);
            ShortestPath = ListShortestPath.ToArray();

        }
        private static void Prepare()
        {
            //Set Structure
            ArrayPointInfo = new VertexInfo[Data.ArrayPoints.Length];

            //Starting point
            ArrayPointInfo[0].IndexOfPriorPoint = -1;
            ArrayPointInfo[0].DistanceToStartpoint = 0;
            ArrayPointInfo[0].Visited = true;

            //Other points
            for (int i = 1; i < ArrayPointInfo.Length; i++)
            {
                ArrayPointInfo[i].DistanceToStartpoint = double.MaxValue;
                ArrayPointInfo[i].IndexOfPriorPoint = -1;
                ArrayPointInfo[i].Visited = false;
            }
        }
        private static void FindNeighboorsOfCurrentPointAndUpdateDistance(Vertex CurrentPoint)
        {
            //Checks and may updates neighboor points 
            int IndexNeighboorPoint;
            VertexInfo CurrentPointInfo = ArrayPointInfo[CurrentPoint.ElementNumber];
            for (int i = 0; i < CurrentPoint.NeighboorsIndices.Count; i++)
            {
                IndexNeighboorPoint = CurrentPoint.NeighboorsIndices[i];

                double DistanceOld = ArrayPointInfo[IndexNeighboorPoint].DistanceToStartpoint;
                double DistanceNew = CurrentPointInfo.DistanceToStartpoint + Data.CalculateLength(CurrentPoint, Data.ArrayPoints[IndexNeighboorPoint]);
                if (DistanceOld > DistanceNew)
                {
                    ArrayPointInfo[IndexNeighboorPoint].DistanceToStartpoint = DistanceNew;
                    ArrayPointInfo[IndexNeighboorPoint].IndexOfPriorPoint = CurrentPoint.ElementNumber;
                }
            }
        }
        private static Vertex FindNewShortestPointToStartpoint()
        {
            //Finds the next shortest point to the starting point
            int IndexShortestPointToStartpoint = -1;
            double ShortestDistance = double.MaxValue;
            for (int i = 0; i < ArrayPointInfo.Length; i++)
            {
                if (!ArrayPointInfo[i].Visited && ShortestDistance >= ArrayPointInfo[i].DistanceToStartpoint)
                {
                    ShortestDistance = ArrayPointInfo[i].DistanceToStartpoint;
                    IndexShortestPointToStartpoint = i;
                }
            }
            return Data.ArrayPoints[IndexShortestPointToStartpoint];
        }
        public static double GetMinPathLength()
        {
            return MinPathLength;
        }
    }

}
