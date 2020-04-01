using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BwInf38Runde2Aufgabe3Neu
{
    struct Vertex
    {
        //Object inspecific
        private static int Index;
        public static void ResetIndex()
        {
            Index = 0;
        }

        //Object specific variables
        public bool Visited;
        public int ElementNumber;
        public double X, Y;
        public List<int> NeighboorsIndices;

        //Constructor
        public Vertex(double _X, double _Y)
        {
            X = _X;
            Y = _Y;
            ElementNumber = Index++;
            NeighboorsIndices = new List<int>();
            Visited = false;
        }

        //Override comparison operators
        public static bool operator ==(Vertex P1, Vertex P2)
        {
            return P1.Equals(P2);
        }
        public static bool operator !=(Vertex P1, Vertex P2)
        {
            return !P1.Equals(P2);
        }
    }
    struct VertexInfo
    {
        public double DistanceToStartpoint;
        public int IndexOfPriorPoint;
        public bool Visited;
    }

    struct Edge
    {
        //Object specific variables
        public double X1, X2;
        public double Y1, Y2;

        //Constructor 1
        public Edge(double _X1, double _Y1, double _X2, double _Y2)
        {
            X1 = _X1;
            Y1 = _Y1;

            X2 = _X2;
            Y2 = _Y2;
        }

        //Constructor 2
        public Edge(Vertex point1, Vertex point2)
        {
            X1 = point1.X;
            Y1 = point1.Y;

            X2 = point2.X;
            Y2 = point2.Y;
        }
    }
    struct RecursionVertexInfo
    {
        //public int PointIndex;
        public int Turns;
        public double Distance;
        public double Angle;
        public List<int> ListOfPriorPoints;

        public RecursionVertexInfo(RecursionVertexInfo recursionPointInfo)
        {
            Turns = recursionPointInfo.Turns;
            Distance = recursionPointInfo.Distance;
            Angle = recursionPointInfo.Angle;

            ListOfPriorPoints = recursionPointInfo.ListOfPriorPoints.ToList();
        }
    }
}
