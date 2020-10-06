using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int gCost;

    public int hCost;

    public int fCost { get { return hCost + gCost; } }

    public Node cameFrom;

    /// <summary>
    /// Представляет нод в сцене
    /// </summary>
    public GameObject plane;

    public bool isWalkable;

    /// <summary>
    /// Позиция в сетке (двумерном массиве)
    /// </summary>
    public Vector2Int position;

    /// <summary>
    /// Позиция в сцене
    /// </summary>
    public Vector3 WorldPos { get { return plane.transform.position; } }

    public Node() { }

    public Node(Vector2Int position, GameObject plane, bool isWalkable)
    {
        this.position = position;
        this.plane = plane;
        this.isWalkable = isWalkable;
    }

}
