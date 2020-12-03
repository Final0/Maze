using System;
using System.Collections.Generic;

public class AStar
{
    public static List<(int, int)> Apply(int[,] maze, (int, int) startPos, (int, int) finalPos)
    {
        Dictionary<(int, int), Node> openList = new Dictionary<(int, int), Node>();
        openList[startPos] = new Node(startPos, finalPos, true);
        Dictionary<(int, int), Node> closeList = new Dictionary<(int, int), Node>();
        Node finalNode = null;

        while (true)
        {
            // 1 - Choose the cheapest node not treated
            Node currentNode = GetCheapestNodeNotTreaded(openList);

            // No path between start and end
            if (currentNode == null)
            {
                break;
            }

            // Move currentNode From openList To closeList
            openList.Remove((currentNode.x, currentNode.y));
            closeList[(currentNode.x, currentNode.y)] = currentNode;

            // We found the finalPos
            if (currentNode.isSamePos(finalPos))
            {
                finalNode = currentNode;
                break;
            }

            // 2 - Update the neighbours of the current node
            UpdateNeighbours(currentNode, maze, openList, closeList, finalPos);
        }
        // 4 - Build shortest path
        List<(int, int)> path = BuildShortestPath(closeList, startPos, finalPos);

        return path;
    }

    private static Node GetCheapestNodeNotTreaded(Dictionary<(int, int), Node> openList)
    {
        Node ret = null;
        int currentMinDistance = int.MaxValue;

        foreach(Node oneNode in openList.Values)
        {
            if (oneNode.totalDistance < currentMinDistance)
            {
                currentMinDistance = oneNode.totalDistance;
                ret = oneNode;
            }
        }

        return ret;
    }

    private static void UpdateNeighbours(Node currentNode, int[,] maze, Dictionary<(int, int), Node> openList, Dictionary<(int, int), Node> closeList, (int, int) finalPos)
    {
        List<Node> neighbours = GetNeighbours(currentNode, maze, openList, closeList, finalPos);
        foreach (Node oneNeighbour in neighbours)
        {
            oneNeighbour.UpdateNeighbourIfNeeds(currentNode);
        }
    }

    private static List<Node> GetNeighbours(Node currentNode, int[,] maze, Dictionary<(int, int), Node> openList, Dictionary<(int, int), Node> closeList, (int, int) finalPos)
    {
        List<Node> neighbours = new List<Node>();
        (int x, int y) leftNodeCoordinates = (currentNode.x - 1, currentNode.y);
        (int x, int y) rightNodeCoordinates = (currentNode.x + 1, currentNode.y);
        (int x, int y) topNodeCoordinates = (currentNode.x, currentNode.y - 1);
        (int x, int y) bottomNodeCoordinates = (currentNode.x, currentNode.y + 1);
        if (!closeList.ContainsKey(leftNodeCoordinates) && MazeGenerator.IsFree(maze, leftNodeCoordinates))
        {
            if (!openList.ContainsKey(leftNodeCoordinates))
                openList[leftNodeCoordinates] = new Node(leftNodeCoordinates, finalPos);

            neighbours.Add(openList[leftNodeCoordinates]);
        }
        if (!closeList.ContainsKey(rightNodeCoordinates) && MazeGenerator.IsFree(maze, rightNodeCoordinates))
        {
            if (!openList.ContainsKey(rightNodeCoordinates))
                openList[rightNodeCoordinates] = new Node(rightNodeCoordinates, finalPos);

            neighbours.Add(openList[rightNodeCoordinates]);
        }
        if (!closeList.ContainsKey(topNodeCoordinates) && MazeGenerator.IsFree(maze, topNodeCoordinates))
        {
            if (!openList.ContainsKey(topNodeCoordinates))
                openList[topNodeCoordinates] = new Node(topNodeCoordinates, finalPos);

            neighbours.Add(openList[topNodeCoordinates]);
        }
        if (!closeList.ContainsKey(bottomNodeCoordinates) && MazeGenerator.IsFree(maze, bottomNodeCoordinates))
        {
            if (!openList.ContainsKey(bottomNodeCoordinates))
                openList[bottomNodeCoordinates] = new Node(bottomNodeCoordinates, finalPos);

            neighbours.Add(openList[bottomNodeCoordinates]);
        }
        return neighbours;
    }

    private static List<(int, int)> BuildShortestPath(Dictionary<(int, int), Node> nodesTab, (int, int) startPos, (int, int) finalPos)
    {
        List<(int, int)> path = new List<(int, int)>();
        // if not path between start & final
        if (!nodesTab[finalPos].hasPrevious())
        {
            return path;
        }
        Node currentNode = nodesTab[finalPos];
        path.Add((currentNode.x, currentNode.y));

        while (currentNode.hasPrevious())
        {
            path.Insert(0, currentNode.previousPos);
            currentNode = nodesTab[currentNode.previousPos];
        }
        
        return path;
    }

    private class Node
    {
        private (int, int) coordinates;
        private Node previousNode;

        public Node((int, int) coordinates, (int x, int y) finalPos, bool isStart = false)
        {
            this.coordinates = coordinates;
            this.h = Math.Abs(x - finalPos.x) + Math.Abs(y - finalPos.y);
            this.g = isStart ? 0 : int.MaxValue;
            this.previousNode = null;
        }


        public int x
        {
            get
            {
                return coordinates.Item1;
            }
        }

        public int y
        {
            get
            {
                return coordinates.Item2;
            }
        }

        public int g { get; private set; }

        public int h { get; private set; }

        public int totalDistance { get { return g + h; } }

        public bool hasPrevious()
        {
            return null != previousNode;
        }

        public (int, int) previousPos
        {
            get
            {
                return previousNode.coordinates;
            }
        }

        public bool isSamePos((int, int) pos)
        {
            return coordinates == pos;
        }


        public void UpdateNeighbourIfNeeds(Node fromNode)
        {
            // distance manhattan
            int distanceBetweenNodes = Math.Abs(x - fromNode.x) + Math.Abs(y - fromNode.y);
            int newDistanceFromStart = fromNode.g + distanceBetweenNodes;
            if (newDistanceFromStart < g)
            {
                g = newDistanceFromStart;
                previousNode = fromNode;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is Node)
            {
                return isSamePos(((Node)obj).coordinates);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return coordinates.GetHashCode();
        }
    }
}
