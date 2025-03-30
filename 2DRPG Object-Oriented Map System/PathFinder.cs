using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace _2DRPG_Object_Oriented_Map_System
{
    public class Pathfinder
    {
        public PathNode[,] nodeMap;
        public List<PathNode> openList = new List<PathNode>();
        public HashSet<PathNode> closedSet = new HashSet<PathNode>(); // Use HashSet for closed list
        public PathNode startingNode, currentNode, targetNode;

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

        public List<Point> FindPath(PathNode[,] nodeMap, Point startingFrom, Point goal)
        {
            if (nodeMap == null)
            {
                return null;
            }

            this.nodeMap = nodeMap; // Set the node map in the pathfinder
            return InitializePathfinding(startingFrom, goal);
        }

        public List<Point> ReversePath(List<Point> path)
        {
            List<Point> reversedPath = new List<Point>();
            for (int i = path.Count - 1; i >= 0; i--)
            {
                reversedPath.Add(path[i]);
            }
            return reversedPath;
        }

        private List<Point> InitializePathfinding(Point startingFrom, Point goal)
        {
            startingNode = nodeMap[startingFrom.X, startingFrom.Y];
            targetNode = nodeMap[goal.X, goal.Y];

            startingNode.GCost = 0;
            startingNode.HCost = CalculateHCost(startingNode, targetNode);
            startingNode.FCost = startingNode.GCost + startingNode.HCost;
            startingNode.exploredFrom = startingFrom;

            openList.Clear();
            closedSet.Clear();
            openList.Add(startingNode);

            while (openList.Count > 0)
            {
                currentNode = GetNodeWithLowestFCost(openList);
                if (currentNode.position == targetNode.position)
                {
                    return ReconstructPath(currentNode);
                }

                openList.Remove(currentNode);
                closedSet.Add(currentNode);

                foreach (var neighbor in GetNeighbors(currentNode))
                {
                    if (!neighbor.isWalkable || closedSet.Contains(neighbor)) continue;

                    int tentativeGCost = currentNode.GCost + 1;
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
            return null; // No path found
        }

        public int CalculateHCost(PathNode node, PathNode targetNode)
        {
            return Math.Abs(node.position.X - targetNode.position.X) + Math.Abs(node.position.Y - targetNode.position.Y);
        }

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

            //path.Add(startingNode.position);
            path.Reverse();
            return path;
        }
    }

    public class PathNode
    {
        public int HCost, FCost, GCost;
        public Point position, exploredFrom;
        public bool isWalkable;

        public PathNode()
        {
            Reset();
        }

        public void Reset()
        {
            HCost = FCost = GCost = 1000000;
            exploredFrom = new Point(-1, -1);
            isWalkable = false;
        }
    }
}