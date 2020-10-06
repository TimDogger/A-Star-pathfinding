using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    private const int straightCost = 10;
    private const int diagCost = 14;

    public List<Node> Path { get; private set; }

    public static Pathfinding Singleton { get; private set; }

    [SerializeField]
    private NavGrid NavGrid { get { return NavGrid.Singleton; } }

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Debug.LogError("Only one instance allowed");
            Destroy(this);
        }

        Singleton = this;
    }

    /// <summary>
    /// Поиск маршрута
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    public void FindPath(Node start, Node end)
    {
        List<Node> openNodes = new List<Node>();
        HashSet<Node> closedNodes = new HashSet<Node>();
        openNodes.Add(start);

        while (openNodes.Count > 0)
        {
            Node currentNode = openNodes[0];
            for (int i = 1; i < openNodes.Count; i++)
            {
                if (openNodes[i].fCost < currentNode.fCost || openNodes[i].fCost == currentNode.fCost && openNodes[i].hCost < currentNode.hCost) currentNode = openNodes[i];
            }

            openNodes.Remove(currentNode);
            closedNodes.Add(currentNode);

            if (currentNode == end)
            {
                RetracePath(start, end);
                NavGrid.DrawPath(Path);
                return;
            }

            foreach (var neighbour in NavGrid.GetNodeNeighbours(currentNode))
            {
                if (!neighbour.isWalkable || closedNodes.Contains(neighbour)) continue;

                int cost = currentNode.gCost + GetDistance(currentNode, neighbour);
                if (cost < neighbour.gCost || !openNodes.Contains(neighbour))
                {
                    neighbour.gCost = cost;
                    neighbour.hCost = GetDistance(neighbour, end);
                    neighbour.cameFrom = currentNode;

                    if (!openNodes.Contains(neighbour)) openNodes.Add(neighbour);
                }
            }
        }
        Debug.Log("Path not found");
    }

    /// <summary>
    /// Записать полученный маршрут
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    private void RetracePath(Node start, Node end)
    {
        Path = new List<Node>();
        Node currentNode = end;

        while (currentNode != start)
        {
            Path.Add(currentNode);
            currentNode = currentNode.cameFrom;
        }
        Path.Reverse();
    }

    /// <summary>
    /// Получение расстояние между нодами
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    private int GetDistance(Node from, Node to)
    {
        int xDistance = Mathf.Abs(from.position.x - to.position.x);
        int yDistance = Mathf.Abs(from.position.y - to.position.y);

        if (xDistance > yDistance) return diagCost * yDistance + straightCost * (xDistance - yDistance);
        else return diagCost * xDistance + straightCost * (yDistance - xDistance);
    }
}
