using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AStarAlgorithm
{
    public class Grid
    {
        public int Rows { get; protected set; }
        public int Columns { get; protected set; }

        public Node[,] Nodes { get; set; }

        public List<Node> Path { get; protected set; }

        public Grid(int rows = 10, int columns = 10)
        {
            Rows = rows;
            Columns = columns;
        }

        /// <summary>
        /// Create a row by column grid without obstacles
        /// </summary>
        /// <returns></returns>
        public void InitializeGrid()
        {
            Node [,]  grid = new Node[Rows,Columns];

            for (int row = 0; row < Rows; row++)
            {
                for (int column = 0; column < Columns; column++)
                {
                    Node node = new Node();
                    node.Location = new Tuple<int, int>(row,column);
                    node.Occupied = false;
                    grid[row, column] = node;
                }
            }
            Nodes = grid;
        }


        /// <summary>
        /// Checks whether a given node location is valid in the grid
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool IsValid(int row, int column)
        {
            if ((row >= 0 && row < Rows) && (column >= 0 && column < Columns))
            {
                return true;
            }
            return false;
        }
        
        /// <summary>
        /// Checks whether a given node in the grid is occupied or not
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public bool IsBlocked(Grid grid,int row, int column)
        {
            if (grid.Nodes[row,column].Occupied == true)
            {
                return true;
            }
            return false;
        }

        public bool IsDestination(int row, int column, Node destination)
        {
            Tuple<int, int> currentLocation = new Tuple<int, int>(row, column);
            if ((currentLocation.Item1 == destination.Location.Item1) && (currentLocation.Item2 == destination.Location.Item2))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// This function calculates the H (Heuristics) value of the node
        /// I used the Euclidean Distance Formula sqrt((current_node.x - destination_node.x)^2 + (current_node.y - destination_node.y)^2)
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        double CalculateHValue(int row, int col, Node destination)
        {

            double H = Math.Sqrt(Math.Pow((row - destination.Location.Item1), 2) + Math.Pow((col - destination.Location.Item2),2));

            return H;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <param name="destination"></param>
        /// <param name="D2"> Cost of moving diagonally</param>
        /// <returns></returns>
        double CalculateHValue(int row, int col, Node destination, double D2 = 1.0)
        {
            double D = 1.0; //   cost of moving in a straight line
            double dx = Math.Abs(row - destination.Location.Item1);
            double dy = Math.Abs(col - destination.Location.Item2);

            double H = D * (dx + dy) + (D2 - 2 * D) * Math.Min(dx, dy);

            return H;
        }
        /// <summary>
        /// This function traces path from source to destination 
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="destination"></param>
        public void TracePath(Node source, Node destination)
        {
            Path = new List<Node>();

            Node currentNode = destination;
            //  Add the destination first in the path list
            Path.Add(destination);

            //  While the current node is not the source node 
            while (currentNode.Parent != null)
            {
                currentNode.IsPath = true;
                Path.Add(currentNode.Parent);
                currentNode = currentNode.Parent;
            }

            Path.Add(currentNode);

        }
        /// <summary>
        /// This function finds the shortest path between a source node and a destination node according to A* Search Algorithm
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public void AStarSearch(Grid grid, Node source, Node destination)
        {
            //  If the source nodeis out of range
            if (IsValid(source.Location.Item1, source.Location.Item2) == false)
            {
                Text.WriteLine("Source Node is out of range", ConsoleColor.Yellow);
                return;
            }

            //  If the destination node is out of range
            if (IsValid(destination.Location.Item1, destination.Location.Item2) == false)
            {
                Text.WriteLine("Destination Node is out of range", ConsoleColor.Yellow);
                return;
            }

            //  Check if either the source or the destination is blocked
            if (IsBlocked(grid, source.Location.Item1, source.Location.Item2) == true || IsBlocked(grid, destination.Location.Item1, destination.Location.Item2) == true)
            {
                Text.WriteLine($"The source or destination is blocked", ConsoleColor.Yellow);
                return;
            }
            //  If the destination cell is the same as the source cell
            if (IsDestination(source.Location.Item1, source.Location.Item2, destination))
            {
                Text.WriteLine("We are already at destination", ConsoleColor.Green);
                return;
            }

            //  Create a closed dictionary and initialize it to empty
            Dictionary<Tuple<int, int>, Node> closedDictionary = new Dictionary<Tuple<int, int>, Node>();

            //  Initializing the starting node in the grid
            int i, j;
            i = source.Location.Item1; j = source.Location.Item2;
            grid.Nodes[i, j].F = 0.0;
            grid.Nodes[i, j].G = 0.0;
            grid.Nodes[i, j].H = 0.0;
            grid.Nodes[i, j].Parent = source;

            //  Create an open dictionary
            Dictionary<Tuple<int, int>, Node> openDictionary = new Dictionary<Tuple<int, int>, Node>();

            //  Add the stating node to the open dictionary and set its F as 0
            openDictionary.Add(source.Location, source);

            //  We set this boolean value as false because initially, the destination has not been reached
            bool foundDestination = false;


            while (openDictionary.Count != 0)
            {
                //  Get the first Node (vertex node) in the dictionary and remove it
                KeyValuePair<Tuple<int, int>, Node> vertexNode = openDictionary.First();

                openDictionary.Remove(vertexNode.Key, out Node currentNode);

                //  Add this vertex node to the closed list
                i = vertexNode.Key.Item1;
                j = vertexNode.Key.Item2;

                closedDictionary.Add(vertexNode.Key, currentNode);

                /* 
                   Generating all the 8 successor of this Vertex Node 

                       N.W   N   N.E 
                         \   |   / 
                          \  |  / 
                       W----Node----E 
                            / | \ 
                          /   |  \ 
                       S.W    S   S.E 

                   Node-->Popped Node/ (i, j) 
                   N -->  North       (i-1, j) 
                   S -->  South       (i+1, j) 
                   E -->  East        (i, j+1) 
                   W -->  West           (i, j-1) 
                   N.E--> North-East  (i-1, j+1) 
                   N.W--> North-West  (i-1, j-1) 
                   S.E--> South-East  (i+1, j+1) 
                   S.W--> South-West  (i+1, j-1)

                */

                //  Store the new g, h and f values of the 8 successors here during each successor iteration
                double gNew, hNew, fNew = 0.0;

                //  -------------------------- 1st Successor (North) --------------------

                //  Only process this cell if this is a valid one
                

                if (IsValid(i - 1, j) == true)
                {
                    int successorI = i - 1;   //  successors i coordinate
                    int successorJ = j;       //  successors j coordinate

                    //  Get the parent node from the close dictionary because it must be there.
                    closedDictionary.TryGetValue(vertexNode.Key, out Node parent);

                    //  If the destination node is the same as the current successor node
                    if (IsDestination(successorI, successorJ, destination) == true)
                    {
                        //  Set the parent of the destination node in the grid and the destination node object
                        grid.Nodes[successorI, successorJ].Parent = parent;
                        destination.Parent = parent;


                        Text.WriteLine($"Destination Node found at Location (i: {successorI}, j: {successorJ})", ConsoleColor.Green);

                        TracePath(source, destination);
                        foundDestination = true;
                        return;

                    }

                    //  If the successor node is already in the closed list or if it is blocked, ignore it. If it is neither blocked nor in
                    //  closed list then do the following below

                    else if (IsBlocked(grid, successorI, successorJ) == false && !closedDictionary.ContainsKey(new Tuple<int, int>(successorI, successorJ)))
                    {
                        gNew = grid.Nodes[i, j].G + 1.0;
                        hNew = CalculateHValue(successorI, successorJ, destination,1.0);
                        fNew = gNew + hNew;

                        /*
                           1. If this current successor is not on the open list, add it to the open list.
                           2. Make the current node the parent of this successor.
                           3. Record the f, g and h values of this successor node
                                               OR
                           4. If it is on the open list already, check to see if this path to the successor is better
                              by using the 'f' value as the measure 
                        */

                        if (!openDictionary.ContainsKey(new Tuple<int, int>(successorI, successorJ)) || (openDictionary.TryGetValue(new Tuple<int, int>(successorI, successorJ), out Node existingSuccessor) && existingSuccessor.F > fNew))
                        {
                            //  Build the successorNode object
                            Node newSuccessorNode = new Node();
                            newSuccessorNode.F = fNew;
                            newSuccessorNode.G = gNew;
                            newSuccessorNode.H = hNew;
                            newSuccessorNode.Location = new Tuple<int, int>(successorI, successorJ);
                            newSuccessorNode.Parent = parent;

                            //  Attempt to add the new successor node, but if it exists and the
                            //  F cost is less, then update the value in the open list
                            bool addStatus = openDictionary.TryAdd(new Tuple<int, int>(successorI, successorJ), newSuccessorNode);

                            if (addStatus == false)
                            {
                                // This means that the node already exists in the open list. Then we update the value
                                openDictionary[new Tuple<int, int>(successorI, successorJ)] = newSuccessorNode;
                            }

                            //  Update the details of this node on the main grid
                            grid.Nodes[successorI, successorJ] = newSuccessorNode;
                        }

                    }


                }

                //  -------------------------- 2nd Successor (South) --------------------

                //  Only process this cell if this is a valid one
                if (IsValid(i + 1, j) == true)
                {
                    int successorI = i + 1;   //  successors i coordinate
                    int successorJ = j;       //  successors j coordinate

                    //  Get the parent node from the close dictionary because it must be there.
                    closedDictionary.TryGetValue(vertexNode.Key, out Node parent);

                    //  If the destination node is the same as the current successor node
                    if (IsDestination(successorI,successorJ , destination) == true)
                    {
                        //  Set the parent of the destination node
                        grid.Nodes[successorI, successorJ ].Parent = parent;
                        destination.Parent = parent;

                        Text.WriteLine($"Destination Node found at Location (i: {successorI}, j: {successorJ})", ConsoleColor.Green);

                        TracePath(source, destination);
                        foundDestination = true;
                        return;

                    }

                    //  If the successor node is already in the closed list or if it is blocked, ignore it. If it is neither blocked nor in
                    //  closed list then do the following below

                    else if (IsBlocked(grid, successorI, successorJ) == false && !closedDictionary.ContainsKey(new Tuple<int, int>(successorI, successorJ)))
                    {
                        gNew = grid.Nodes[i, j].G + 1.0;
                        hNew = CalculateHValue(successorI, successorJ, destination, 1.0);
                        fNew = gNew + hNew;

                        /*
                           1. If this current successor is not on the open list, add it to the open list.
                           2. Make the current node the parent of this successor.
                           3. Record the f, g and h values of this successor node
                                               OR
                           4. If it is on the open list already, check to see if this path to the successor is better
                              by using the 'f' value as the measure 
                        */

                        if (!openDictionary.ContainsKey(new Tuple<int, int>(successorI, successorJ)) || (openDictionary.TryGetValue(new Tuple<int, int>(successorI, successorJ), out Node existingSuccessor) && existingSuccessor.F > fNew))
                        {
                            //  Build the successorNode object
                            Node newSuccessorNode = new Node();
                            newSuccessorNode.F = fNew;
                            newSuccessorNode.G = gNew;
                            newSuccessorNode.H = hNew;
                            newSuccessorNode.Location = new Tuple<int, int>(successorI, successorJ);
                            newSuccessorNode.Parent = parent;

                            //  Attempt to add the new successor node, but if it exists and the
                            //  F cost is less, then update the value in the open list
                            bool addStatus = openDictionary.TryAdd(new Tuple<int, int>(successorI, successorJ), newSuccessorNode);

                            if (addStatus == false)
                            {
                                // This means that the node already exists in the open list. Then we update the value
                                openDictionary[new Tuple<int, int>(successorI, successorJ)] = newSuccessorNode;
                            }

                            //  Update the details of this node on the main grid
                            grid.Nodes[successorI, successorJ] = newSuccessorNode;
                        }

                    }


                }

                //  -------------------------- 3rd Successor (East) --------------------

                //  Only process this cell if this is a valid one
                if (IsValid(i, j + 1) == true)
                {
                    int successorI = i;         //  successors i coordinate
                    int successorJ = j + 1;       //  successors j coordinate

                    //  Get the parent node from the close dictionary because it must be there.
                    closedDictionary.TryGetValue(vertexNode.Key, out Node parent);

                    //  If the destination node is the same as the current successor node
                    if (IsDestination(successorI, successorJ, destination) == true)
                    {
                        //  Set the parent of the destination node
                        grid.Nodes[successorI, successorJ].Parent = parent;
                        destination.Parent = parent;

                        Text.WriteLine($"Destination Node found at Location (i: {successorI}, j: {successorJ})", ConsoleColor.Green);

                        TracePath(source, destination);
                        foundDestination = true;
                        return;

                    }

                    //  If the successor node is already in the closed list or if it is blocked, ignore it. If it is neither blocked nor in
                    //  closed list then do the following below

                    else if (IsBlocked(grid, successorI, successorJ) == false && !closedDictionary.ContainsKey(new Tuple<int, int>(successorI, successorJ)))
                    {
                        gNew = grid.Nodes[i, j].G + 1.0;
                        hNew = CalculateHValue(successorI, successorJ, destination, 1.0);
                        fNew = gNew + hNew;

                        /*
                           1. If this current successor is not on the open list, add it to the open list.
                           2. Make the current node the parent of this successor.
                           3. Record the f, g and h values of this successor node
                                               OR
                           4. If it is on the open list already, check to see if this path to the successor is better
                              by using the 'f' value as the measure 
                        */

                        if (!openDictionary.ContainsKey(new Tuple<int, int>(successorI, successorJ)) || (openDictionary.TryGetValue(new Tuple<int, int>(successorI, successorJ), out Node existingSuccessor) && existingSuccessor.F > fNew))
                        {
                            //  Build the successorNode object
                            Node newSuccessorNode = new Node();
                            newSuccessorNode.F = fNew;
                            newSuccessorNode.G = gNew;
                            newSuccessorNode.H = hNew;
                            newSuccessorNode.Location = new Tuple<int, int>(successorI, successorJ);
                            newSuccessorNode.Parent = parent;

                            //  Attempt to add the new successor node, but if it exists and the
                            //  F cost is less, then update the value in the open list
                            bool addStatus = openDictionary.TryAdd(new Tuple<int, int>(successorI, successorJ), newSuccessorNode);

                            if (addStatus == false)
                            {
                                // This means that the node already exists in the open list. Then we update the value
                                openDictionary[new Tuple<int, int>(successorI, successorJ)] = newSuccessorNode;
                            }

                            //  Update the details of this node on the main grid
                            grid.Nodes[successorI, successorJ] = newSuccessorNode;
                        }

                    }


                }

                //  -------------------------- 4th Successor (West) --------------------

                //  Only process this cell if this is a valid one
                if (IsValid(i, j - 1) == true)
                {
                    int successorI = i;   //  successors i coordinate
                    int successorJ = j - 1;       //  successors j coordinate

                    //  Get the parent node from the close dictionary because it must be there.
                    closedDictionary.TryGetValue(vertexNode.Key, out Node parent);

                    //  If the destination node is the same as the current successor node
                    if (IsDestination(successorI, successorJ, destination) == true)
                    {
                        //  Set the parent of the destination node
                        grid.Nodes[successorI, successorJ].Parent = parent;
                        destination.Parent = parent;

                        Text.WriteLine($"Destination Node found at Location (i: {successorI}, j: {successorJ})", ConsoleColor.Green);

                        TracePath(source, destination);
                        foundDestination = true;
                        return;

                    }

                    //  If the successor node is already in the closed list or if it is blocked, ignore it. If it is neither blocked nor in
                    //  closed list then do the following below

                    else if (IsBlocked(grid, successorI, successorJ) == false && !closedDictionary.ContainsKey(new Tuple<int, int>(successorI, successorJ)))
                    {
                        gNew = grid.Nodes[i, j].G + 1.0;
                        hNew = CalculateHValue(successorI, successorJ, destination, 1.0);
                        fNew = gNew + hNew;

                        /*
                           1. If this current successor is not on the open list, add it to the open list.
                           2. Make the current node the parent of this successor.
                           3. Record the f, g and h values of this successor node
                                               OR
                           4. If it is on the open list already, check to see if this path to the successor is better
                              by using the 'f' value as the measure 
                        */

                        if (!openDictionary.ContainsKey(new Tuple<int, int>(successorI, successorJ)) || (openDictionary.TryGetValue(new Tuple<int, int>(successorI, successorJ), out Node existingSuccessor) && existingSuccessor.F > fNew))
                        {
                            //  Build the successorNode object
                            Node newSuccessorNode = new Node();
                            newSuccessorNode.F = fNew;
                            newSuccessorNode.G = gNew;
                            newSuccessorNode.H = hNew;
                            newSuccessorNode.Location = new Tuple<int, int>(successorI, successorJ);
                            newSuccessorNode.Parent = parent;

                            //  Attempt to add the new successor node, but if it exists and the
                            //  F cost is less, then update the value in the open list
                            bool addStatus = openDictionary.TryAdd(new Tuple<int, int>(successorI, successorJ), newSuccessorNode);

                            if (addStatus == false)
                            {
                                // This means that the node already exists in the open list. Then we update the value
                                openDictionary[new Tuple<int, int>(successorI, successorJ)] = newSuccessorNode;
                            }

                            //  Update the details of this node on the main grid
                            grid.Nodes[successorI, successorJ] = newSuccessorNode;
                        }

                    }


                }

                //  -------------------------- 5th Successor (North East) --------------------

                //  Only process this cell if this is a valid one
                if (IsValid(i - 1, j + 1) == true)
                {
                    int successorI = i - 1;         //  successors i coordinate
                    int successorJ = j + 1;         //  successors j coordinate

                    //  Get the parent node from the close dictionary because it must be there.
                    closedDictionary.TryGetValue(vertexNode.Key, out Node parent);

                    //  If the destination node is the same as the current successor node
                    if (IsDestination(successorI, successorJ, destination) == true)
                    {
                        //  Set the parent of the destination node
                        grid.Nodes[successorI, successorJ].Parent = parent;
                        destination.Parent = parent;

                        Text.WriteLine($"Destination Node found at Location (i: {successorI}, j: {successorJ})", ConsoleColor.Green);

                        TracePath(source, destination);
                        foundDestination = true;
                        return;

                    }

                    //  If the successor node is already in the closed list or if it is blocked, ignore it. If it is neither blocked nor in
                    //  closed list then do the following below

                    else if (IsBlocked(grid, successorI, successorJ) == false && !closedDictionary.ContainsKey(new Tuple<int, int>(successorI, successorJ)))
                    {
                        gNew = grid.Nodes[i, j].G + 1.0;
                        hNew = CalculateHValue(successorI, successorJ, destination, 1.0);
                        fNew = gNew + hNew;

                        /*
                           1. If this current successor is not on the open list, add it to the open list.
                           2. Make the current node the parent of this successor.
                           3. Record the f, g and h values of this successor node
                                               OR
                           4. If it is on the open list already, check to see if this path to the successor is better
                              by using the 'f' value as the measure 
                        */

                        if (!openDictionary.ContainsKey(new Tuple<int, int>(successorI, successorJ)) || (openDictionary.TryGetValue(new Tuple<int, int>(successorI, successorJ), out Node existingSuccessor) && existingSuccessor.F > fNew))
                        {
                            //  Build the successorNode object
                            Node newSuccessorNode = new Node();
                            newSuccessorNode.F = fNew;
                            newSuccessorNode.G = gNew;
                            newSuccessorNode.H = hNew;
                            newSuccessorNode.Location = new Tuple<int, int>(successorI, successorJ);
                            newSuccessorNode.Parent = parent;

                            //  Attempt to add the new successor node, but if it exists and the
                            //  F cost is less, then update the value in the open list
                            bool addStatus = openDictionary.TryAdd(new Tuple<int, int>(successorI, successorJ), newSuccessorNode);

                            if (addStatus == false)
                            {
                                // This means that the node already exists in the open list. Then we update the value
                                openDictionary[new Tuple<int, int>(successorI, successorJ)] = newSuccessorNode;
                            }

                            //  Update the details of this node on the main grid
                            grid.Nodes[successorI, successorJ] = newSuccessorNode;
                        }

                    }


                }

                //  -------------------------- 6th Successor (North West) --------------------

                //  Only process this cell if this is a valid one
                if (IsValid(i - 1, j - 1) == true)
                {
                    int successorI = i - 1;         //  successors i coordinate
                    int successorJ = j - 1;         //  successors j coordinate

                    //  Get the parent node from the close dictionary because it must be there.
                    closedDictionary.TryGetValue(vertexNode.Key, out Node parent);

                    //  If the destination node is the same as the current successor node
                    if (IsDestination(successorI, successorJ, destination) == true)
                    {
                        //  Set the parent of the destination node
                        grid.Nodes[successorI, successorJ].Parent = parent;
                        destination.Parent = parent;

                        Text.WriteLine($"Destination Node found at Location (i: {successorI}, j: {successorJ})", ConsoleColor.Green);

                        TracePath(source, destination);
                        foundDestination = true;
                        return;

                    }

                    //  If the successor node is already in the closed list or if it is blocked, ignore it. If it is neither blocked nor in
                    //  closed list then do the following below

                    else if (IsBlocked(grid, successorI, successorJ) == false && !closedDictionary.ContainsKey(new Tuple<int, int>(successorI, successorJ)))
                    {
                        gNew = grid.Nodes[i, j].G + 1.0;
                        hNew = CalculateHValue(successorI, successorJ, destination, 1.0);
                        fNew = gNew + hNew;

                        /*
                           1. If this current successor is not on the open list, add it to the open list.
                           2. Make the current node the parent of this successor.
                           3. Record the f, g and h values of this successor node
                                               OR
                           4. If it is on the open list already, check to see if this path to the successor is better
                              by using the 'f' value as the measure 
                        */

                        if (!openDictionary.ContainsKey(new Tuple<int, int>(successorI, successorJ)) || (openDictionary.TryGetValue(new Tuple<int, int>(successorI, successorJ), out Node existingSuccessor) && existingSuccessor.F > fNew))
                        {
                            //  Build the successorNode object
                            Node newSuccessorNode = new Node();
                            newSuccessorNode.F = fNew;
                            newSuccessorNode.G = gNew;
                            newSuccessorNode.H = hNew;
                            newSuccessorNode.Location = new Tuple<int, int>(successorI, successorJ);
                            newSuccessorNode.Parent = parent;

                            //  Attempt to add the new successor node, but if it exists and the
                            //  F cost is less, then update the value in the open list
                            bool addStatus = openDictionary.TryAdd(new Tuple<int, int>(successorI, successorJ), newSuccessorNode);

                            if (addStatus == false)
                            {
                                // This means that the node already exists in the open list. Then we update the value
                                openDictionary[new Tuple<int, int>(successorI, successorJ)] = newSuccessorNode;
                            }

                            //  Update the details of this node on the main grid
                            grid.Nodes[successorI, successorJ] = newSuccessorNode;
                        }

                    }


                }

                //  -------------------------- 7th Successor (South East) --------------------

                //  Only process this cell if this is a valid one
                if (IsValid(i + 1, j + 1) == true)
                {
                    int successorI = i + 1;   //  successors i coordinate
                    int successorJ = j + 1;       //  successors j coordinate

                    //  Get the parent node from the close dictionary because it must be there.
                    closedDictionary.TryGetValue(vertexNode.Key, out Node parent);

                    //  If the destination node is the same as the current successor node
                    if (IsDestination(successorI, successorJ, destination) == true)
                    {
                        //  Set the parent of the destination node
                        grid.Nodes[successorI, successorJ].Parent = parent;
                        destination.Parent = parent;

                        Text.WriteLine($"Destination Node found at Location (i: {successorI}, j: {successorJ})", ConsoleColor.Green);

                        TracePath(source, destination);
                        foundDestination = true;
                        return;

                    }

                    //  If the successor node is already in the closed list or if it is blocked, ignore it. If it is neither blocked nor in
                    //  closed list then do the following below

                    else if (IsBlocked(grid, successorI, successorJ) == false && !closedDictionary.ContainsKey(new Tuple<int, int>(successorI, successorJ)))
                    {
                        gNew = grid.Nodes[i, j].G + 1.0;
                        hNew = CalculateHValue(successorI, successorJ, destination, 1.0);
                        fNew = gNew + hNew;

                        /*
                           1. If this current successor is not on the open list, add it to the open list.
                           2. Make the current node the parent of this successor.
                           3. Record the f, g and h values of this successor node
                                               OR
                           4. If it is on the open list already, check to see if this path to the successor is better
                              by using the 'f' value as the measure 
                        */

                        if (!openDictionary.ContainsKey(new Tuple<int, int>(successorI, successorJ)) || (openDictionary.TryGetValue(new Tuple<int, int>(successorI, successorJ), out Node existingSuccessor) && existingSuccessor.F > fNew))
                        {
                            //  Build the successorNode object
                            Node newSuccessorNode = new Node();
                            newSuccessorNode.F = fNew;
                            newSuccessorNode.G = gNew;
                            newSuccessorNode.H = hNew;
                            newSuccessorNode.Location = new Tuple<int, int>(successorI, successorJ);
                            newSuccessorNode.Parent = parent;

                            //  Attempt to add the new successor node, but if it exists and the
                            //  F cost is less, then update the value in the open list
                            bool addStatus = openDictionary.TryAdd(new Tuple<int, int>(successorI, successorJ), newSuccessorNode);

                            if (addStatus == false)
                            {
                                // This means that the node already exists in the open list. Then we update the value
                                openDictionary[new Tuple<int, int>(successorI, successorJ)] = newSuccessorNode;
                            }

                            //  Update the details of this node on the main grid
                            grid.Nodes[successorI, successorJ] = newSuccessorNode;
                        }

                    }


                }

                //  -------------------------- 8th Successor (South West) --------------------

                //  Only process this cell if this is a valid one
                if (IsValid(i + 1, j - 1) == true)
                {
                    int successorI = i + 1;   //  successors i coordinate
                    int successorJ = j - 1;       //  successors j coordinate

                    //  Get the parent node from the close dictionary because it must be there.
                    closedDictionary.TryGetValue(vertexNode.Key, out Node parent);

                    //  If the destination node is the same as the current successor node
                    if (IsDestination(successorI, successorJ, destination) == true)
                    {
                        //  Set the parent of the destination node
                        grid.Nodes[successorI, successorJ].Parent = parent;
                        destination.Parent = parent;

                        Text.WriteLine($"Destination Node found at Location (i: {successorI}, j: {successorJ})", ConsoleColor.Green);

                        TracePath(source, destination);
                        foundDestination = true;
                        return;

                    }

                    //  If the successor node is already in the closed list or if it is blocked, ignore it. If it is neither blocked nor in
                    //  closed list then do the following below

                    else if (IsBlocked(grid, successorI, successorJ) == false && !closedDictionary.ContainsKey(new Tuple<int, int>(successorI, successorJ)))
                    {
                        gNew = grid.Nodes[i, j].G + 1.0;
                        hNew = CalculateHValue(successorI, successorJ, destination, 1.0);
                        fNew = gNew + hNew;

                        /*
                           1. If this current successor is not on the open list, add it to the open list.
                           2. Make the current node the parent of this successor.
                           3. Record the f, g and h values of this successor node
                                               OR
                           4. If it is on the open list already, check to see if this path to the successor is better
                              by using the 'f' value as the measure 
                        */

                        if (!openDictionary.ContainsKey(new Tuple<int, int>(successorI, successorJ)) || (openDictionary.TryGetValue(new Tuple<int, int>(successorI, successorJ), out Node existingSuccessor) && existingSuccessor.F > fNew))
                        {
                            //  Build the successorNode object
                            Node newSuccessorNode = new Node();
                            newSuccessorNode.F = fNew;
                            newSuccessorNode.G = gNew;
                            newSuccessorNode.H = hNew;
                            newSuccessorNode.Location = new Tuple<int, int>(successorI, successorJ);
                            newSuccessorNode.Parent = parent;

                            //  Attempt to add the new successor node, but if it exists and the
                            //  F cost is less, then update the value in the open list
                            bool addStatus = openDictionary.TryAdd(new Tuple<int, int>(successorI, successorJ), newSuccessorNode);

                            if (addStatus == false)
                            {
                                // This means that the node already exists in the open list. Then we update the value
                                openDictionary[new Tuple<int, int>(successorI, successorJ)] = newSuccessorNode;
                            }

                            //  Update the details of this node on the main grid
                            grid.Nodes[successorI, successorJ] = newSuccessorNode;
                        }

                    }


                }


            }

            //  When the destination node is not found and the open list is empty, then we conclude that we failed to reach the 
            //  destination node. This may happen when there is no way to destination node due to obstacles

            if (foundDestination == false)
            {
                Text.WriteLine("Failed to find the destination node", ConsoleColor.Yellow);
            }

            return;
        } 
        public Node[][] AddObstaclestoGrid()
        {
            return null;
        }
    }
}
