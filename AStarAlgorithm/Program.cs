using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace AStarAlgorithm
{
    class Program
    {
        
        static void Main(string[] args)
        {

            Grid grid = new Grid(rows:50,columns:50);
            grid.InitializeGrid();

            Node source = new Node() { Location = new Tuple<int, int>(0, 0) };

            Node destination = new Node() { Location = new Tuple<int, int>(49, 49) };

            Text.WriteLine($"Grid contains {grid.Nodes.Length} Nodes", ConsoleColor.Blue);


            Text.WriteLine("-------------------------------- GRID VIEW ----------------------------------------", ConsoleColor.Cyan);

            RandomizeGridNodes(grid, source, destination, maxObstacles: 1250);

            //  NOTE: Consider commenting BuildingVisuals when running large grid sizes because Console Window might not display well
            //BuildGridVisuals(grid, source, destination);

            Text.WriteLine("--------------------------------A*STAR MAGIC ----------------------------------------", ConsoleColor.Cyan);

            
            Stopwatch stopwatch = new Stopwatch();

            stopwatch.Start();

            //  Simulate Single Agent Path Finding 
            //  Change to true if you want to test this
            if (false)
            {
                grid.AStarSearch(grid, source, destination);

            }

            //  Run multiple path finding operations in Parallel to simulate multiple Agents finding path simultaneously
            //  Change to true if you want to test this
            if (true)
            {
                Parallel.For(0, 10, index => { grid.AStarSearch(grid, source, destination); });
            }


            stopwatch.Stop();

            Text.WriteLine($"Time elapsed {stopwatch.Elapsed}", ConsoleColor.White);

            //  NOTE: Consider commenting BuildingVisuals when running large grid sizes because Console Window might not display well
            //BuildGridVisuals(grid, source, destination);
        }

        static void BuildGridVisuals(Grid grid,Node source, Node destination)
        {

            for (int row = 0; row < grid.Rows; row++)
            {
                for (int column = 0; column < grid.Columns; column++)
                {
                    Node currentNode = grid.Nodes[row, column];

                    // source node, paint it blue
                    if (source.Location.Item1 == row && source.Location.Item2 == column)
                    {
                        Text.Write("[*] \t", ConsoleColor.Blue);
                    }

                    // destination node, paint it green
                    else if (destination.Location.Item1 == row && destination.Location.Item2 == column)
                    {
                        Text.Write("[*] \t", ConsoleColor.Green);
                    }

                    // obstacle node, paint it red
                    else if (currentNode.Occupied == true)
                    {
                        Text.Write("[*] \t", ConsoleColor.Red);
                    }
                    else if (currentNode.IsPath == true)
                    {
                        Text.Write("[*] \t", ConsoleColor.Cyan);
                    }
                    // free node
                    else
                    {
                        Text.Write("[*] \t", ConsoleColor.White);
                    }
                }
                Console.WriteLine("\n");
            }


        }

        /// <summary>
        /// Randomly create obstacles in the grid
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="obstacles"></param>
        static void RandomizeGridNodes(Grid grid, Node source, Node destination, int maxObstacles = 10)
        {
            int obstaclesAdded = 0;

            Random random = new Random(DateTime.UtcNow.Millisecond);

            //  Generate a random number for each coordinate i and j
            //  Check if the obstacle is on the source or destination node, if it is generate another random number
            //  If it is not, assign that node as occupied and increment the obstacles added count

            int randI = random.Next(0, grid.Rows - 1);
            int randJ = random.Next(0, grid.Columns - 1);

            while (obstaclesAdded <= maxObstacles )
            {
                if ((source.Location.Item1 != randI && source.Location.Item2 != randJ) && (destination.Location.Item1 != randI && destination.Location.Item2 != randJ))
                {
                    grid.Nodes[randI, randJ].Occupied = true;
                }

                randI = random.Next(0, grid.Rows - 1);
                randJ = random.Next(0, grid.Columns - 1);

                obstaclesAdded++;

            }

        }



    }




}
