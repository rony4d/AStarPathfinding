using System;
using System.Collections.Generic;
using System.Text;

namespace AStarAlgorithm
{
    public class Node
    {
        public Tuple<int,int> Location { get; set; }
        public double G { get; set; }
        public double H { get; set; }
        public double F { get; set; }
        public Node Parent { get; set; }
        public bool Occupied { get; set; }
        public bool IsPath { get; set; }


    }
}
