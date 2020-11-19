using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Node
{
    public string name;
    public List<Node> children;

    public Node(string name)
    {
        this.name = name;
        this.children = new List<Node>();
    }
}

