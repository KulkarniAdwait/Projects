using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AstarPathFinding
{
    public class PathFinder
    {
        private const int MOVE_COST = 10;
        private int gridRows;
        private int gridColumns;

        List<Vector2> openList = new List<Vector2>();
        List<Vector2> closedList = new List<Vector2>();
        Vector2[,] parentList;

        bool[,] isWalkable;
        int[,] F;
        int[,] G;
        int[,] H;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gridRows"></param>
        /// <param name="gridColumns"></param>
        /// <param name="isWalkable"></param>
        public PathFinder(int gridRows, int gridColumns, bool[,] isWalkable)
        {
            this.gridRows = gridRows;
            this.gridColumns = gridColumns;

            init(isWalkable);
           
        }

        private void init(bool[,] isWalkable)
        {
            this.isWalkable = isWalkable;   
            parentList = new Vector2[gridRows, gridColumns];

            F = new int[gridRows, gridColumns];
            G = new int[gridRows, gridColumns];
            H = new int[gridRows, gridColumns];
            for (int i = 0; i < gridRows; i++)
            {
                for (int j = 0; j < gridColumns; j++)
                {
                    F[i, j] = int.MaxValue;
                    G[i, j] = int.MaxValue;
                    H[i, j] = int.MaxValue;
                }
            }
        }

        public Vector2[] findPath(Vector2 start, Vector2 goal, bool[,] isWalkable)
        {
            if (start.Equals(goal))
            {
                return null;
            }
            init(isWalkable);
            Vector2 currVertex = start;
            //put start node on frontier list
            G[(int)start.Y, (int)start.X] = 0;
            openList.Add(currVertex);

            while (openList.Count > 0 && hasValue(closedList, goal) != true)
            {
                //select node with lowest F cost and repeat
                currVertex = findLowestCostVertex();
                //1. find all adjacent vertices
                List<Vector2> adjList = findAdjacenyList(currVertex);
                foreach (Vector2 adjVertex in adjList)
                {
                    //only do if the vertex is not already explored
                    if (hasValue(closedList, adjVertex) != true)
                    {
                        //add each vertex to the open list
                        if (hasValue(openList, adjVertex) != true)
                        {
                            openList.Add(adjVertex);
                        }

                        //2. calculate G cost to all adjacent vertices
                        if (G[(int)adjVertex.Y, (int)adjVertex.X] >= G[(int)currVertex.Y, (int)currVertex.X] + MOVE_COST)
                        {
                            G[(int)adjVertex.Y, (int)adjVertex.X] = G[(int)currVertex.Y, (int)currVertex.X] + MOVE_COST;
                            //3. calculate H cost for all adjacent vertices
                            H[(int)adjVertex.Y, (int)adjVertex.X] = findHeuristicCost(adjVertex, goal);
                            //4. calculate F cost for all adjacent vertices
                            F[(int)adjVertex.Y, (int)adjVertex.X] = G[(int)adjVertex.Y, (int)adjVertex.X] + H[(int)adjVertex.Y, (int)adjVertex.X];
                            //set current vertex as parent
                            parentList[(int)adjVertex.Y, (int)adjVertex.X] = currVertex;
                        }
                    }
                }

                //put current vertex on explored list
                removeValue(openList, currVertex);
                closedList.Add(currVertex);
            }


            if (hasValue(closedList, goal) == true && hasValue(closedList, start) == true)
            {
                Vector2 pathVertex = goal;
                List<Vector2> path = new List<Vector2>();
                path.Add(pathVertex);
                while (pathVertex != start)
                {
                    pathVertex = parentList[(int)pathVertex.Y, (int)pathVertex.X];
                    path.Add(pathVertex);
                }
                path.Reverse();
                return path.ToArray();
            }
            return null;
        }

        /// <summary>
        /// Finds if there are walkable paths in the 4 directions around the current position.
        /// Turning round a corner is not allowed diagonally
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        private List<Vector2> findAdjacenyList(Vector2 position)
        {
            List<Vector2> adjacencyList = new List<Vector2>();
            int posX = (int)position.X;
            int posY = (int)position.Y;
            //check left
            if (posX - 1 >= 0 && posY >= 0 && isWalkable[posY, posX - 1])
            {
                adjacencyList.Add(new Vector2(posX - 1, posY));
            }
            //check up
            if (posY - 1 >= 0 && posX >= 0 && isWalkable[posY - 1, posX])
            {
                adjacencyList.Add(new Vector2(posX, posY - 1));
            }
            ////check left up
            ////is left up legal
            //if (posX - 1 > 0 && posY - 1 > 0 && isWalkable[posY - 1, posX - 1])
            //{
            //    //if up or left is blocked then this is blocked too
            //    //no corner cutting
            //    if (true)
            //    {
            //        adjacencyList.Add(new Vector2(posX - 1, posY - 1));
            //    }
            //}


            //check right
            if (posX + 1 < gridColumns && (posY >= 0 && posY < gridRows) && isWalkable[posY, posX + 1])
            {
                adjacencyList.Add(new Vector2(posX + 1, posY));
            }
            //check down
            if (posY + 1 < gridRows && (posX >= 0 && posX < gridColumns) && isWalkable[posY + 1, posX])
            {
                adjacencyList.Add(new Vector2(posX, posY + 1));
            }

            return adjacencyList;
        }

        /// <summary>
        /// Finds the heuristic(H) cost from start to goal.
        /// Uses manhatten method
        /// </summary>
        /// <param name="current"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        private int findHeuristicCost(Vector2 current, Vector2 goal)
        {
            int hCost = 0;
            hCost = 5 * (int)(Math.Abs(goal.X - current.X) + Math.Abs(goal.Y - current.Y));
            return hCost;
        }

        /// <summary>
        /// Finds the vertex with the least F cost in the open list.
        /// In case of a tie. The vertex added first wins.
        /// </summary>
        /// <returns></returns>
        private Vector2 findLowestCostVertex()
        {
            int minCost = int.MaxValue;
            Vector2 minVector = Vector2.Zero;
            if (openList.Count == 1)
                return openList[0];
            foreach (Vector2 item in openList)
            {
                if (F[(int)item.Y, (int)item.X] <= minCost)
                {
                    minCost = F[(int)item.Y, (int)item.X];
                    minVector = item;
                }
            }
            return minVector;
        }

        private bool hasValue(List<Vector2> sourceList, Vector2 value)
        {
            foreach (Vector2 item in sourceList)
            {
                if (item.X == value.X && item.Y == value.Y)
                    return true;
            }
            return false;
        }

        private void removeValue(List<Vector2> sourceList, Vector2 value)
        {
            if (hasValue(sourceList, value) == true)
            {
                int remIndex = -1;
                for (int i = 0; i < sourceList.Count; i++)
                {
                    if (sourceList[i].X == value.X && sourceList[i].Y == value.Y)
                    {
                        remIndex = i;
                        break;
                    }
                }
                if (remIndex > -1)
                {
                    sourceList.RemoveAt(remIndex);
                }
            }
        }
    }
}
