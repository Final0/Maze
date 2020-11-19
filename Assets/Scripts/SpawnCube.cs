using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCube : MonoBehaviour
{
    public GameObject cube;
    public int nbCube;
    Vector3 pos;

    Vector3 Direction(Vector3 pos)
    {
        int axis = Random.Range(1, 4);
        int value = Random.Range(0, 2);

        if (value == 0)
            value = -1;

        if (axis == 1)
            pos.x += value;
        else if (axis == 2)
            pos.y += value;
        else if (axis == 3)
            pos.z += value;

        return pos;
    }

    void Spawner(int i, int x)
    {
        if (i <= x)
        {
            if (i != 1)
                pos = Direction(pos);

            if (!GameObject.Find("obj_" + pos.x + "_" + pos.y + "_" + pos.z))
            {
                GameObject go = Instantiate(cube, pos, Quaternion.identity);

                go.name = "obj_" + pos.x + "_" + pos.y + "_" + pos.z;

                Spawner(++i, x);
            }   
            else
                Spawner(i, x);  
        }
    }

    void Start()
    {
        pos = Vector3.zero;

        Spawner(1, nbCube);
    }
}
