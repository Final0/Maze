using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompterRecursivite : MonoBehaviour
{
    void Compter(int i,int x)
    {
        if (i != x)
        {
            Debug.Log(i);
            i++;
            Compter(i, x);
        }
        else
            Debug.Log(x);        
    }

    void Start()
    {   
        Compter(1, 10);
    }
}
