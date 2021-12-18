using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Testing
{
    public class Maze
    {
        public Maze(int _size)
        {
            Console.Clear();
            size = _size;
        }

        public void Run()
        {
            //Declarations
            checkedPoints = new List<Point>();
            visitedPoints = new List<Point>();

            //Generate graph
            int[,] graph = new int[size, size];

            //Generate start and end verticies
            start = GetRandomEdgeVert(size);
            end   = GetRandomEdgeVert(size);

            Console.WriteLine($"\nStart: ({start.X},{start.Y})");
            Console.WriteLine($"End: ({end.X},{end.Y})");

            while ((end.X == start.X) && (end.Y == start.Y))
            {
                end = GetRandomEdgeVert(size);
            }

            //Weight the graph 
            weightedGraph = new int[size, size];

            for (int i = 0; i < size; i++) //Y
            {
                for (int j = 0; j < size; j++) //X
                {
                    weightedGraph[j, i] = Math.Abs(j - end.X) + Math.Abs(i - end.Y);

                    /*
                    if (j == start.X && i == start.Y)
                    {
                        Console.Write("\tX");
                    }
                    else
                    {
                        Console.Write($"\t{weightedGraph[j, i]}");
                    }
                    */
                }
                //Console.WriteLine();
            }

            //Begin Main Loop
            visitedPoints.Add(start);
            Point currentPoint = start;
            int count = 1;

            while (currentPoint != end)
            {
                Console.WriteLine($"Point {count}: ({currentPoint.X},{currentPoint.Y})...");
                if (Move(ref currentPoint) == -1)
                {
                    break;
                }
                count++;
            }

            //Visual Output
            /*
            Console.WriteLine();
            for (int i = 0; i < size; i++) //Num rows
            {
                for (int j = 0; j < size; j++) //Num columns
                {
                    Point tempPoint = new Point(j, i);
                    if (visitedPoints.Contains(tempPoint))
                    {
                        Console.Write("X");
                    }
                    else
                    {
                        Console.Write("0");
                    }
                }
                Console.WriteLine();
            }
            Console.ReadKey();
            */
        }

        private static Point GetRandomEdgeVert(int _size)
        {
            int _X, _Y;
            if (rand.Next(0, 2) == 0) //choose vert. or horz. edge
            {
                //vert.
                _X = (rand.Next(0, 2) == 0) ? 0 : _size - 1; //choose left or right side
                _Y = rand.Next(0, _size - 1);
            }
            else
            {
                //horz.
                _Y = (rand.Next(0, 2) == 0) ? 0 : _size - 1; //choose top or bottom side
                _X = rand.Next(0, _size - 1);
            }

            return new Point(_X, _Y);
        }
        private int TestPoint(Point testPoint)
        {
            //Clear
            checkedPoints.Clear();

            //Run greedy on that point
            while (testPoint != end)
            {
                try
                {
                    Console.WriteLine($"Testing Point: ({testPoint.X},{testPoint.Y})...");
                    testPoint = RunGreedy(testPoint);
                }
                catch (NoSolution)
                {
                    Console.WriteLine($"No solution found.");
                    return -1;
                }
            }

            return 0;
        }
        private Point RunGreedy(Point currentPoint)
        {
            //Add current node to list of checked nodes
            checkedPoints.Add(currentPoint);

            //Find the smallest unvisited adjacent node
            int minValue = weightedGraph[currentPoint.X, currentPoint.Y];
            Point nextPoint = new Point();
            bool found = false;

            //Check Left
            Point left = new Point(currentPoint.X - 1, currentPoint.Y);
            if (
                currentPoint.X - 1 >= 0 &&
                !checkedPoints.Contains(left) &&
                !visitedPoints.Contains(left) &&

                weightedGraph[currentPoint.X - 1, currentPoint.Y] < minValue
                )
            {
                minValue = weightedGraph[currentPoint.X - 1, currentPoint.Y];
                nextPoint = left;
                found = true;
            }

            //Check Top
            Point top = new Point(currentPoint.X, currentPoint.Y - 1);
            if (
                currentPoint.Y - 1 >= 0 &&
                !checkedPoints.Contains(top) &&
                !visitedPoints.Contains(top) &&
                weightedGraph[currentPoint.X, currentPoint.Y - 1] < minValue
                )
            {
                minValue = weightedGraph[currentPoint.X, currentPoint.Y - 1];
                nextPoint = top;
                found = true;
            }

            //Check Right
            Point right = new Point(currentPoint.X + 1, currentPoint.Y);
            if (
                currentPoint.X + 1 < weightedGraph.GetLength(0) &&
                !checkedPoints.Contains(right) &&
                !visitedPoints.Contains(right) &&
                weightedGraph[currentPoint.X + 1, currentPoint.Y] < minValue
                )
            {
                minValue = weightedGraph[currentPoint.X + 1, currentPoint.Y];
                nextPoint = right;
                found = true;
            }

            //Check Bottom
            Point bottom = new Point(currentPoint.X, currentPoint.Y + 1);
            if (
                currentPoint.Y + 1 < weightedGraph.GetLength(1) &&
                !checkedPoints.Contains(bottom) &&
                !visitedPoints.Contains(bottom) &&
                weightedGraph[currentPoint.X, currentPoint.Y + 1] < minValue
                )
            {
                minValue = weightedGraph[currentPoint.X, currentPoint.Y + 1];
                nextPoint = bottom;
                found = true;
            }

            //Return the next point to check
            if (!found)
            {
                throw new NoSolution();
            }
            return nextPoint;
        }
        private int RunDjikstra(Point currentPoint)
        {
            int largeValue = 1000;
            int sum = 0;

            //Create a set of unchecked nodes
            int[,] unvisitedNodes = new int[size, size];
            List<Point> unvisitedNodes_List = new List<Point>();

            //Assign initial tentative distances
            for (int i = 0; i < size; i++) //Y
            {
                for (int j = 0; j < size; j++) //X
                {
                    unvisitedNodes[j, i] = largeValue;
                    unvisitedNodes_List.Add(new Point(j, i));
                }
            }
            unvisitedNodes[currentPoint.X, currentPoint.Y] = 0;

            int visits = 0;
            while (
                   unvisitedNodes_List.Contains(end) &&           //Destination node is still in the unvisited set 
                   sum != unvisitedNodes_List.Count * largeValue  //All unvisited nodes are not infinity
                   )
            {
                if (visits > 100)
                {
                    int k = 4;
                }

                //Check all valid unvisited neighbors and assign values
                //Left
                Point left = new Point(currentPoint.X - 1, currentPoint.Y);
                if (
                    currentPoint.X - 1 >= 0 &&
                    unvisitedNodes_List.Contains(left) &&
                    !visitedPoints.Contains(left)
                    )
                {
                    unvisitedNodes[left.X, left.Y] = unvisitedNodes[currentPoint.X, currentPoint.Y] + 1;
                }

                //Top
                Point top = new Point(currentPoint.X, currentPoint.Y - 1);
                if (
                    currentPoint.Y - 1 >= 0 &&
                    unvisitedNodes_List.Contains(top) &&
                    !visitedPoints.Contains(top)
                    )
                {
                    unvisitedNodes[top.X, top.Y] = unvisitedNodes[currentPoint.X, currentPoint.Y] + 1;
                }

                //Right
                Point right = new Point(currentPoint.X + 1, currentPoint.Y);
                if (
                    currentPoint.X + 1 < weightedGraph.GetLength(0) &&
                    unvisitedNodes_List.Contains(right) &&
                    !visitedPoints.Contains(right)
                    )
                {
                    unvisitedNodes[right.X, right.Y] = unvisitedNodes[currentPoint.X, currentPoint.Y] + 1;
                }

                //Bottom
                Point bottom = new Point(currentPoint.X, currentPoint.Y + 1);
                if (
                    currentPoint.Y + 1 < weightedGraph.GetLength(1) &&
                    unvisitedNodes_List.Contains(bottom) &&
                    !visitedPoints.Contains(bottom)
                    )
                {
                    unvisitedNodes[bottom.X, bottom.Y] = unvisitedNodes[currentPoint.X, currentPoint.Y] + 1;
                }

                //Remove current node from the unvisited set
                unvisitedNodes_List.Remove(currentPoint);
                sum = 0;
                foreach (Point point in unvisitedNodes_List)
                {
                    sum += unvisitedNodes[point.X, point.Y];
                }

                //Set new node 
                int minValue = largeValue;
                Point minLocation = new Point();

                foreach (Point point in unvisitedNodes_List)
                {
                    if (unvisitedNodes[point.X, point.Y] < minValue)
                    {
                        minValue = unvisitedNodes[point.X, point.Y];
                        minLocation = point;
                    }
                }

                currentPoint = minLocation;
                visits++;
            }

            if (unvisitedNodes_List.Contains(end))
            {
                return -1;
            }

            return 0;
        }

        private int Move(ref Point currentPoint)
        {
            List<int> directionsToCheck = new List<int> { 0, 1, 2, 3 };

            while (directionsToCheck.Count > 0)
            {
                //Pick a random direction
                int index = rand.Next(0, directionsToCheck.Count);
                int direction = directionsToCheck[index];

                //Check that direction
                switch (direction)
                {
                    case 0:
                        Console.WriteLine($"Checking Left...");
                        //Get the left point
                        Point left = new Point(currentPoint.X - 1, currentPoint.Y);

                        if (
                            currentPoint.X - 1 >= 0 &&
                            !visitedPoints.Contains(left)
                            )
                        {
                            //Send the left point to the djikstra algo as a test point
                            if (RunDjikstra(left) != -1)
                            {
                                //If djikstra doesnt error out, we're done here
                                visitedPoints.Add(left);
                                currentPoint = left;
                                return 0;
                            }
                            else
                            {
                                directionsToCheck.RemoveAt(index);
                                break;
                            }
                        }
                        else
                        {
                            directionsToCheck.RemoveAt(index);
                            break;
                        }


                    case 1:
                        Console.WriteLine($"Checking Top...");
                        Point top = new Point(currentPoint.X, currentPoint.Y - 1);
                        if (
                            currentPoint.Y - 1 >= 0 &&
                            !visitedPoints.Contains(top)
                            )
                        {
                            if (RunDjikstra(top) != -1)
                            {
                                visitedPoints.Add(top);
                                currentPoint = top;
                                return 0;
                            }
                            else
                            {
                                directionsToCheck.RemoveAt(index);
                                break;
                            }
                        }
                        else
                        {
                            directionsToCheck.RemoveAt(index);
                            break;
                        }
                    case 2:
                        Console.WriteLine($"Checking Right...");
                        Point right = new Point(currentPoint.X + 1, currentPoint.Y);

                        if (
                            currentPoint.X + 1 < weightedGraph.GetLength(0) &&
                            !visitedPoints.Contains(right)
                            )
                        {
                            if (RunDjikstra(right) != -1)
                            {
                                visitedPoints.Add(right);
                                currentPoint = right;
                                return 0;
                            }
                            else
                            {
                                directionsToCheck.RemoveAt(index);
                                break;
                            }
                        }
                        else
                        {
                            directionsToCheck.RemoveAt(index);
                            break;
                        }

                    case 3:
                        Console.WriteLine($"Checking Bottom...");
                        Point bottom = new Point(currentPoint.X, currentPoint.Y + 1);

                        if (
                            currentPoint.Y + 1 < weightedGraph.GetLength(0) &&
                            !visitedPoints.Contains(bottom)
                            )
                        {
                            if (RunDjikstra(bottom) != -1)
                            {
                                visitedPoints.Add(bottom);
                                currentPoint = bottom;
                                return 0;
                            }
                            else
                            {
                                directionsToCheck.RemoveAt(index);
                                break;
                            }
                        }
                        else
                        {
                            directionsToCheck.RemoveAt(index);
                            break;
                        }
                }
            }

            return -1;
        }

        //Parameters
        private static readonly Random rand = new Random();
        public int size { get; set; }
        public Point end { get; set; }
        public Point start { get; set; }
        public List<Point> visitedPoints { get; set; }
        private static List<Point> checkedPoints { get; set; }
        public int[,] weightedGraph { get; set; }
    }

    public class NoSolution : Exception
    {
        public NoSolution() { }

    }
}
