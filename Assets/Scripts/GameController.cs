using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour, AStar.Level
{
    [SerializeField]
    private MazeGenerator mazeGenerator;

    [SerializeField]
    private GameObject wallPrefab;

    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    private GameObject treasureGameObject;

    [SerializeField]
    private GameObject bonusGameObject;

    private int[,] maze;

    private PlayerController playerObj;

    public bool allowDiagonalMove = true;

    private void Start()
    {
        maze = mazeGenerator.CreateMaze();
        MazeIn3D(maze, wallPrefab);

        (int, int) randomPos = SpawnPlayerAtPos(playerController);
        playerObj = CreatePlayer(randomPos, playerController);

        SpawningCollectibles();
    }

    #region Spawning Collectibles (Treasures & Bonus)
    private const int NUMBER_TREASURE = 6;
    private const int NUMBER_BONUS = 8;

    private void SpawningCollectibles()
    {
        for (int i = 0; i < NUMBER_TREASURE; i++)
        {
            (int, int) randomTreasurePos = randomCollectible(treasureGameObject);
            SpawnCollectible(randomTreasurePos, treasureGameObject);
        }

        for (int i = 0; i < NUMBER_BONUS; i++)
        {
            (int, int) randomBonusPos = randomCollectible(bonusGameObject);
            SpawnCollectible(randomBonusPos, bonusGameObject);
        }
    }

    private GameObject SpawnCollectible((int, int) randomPos, GameObject collectible)
    {
        return Instantiate(collectible, FromMazeTo3D(randomPos), Quaternion.identity);
    }

    private (int, int) randomCollectible(GameObject collectible)
    {
        (int, int) randomPos;
        do
        {
            int randomX = UnityEngine.Random.Range(0, maze.GetLength(0));
            int randomY = UnityEngine.Random.Range(0, maze.GetLength(1));
            randomPos = (randomX, randomY);
        } while (!MazeGenerator.IsFree(maze, randomPos));

        return randomPos;
    }
    #endregion

    private PlayerController CreatePlayer((int, int) randomPos, PlayerController playerController)
    {
        return GameObject.Instantiate(playerController, FromMazeTo3D(randomPos), Quaternion.identity);
    }   

    private (int, int) SpawnPlayerAtPos(PlayerController playerController)
    {
        (int, int) randomPos;
        do
        {
            int randomX = UnityEngine.Random.Range(0, maze.GetLength(0));
            int randomY = UnityEngine.Random.Range(0, maze.GetLength(1));
            randomPos = (randomX, randomY);
        } while (!MazeGenerator.IsFree(maze, randomPos));

        return randomPos;
    }
    
    public void MovePlayer(Vector3 final3DPos)
    {
        //Debug.Log("Moving to " + final3DPos);
        // 1 - Convert 3D coordinates to Maze coordinates => finalMazePos
        (int, int) finalMazePos = From3DMaze(final3DPos);
        // 2 - Apply Dijkstra to find the path between playerPos and finalMazePos
        List<(int, int)> shortestPath = AStar.Apply(this, From3DMaze(playerObj.transform.position), finalMazePos, allowDiagonalMove);
        // 3 - Ask the player to follow this path
        playerObj.Move(FromMazeTo3D(shortestPath));
    }

    public void MazeIn3D(int[,] maze, GameObject prefab)
    {
        for (int y = 0; y < maze.GetLength(1); y++)
        {
            for (int x = 0; x < maze.GetLength(0); x++)
            {
                if (maze[x, y] == MazeGenerator.CELL_TYPE_WALL)
                {
                    Vector3 position = FromMazeTo3D((x, y));
                    GameObject.Instantiate(prefab, position, Quaternion.identity);
                }
            }
        }
    }

    private static Vector3 FromMazeTo3D((int x, int y) p)
    {
        return new Vector3(p.x + 0.5f, 0.5f, -p.y - 0.5f);
    }

    private static (int, int) From3DMaze(Vector3 coordinates3d)
    {
        int x = (int)coordinates3d.x;
        int y = -(int)coordinates3d.z;
        return (x, y);
    }

    private static List<Vector3> FromMazeTo3D(List<(int, int)> path)
    {
        List<Vector3> ret = new List<Vector3>();
        foreach (var onePoint in path)
        {
            ret.Add(FromMazeTo3D(onePoint));
        }
        return ret;
    }

    public bool IsFree((int x, int y) pos)
    {
        return MazeGenerator.IsFree(maze, pos);
    }

    public double Cost((int x, int y) from, (int x, int y) to)
    {
        int dx = to.x - from.x;
        int dy = to.y - from.y;
        return Math.Sqrt(dx * dx + dy * dy);
    }

    public double Heuristic((int x, int y) from, (int x, int y) to)
    {
        int dx = to.x - from.x;
        int dy = to.y - from.y;
        return Math.Sqrt(dx * dx + dy * dy) / 2.0d;
    }
}