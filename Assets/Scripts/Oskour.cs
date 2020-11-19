using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Oskour : MonoBehaviour
{
    private const int CELL_TYPE_WALL = 0;
    private const int CELL_TYPE_TO_EXPLORE = 1;
    private const int CELL_TYPE_EXPLORED = 2;
    public const int CELL_TYPE_ROOM = 3;

    [SerializeField]
    private int gridWidth = 10;
    [SerializeField]
    private int gridHeight = 5;

    [Header("Rooms")]

    public GameObject wallPrefab;

    [SerializeField]
    private int nbRooms = 2;

    [SerializeField]
    private int minRoomX = 2;

    [SerializeField]
    private int maxRoomX = 3;

    [SerializeField]
    private int minRoomY = 2;

    [SerializeField]
    private int maxRoomY = 3;

    // Start is called before the first frame update
    void Start()
    {
        int[,] maze = InitMaze(gridWidth, gridHeight);
        (int, int) randomCell = GetRandomStartingPoint(gridWidth, gridHeight);
        CreateMaze(maze, randomCell);
        MakeRoom(maze);
        DisplayMazeInConsole(maze);
        MazeIn3D(maze, wallPrefab);
    }

    private void MazeIn3D(int[,] maze, GameObject prefab)
    {
        for (int y = 0; y < maze.GetLength(1); y++)
        {
            for (int x = 0; x < maze.GetLength(0); x++)
            {
                if (maze[x, y] == CELL_TYPE_WALL)
                {
                    Vector3 position = FromMazeTo3D((x, y));
                    GameObject.Instantiate(prefab, position, Quaternion.identity);
                }
            }
        }
    }

    private Vector3 FromMazeTo3D((int x, int y) p)
    {
        return new Vector3(p.x + 0.5f, 0.5f, -p.y - 0.5f);
    }

    private void MakeRoom(int[,] maze)
    {
        for (int i = 0; i < nbRooms; i++)
        {
            (int, int) randomRoomSize = GetRandomRoomSize(minRoomX, maxRoomX, minRoomY, maxRoomY);
            (int, int) randomRoomCell = GetRandomRoomPoint(gridWidth, gridWidth);

            if (randomRoomCell.Item1 >= maze.GetLength(0) || randomRoomCell.Item2 >= maze.GetLength(1))
            {
                i--;
                continue;
            }

            for (int y = 0; y < randomRoomSize.Item2; y++)
            {
                for (int x = 0; x < randomRoomSize.Item1; x++)
                {
                    int randomX = randomRoomCell.Item1 + x;
                    int randomY = randomRoomCell.Item2 + y;

                    if (randomX > 0 && randomY > 0 && randomX < (maze.GetLength(0) - 1) && randomY < (maze.GetLength(1) - 1))
                        maze[randomX, randomY] = CELL_TYPE_ROOM;
                }
            }
        }
    }

    private (int, int) GetRandomRoomSize(int minRoomX, int maxRoomX, int minRoomY, int maxRoomY)
    {
        int randomXRoom = UnityEngine.Random.Range(minRoomX, maxRoomX + 1);
        int randomYRoom = UnityEngine.Random.Range(minRoomY, maxRoomY + 1);
        return (randomXRoom, randomYRoom);
    }

    private (int, int) GetRandomRoomPoint(int gridWidth, int gridHeiht)
    {
        int randomXInMaze = 1 + UnityEngine.Random.Range(0, gridWidth) * 2;
        int randomYInMaze = 1 + UnityEngine.Random.Range(0, gridHeiht) * 2;
        return (randomXInMaze, randomYInMaze);
    }

    private void CreateMaze(int[,] maze, (int, int) cellToExplore)
    {
        // 3.0) La marquer comme explorée(= lui mettre la valeur 2)
        maze[cellToExplore.Item1, cellToExplore.Item2] = CELL_TYPE_EXPLORED;

        // 3.1) Tant qu’il y une cellule non visitée adjacente(Sinon c’est un cul de sac)
        // 3.1.1) Choisir une direction au hasard vers une cellule non visitée => childCell
        
        while (true)
        {
            List<(int, int)> cellsToExplore = GetCellsToExplore(maze, cellToExplore);
            if (cellsToExplore.Count == 0)
                break;

            int random = UnityEngine.Random.Range(0, cellsToExplore.Count);
            (int, int) randomCell = cellsToExplore[random];
            BreakWall(maze, cellToExplore, randomCell);
            CreateMaze(maze, randomCell);
        }

        // 3.1.2) Casser le mur vers la nouvelle cellule (c a d mettre le mur à CELL_TYPE_EXPLORED)
        // 3.1.3) Explorer depuis la nouvelle cellule      Explore(maze, childCell) 
    }

    private void BreakWall(int[,] maze, (int, int) cellToExplore, (int, int) randomCell)
    {
        int x = (cellToExplore.Item1 + randomCell.Item1) / 2;
        int y = (cellToExplore.Item2 + randomCell.Item2) / 2;

        maze[x, y] = 2;
    }

    private List<(int, int)> GetCellsToExplore(int[,] maze, (int, int) cell)
    {
        List<(int, int)> cells = new List<(int, int)>();
        int leftX = cell.Item1 - 2;
        int rightX = cell.Item1 + 2;
        int topY = cell.Item2 - 2;
        int bottomY = cell.Item2 + 2;

        // left cell
        if (leftX >= 0 && maze[leftX, cell.Item2] == 1)
        {
            cells.Add((leftX, cell.Item2));
        }

        // right cell
        if (rightX < maze.GetLength(0) && maze[rightX, cell.Item2] == 1)
        {
            cells.Add((rightX, cell.Item2));
        }

        // top cell
        if (topY >= 0 && maze[cell.Item1, topY] == 1)
        {
            cells.Add((cell.Item1, topY));
        }

        // bottom cell
        if (bottomY < maze.GetLength(1) && maze[cell.Item1, bottomY] == 1)
        {
            cells.Add((cell.Item1, bottomY));
        }

        return cells;
    }

    private (int, int) GetRandomStartingPoint(int gridWidth, int gridHeiht)
    {
        int randomXInMaze = 1 + UnityEngine.Random.Range(0, gridWidth) * 2;
        int randomYInMaze = 1 + UnityEngine.Random.Range(0, gridHeiht) * 2;
        return (randomXInMaze, randomYInMaze);
    }

    private int[,] InitMaze(int gridWidth, int gridHeiht)
    {
        int mazeWidth = gridWidth * 2 + 1;
        int mazeHeight = gridHeiht * 2 + 1;
        int[,] maze = new int[mazeWidth, mazeHeight];
        for (int y = 0; y < mazeHeight; y++)
        {
            for (int x = 0; x < mazeWidth; x++)
            {
                maze[x, y] = x % 2 == 0 || y % 2 == 0 ? CELL_TYPE_WALL : CELL_TYPE_TO_EXPLORE;
            }
        }
        return maze;
    }

    private void DisplayMazeInConsole(int[,] maze)
    {
        string strVersion = "";
        for (int y = 0; y < maze.GetLength(1); y++)
        {
            for (int x = 0; x < maze.GetLength(0); x++)
            {
                int cellType = maze[x, y];
                switch (cellType)
                {
                    case CELL_TYPE_WALL:
                        strVersion += "█";
                        break;
                    case CELL_TYPE_TO_EXPLORE:
                        strVersion += "▓";
                        break;
                    case CELL_TYPE_EXPLORED:
                        strVersion += "░";
                        break;
                    case CELL_TYPE_ROOM:
                        strVersion += "▒";
                        break;
                }
            }
            strVersion += "\n";
        }
        Debug.Log(strVersion);
    }
}