using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Node 
{
    public Vector2 position;
    public int distance = -1;
    public List<Node> nearNodes;
    public Node prev;
    public bool visited = false;

    public void Reset() 
    {
        distance = -1;
        visited = false;
        prev = null;
    }
}
