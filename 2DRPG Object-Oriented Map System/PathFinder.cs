using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace _2DRPG_Object_Oriented_Map_System
{
    /// <summary>
    /// This class is responsible for pathfinding in the game.
    /// </summary>
    public class Pathfinder
    {
        // Multidimensional array to hold the nodes of the map.
        public PathNode[,] nodeMap;
        // Open list to hold nodes to be evaluated.
        public List<PathNode> openList = new List<PathNode>();
        // Closed list to hold nodes that have been evaluated, using a hash set for faster look ups.
        public HashSet<PathNode> closedSet = new HashSet<PathNode>(); 
        // Path nodes to hold the starting, current and target nodes.
        public PathNode startingNode, currentNode, targetNode;

        /// <summary>
        /// This method builds a node map from a tile map.
        /// </summary>
        /// <param name="map"></param>
        /// <param name="excludedPoint">The excluded point is used for enemy's, so they can exclude themselves.</param>
        /// <returns></returns>
        public PathNode[,] BuildNodeMap(Tile[,] map, Point? excludedPoint = null)
        {
            PathNode[,] nodeMap = new PathNode[map.GetLength(0), map.GetLength(1)];
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    nodeMap[x, y] = new PathNode
                    {
                        position = new Point(x, y),
                        isWalkable = map[x, y].IsWalkable
                    };

                    // If an excluded point is provided, mark it as unwalkable.
                    if (excludedPoint.HasValue && excludedPoint.Value.X == x && excludedPoint.Value.Y == y)
                    {
                        nodeMap[x, y].isWalkable = false;
                    }
                }
            }
            return nodeMap;
        }

        /// <summary>
        /// This method finds a path from a starting point to a goal point.
        /// </summary>
        /// <param name="nodeMap"></param>
        /// <param name="startingFrom"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        public List<Point> FindPath(PathNode[,] nodeMap, Point startingFrom, Point goal)
        {
            if (nodeMap == null)
            {
                return null;
            }

            // Assign this class's node map to the provided one.
            this.nodeMap = nodeMap; 
            // Return a list of points from the pathfinding method.
            return InitializePathfinding(startingFrom, goal);
        }


        /// <summary>
        /// Initializes the pathfinding algorithm, setting the starting node, target node and open list.
        /// </summary>
        /// <param name="startingFrom"></param>
        /// <param name="goal"></param>
        /// <returns></returns>
        private List<Point> InitializePathfinding(Point startingFrom, Point goal)
        {
            // Set the starting node and target node to the provided points.
            startingNode = nodeMap[startingFrom.X, startingFrom.Y];
            targetNode = nodeMap[goal.X, goal.Y];

            // Initialize the costs.
            startingNode.GCost = 0;
            startingNode.HCost = CalculateHCost(startingNode, targetNode);
            startingNode.FCost = startingNode.GCost + startingNode.HCost;

            // The starting node has no explored from point, therefore set it to the starting point.
            startingNode.exploredFrom = startingFrom;

            // Clear the lists before starting the algorithm, to avoid any previous data.
            openList.Clear();
            closedSet.Clear();

            // Add the starting node to the open list.
            openList.Add(startingNode);

            while (openList.Count > 0)
            {
                // Get the node with the lowest F cost.
                currentNode = GetNodeWithLowestFCost(openList);

                if (currentNode.position == targetNode.position)
                {
                    // Return the path because the target node has been reached.
                    return ReconstructPath(currentNode);
                }

                // Switch the current node from the open list to the closed list.

                openList.Remove(currentNode);
                closedSet.Add(currentNode);

                // Check the neighbours of the current node.
                foreach (var neighbor in GetNeighbors(currentNode))
                {
                    // If the neighbor is not walkable or in closed set, skip it.
                    if (!neighbor.isWalkable || closedSet.Contains(neighbor)) continue;

                    // Calculate the tentative G cost.
                    int tentativeGCost = currentNode.GCost + 1;

                    // Add the neighbor to the list if it's cost is less than the current cost.
                    if (tentativeGCost < neighbor.GCost || !openList.Contains(neighbor))
                    {
                        neighbor.GCost = tentativeGCost;
                        neighbor.HCost = CalculateHCost(neighbor, targetNode);
                        neighbor.FCost = neighbor.GCost + neighbor.HCost;
                        neighbor.exploredFrom = currentNode.position;

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }
            // If no path is found, return null.
            return null;
        }

        /// <summary>
        /// This method calculate's the H cost of current node based on the target node.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="targetNode"></param>
        /// <returns></returns>
        public int CalculateHCost(PathNode node, PathNode targetNode)
        {
            return Math.Abs(node.position.X - targetNode.position.X) + Math.Abs(node.position.Y - targetNode.position.Y);
        }

        /// <summary>
        /// This method calculate's the F cost of the current node.
        /// </summary>
        /// <param name="openList"></param>
        /// <returns></returns>
        public PathNode GetNodeWithLowestFCost(List<PathNode> openList)
        {
            if (openList == null || openList.Count == 0)
            {
                return null;
            }
            PathNode lowestFCostNode = openList[0];
            foreach (PathNode node in openList)
            {
                if (node.FCost < lowestFCostNode.FCost)
                {
                    lowestFCostNode = node;
                }
            }
            return lowestFCostNode;
        }

        /// <summary>
        /// This method returns the neighbors of a node, based on orthogonal directions.
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public List<PathNode> GetNeighbors(PathNode node)
        {
            List<PathNode> neighbors = new List<PathNode>();
            Point[] directions = new Point[]
            {
                new Point(0, 1), new Point(1, 0), new Point(0, -1), new Point(-1, 0)
            };

            foreach (var direction in directions)
            {
                Point neighborPos = new Point(node.position.X + direction.X, node.position.Y + direction.Y);
                if (neighborPos.X >= 0 && neighborPos.X < nodeMap.GetLength(0) &&
                    neighborPos.Y >= 0 && neighborPos.Y < nodeMap.GetLength(1))
                {
                    PathNode neighbor = nodeMap[neighborPos.X, neighborPos.Y];
                    neighbors.Add(neighbor);
                }
            }
            return neighbors;
        }

        /// <summary>
        /// This method reconstructs the path from the end node to the start node. Used after a path is found to reconstruct the path.
        /// </summary>
        /// <param name="endNode"></param>
        /// <returns></returns>
        public List<Point> ReconstructPath(PathNode endNode)
        {
            List<Point> path = new List<Point>();
            PathNode currentNode = endNode;

            while (currentNode != null && currentNode.position != startingNode.position)
            {
                path.Add(currentNode.position);
                if (currentNode.exploredFrom.X == -1)
                {
                    break;
                }
                currentNode = nodeMap[currentNode.exploredFrom.X, currentNode.exploredFrom.Y];
            }
            path.Reverse();
            return path;
        }
    }

    /// <summary>
    /// The Path Node class is used to represent a node in the pathfinding algorithm.
    /// </summary>
    public class PathNode
    {
        public int HCost, FCost, GCost;
        public Point position, exploredFrom;
        public bool isWalkable;

        public PathNode()
        {
            Reset();
        }

        /// <summary>
        /// This method reset's the node's cost and explored from point.
        /// </summary>
        public void Reset()
        {
            HCost = FCost = GCost = 1000000;
            exploredFrom = new Point(-1, -1);
            isWalkable = false;
        }
    }
}