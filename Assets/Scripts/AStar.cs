using System;
using System.Collections.Generic;

public class AStar
{
    public interface Level
    {
        bool IsFree((int x, int y) pos);

        double Cost((int x, int y) from, (int x, int y) to);

        double Heuristic((int x, int y) from, (int x, int y) to);
    }

    public static List<(int, int)> Apply(Level level, (int, int) startPos, (int, int) finalPos, bool allowDiagonalMove)
    {
        Dictionary<(int, int), Node> openList = new Dictionary<(int, int), Node>();
        openList[startPos] = new Node(startPos, level.Heuristic(startPos, finalPos), true);
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
            UpdateNeighbours(currentNode, level, openList, closeList, finalPos, allowDiagonalMove);
        }
        // 4 - Build shortest path
        List<(int, int)> path = BuildShortestPath(closeList, startPos, finalPos);

        return path;
    }

    private static Node GetCheapestNodeNotTreaded(Dictionary<(int, int), Node> openList)
    {
        Node ret = null;
        double currentMinDistance = double.MaxValue;

        foreach(Node oneNode in openList.Values)
        {
            if (oneNode.totalCost < currentMinDistance)
            {
                currentMinDistance = oneNode.totalCost;
                ret = oneNode;
            }
        }

        return ret;
    }

    private static void UpdateNeighbours(Node currentNode, Level level, Dictionary<(int, int), Node> openList, Dictionary<(int, int), Node> closeList, (int, int) finalPos, bool allowDiagonalMove)
    {
        List<Node> neighbours = GetNeighbours(currentNode, level, openList, closeList, finalPos, allowDiagonalMove);
        foreach (Node oneNeighbour in neighbours)
        {
            oneNeighbour.UpdateNeighbourIfNeeds(currentNode, level.Cost(currentNode.Pos, oneNeighbour.Pos));
        }
    }

    private static List<Node> GetNeighbours(Node currentNode, Level level, Dictionary<(int, int), Node> openList, Dictionary<(int, int), Node> closeList, (int, int) finalPos, bool allowDiagonalMove)
    {
        List<Node> neighbours = new List<Node>();
        (int x, int y) leftNodeCoordinates = (currentNode.x - 1, currentNode.y);
        (int x, int y) rightNodeCoordinates = (currentNode.x + 1, currentNode.y);
        (int x, int y) topNodeCoordinates = (currentNode.x, currentNode.y - 1);
        (int x, int y) bottomNodeCoordinates = (currentNode.x, currentNode.y + 1);

        (int x, int y) bottomLeftNodeCoordinates = (currentNode.x - 1, currentNode.y + 1);
        (int x, int y) bottomRightNodeCoordinates = (currentNode.x + 1, currentNode.y + 1);
        (int x, int y) topLeftNodeCoordinates = (currentNode.x - 1, currentNode.y - 1);
        (int x, int y) topRightNodeCoordinates = (currentNode.x + 1, currentNode.y - 1);

        if (!closeList.ContainsKey(leftNodeCoordinates) && level.IsFree(leftNodeCoordinates))
        {
            if (!openList.ContainsKey(leftNodeCoordinates))
                openList[leftNodeCoordinates] = new Node(leftNodeCoordinates, level.Heuristic(leftNodeCoordinates , finalPos));

            neighbours.Add(openList[leftNodeCoordinates]);
        }
        if (!closeList.ContainsKey(rightNodeCoordinates) && level.IsFree(rightNodeCoordinates))
        {
            if (!openList.ContainsKey(rightNodeCoordinates))
                openList[rightNodeCoordinates] = new Node(rightNodeCoordinates, level.Heuristic(rightNodeCoordinates, finalPos));

            neighbours.Add(openList[rightNodeCoordinates]);
        }
        if (!closeList.ContainsKey(topNodeCoordinates) && level.IsFree(topNodeCoordinates))
        {
            if (!openList.ContainsKey(topNodeCoordinates))
                openList[topNodeCoordinates] = new Node(topNodeCoordinates, level.Heuristic(topNodeCoordinates, finalPos));

            neighbours.Add(openList[topNodeCoordinates]);
        }
        if (!closeList.ContainsKey(bottomNodeCoordinates) && level.IsFree(bottomNodeCoordinates))
        {
            if (!openList.ContainsKey(bottomNodeCoordinates))
                openList[bottomNodeCoordinates] = new Node(bottomNodeCoordinates, level.Heuristic(bottomNodeCoordinates, finalPos));

            neighbours.Add(openList[bottomNodeCoordinates]);
        }
        if (allowDiagonalMove)
        {
            if ((!closeList.ContainsKey(rightNodeCoordinates) && level.IsFree(rightNodeCoordinates)) && (!closeList.ContainsKey(bottomNodeCoordinates) && level.IsFree(bottomNodeCoordinates)))
            {
                if (!openList.ContainsKey(bottomRightNodeCoordinates))
                    openList[bottomRightNodeCoordinates] = new Node(bottomRightNodeCoordinates, level.Heuristic(bottomRightNodeCoordinates, finalPos));

                neighbours.Add(openList[bottomRightNodeCoordinates]);
            }
            if ((!closeList.ContainsKey(leftNodeCoordinates) && level.IsFree(leftNodeCoordinates)) && (!closeList.ContainsKey(bottomNodeCoordinates) && level.IsFree(bottomNodeCoordinates)))
            {
                if (!openList.ContainsKey(bottomLeftNodeCoordinates))
                    openList[bottomLeftNodeCoordinates] = new Node(bottomLeftNodeCoordinates, level.Heuristic(bottomLeftNodeCoordinates, finalPos));

                neighbours.Add(openList[bottomLeftNodeCoordinates]);
            }
            if ((!closeList.ContainsKey(rightNodeCoordinates) && level.IsFree(rightNodeCoordinates)) && (!closeList.ContainsKey(topNodeCoordinates) && level.IsFree(topNodeCoordinates)))
            {
                if (!openList.ContainsKey(topRightNodeCoordinates))
                    openList[topRightNodeCoordinates] = new Node(topRightNodeCoordinates, level.Heuristic(topRightNodeCoordinates, finalPos));

                neighbours.Add(openList[topRightNodeCoordinates]);
            }
            if ((!closeList.ContainsKey(leftNodeCoordinates) && level.IsFree(leftNodeCoordinates)) && (!closeList.ContainsKey(topNodeCoordinates) && level.IsFree(topNodeCoordinates)))
            {
                if (!openList.ContainsKey(topLeftNodeCoordinates))
                    openList[topLeftNodeCoordinates] = new Node(topLeftNodeCoordinates, level.Heuristic(topLeftNodeCoordinates, finalPos));

                neighbours.Add(openList[topLeftNodeCoordinates]);
            }
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

        public Node((int, int) coordinates, double heuristic, bool isStart = false)
        {
            this.coordinates = coordinates;
            this.h = heuristic;
            this.g = isStart ? 0 : double.MaxValue;
            this.previousNode = null;
        }

        public (int, int) Pos
        {
           get { return coordinates; } 
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

        public double g { get; private set; }

        public double h { get; private set; }

        public double totalCost { get { return g + h; } }

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


        public void UpdateNeighbourIfNeeds(Node fromNode, double cost)
        {
            // distance manhattan
            double newDistanceFromStart = fromNode.g + cost;

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
