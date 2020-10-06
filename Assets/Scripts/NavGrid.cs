using System.Collections.Generic;
using UnityEngine;

/*
public class NodePlane
{
    public Node node;
    public GameObject plane;

    public NodePlane()
    {

    }

    public NodePlane(Node node, GameObject plane)
    {
        this.node = node;
        this.plane = plane;
    }
}*/

/// <summary>
/// Навигационная сетка
/// </summary>
public class NavGrid : MonoBehaviour
{
    public const float cameraRatio = .9f;
    private const int STRAIGHT_COST = 10;
    private const int DIAGONAL_COST = 14;

    /// <summary>
    /// Размер сетки
    /// </summary>
    public Vector2Int gridSize;

    public Material walkableMaterial;
    public Material nonWalkableMaterial;
    public Material pathMaterial;
    public Material startMaterial;
    public Material endMaterial;

    public static NavGrid Singleton { get; private set; }

    public GameObject cameraGO;



    /// <summary>
    /// Префаб нода
    /// </summary>
    [SerializeField]
    private GameObject nodePrefab;

    /// <summary>
    /// Ноды сетки
    /// </summary>
    [SerializeField]
    private Node[,] gridNodes;

    /// <summary>
    /// Задать состояние нода (препятствие или нет)
    /// </summary>
    /// <param name="nodePlane"></param>
    /// <param name="isWalkable"></param>
    public void SetNodeWakable(Node nodePlane, bool isWalkable)
    {
        nodePlane.isWalkable = isWalkable;
        MeshRenderer meshRenderer = nodePlane.plane.GetComponent<MeshRenderer>();
        if (isWalkable) meshRenderer.sharedMaterial = walkableMaterial;
        else meshRenderer.sharedMaterial = nonWalkableMaterial;
    }

    /// <summary>
    /// Задать начало маршрута
    /// </summary>
    /// <param name="nodePlane"></param>
    public void SetStartNode(Node nodePlane)
    {
        MeshRenderer meshRenderer = nodePlane.plane.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = startMaterial;
    }

    /// <summary>
    /// Задать конец маршрута
    /// </summary>
    /// <param name="nodePlane"></param>
    public void SetEndNode(Node nodePlane)
    {
        MeshRenderer meshRenderer = nodePlane.plane.GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = endMaterial;
    }

    /// <summary>
    /// Получение нода по мировой позиции
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="node"></param>
    /// <returns></returns>
    public bool TryGetNodeAt(Vector3 pos, out Node node)
    {
        foreach (var _node in gridNodes)
        {
            if (_node.WorldPos == pos)
            {
                node = _node;
                return true;
            }
        }
        node = null;
        return false;
    }

    /// <summary>
    /// Отрисовка маршрута
    /// </summary>
    /// <param name="path"></param>
    public void DrawPath(List<Node> path)
    {
        for (int i = 0; i < path.Count -1; i++)
        {
            path[i].plane.GetComponent<MeshRenderer>().sharedMaterial = pathMaterial;
        }
    }

    /// <summary>
    /// Очистка сетки
    /// </summary>
    public void Clear()
    {
        Build(gridSize);
    }

    /// <summary>
    /// Получение соседей нода
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public List<Node> GetNodeNeighbours(Node node)
    {
        List<Node> nodes = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == node.position.x && y == node.position.y) continue;

                int nX = node.position.x + x;
                int nY = node.position.y + y;

                if (nX >= 0 && nX < gridSize.x && nY >= 0 && nY < gridSize.y) nodes.Add(gridNodes[nX, nY]);
            }
        }

        return nodes;
    }

    /// <summary>
    /// Создание навигационной сетки заданного размера
    /// </summary>
    /// <param name="newSize">размер сетки</param>
    public void Build(Vector2Int newSize)
    {
        ClearGrid();

        gridNodes = new Node[newSize.x, newSize.y];
        Vector3 centerOffset = new Vector3(newSize.x * 10, 0, newSize.y * 10);
        centerOffset *= .5f;

        for (int x = 0; x < newSize.x; x++)
        {
            for (int y = 0; y < newSize.y; y++)
            {
                Vector2Int pos = new Vector2Int(x * 10, y * 10);

                GameObject plane = Instantiate(nodePrefab, transform, false);

                MeshRenderer renderer = plane.GetComponent<MeshRenderer>();
                renderer.material = walkableMaterial;

                plane.transform.position = new Vector3(pos.x, 0, pos.y) - centerOffset;

                Node node = new Node(new Vector2Int(x,y), plane, true);
                gridNodes[x, y] = node;
            }
        }
        gridSize = newSize;

        // Обновить позицию камеры под новую сетку
        Bounds bounds = GetBoundsOfGrid();
        float cameraHeight = (bounds.size.x > bounds.size.z) ? bounds.size.x : bounds.size.z;
        cameraHeight *= cameraRatio;
        Vector3 cameraPos = new Vector3(bounds.center.x, cameraHeight, bounds.center.z);

        cameraGO.transform.position = cameraPos;
    }

    /// <summary>
    /// Получить расстояние от нода до нода
    /// </summary>
    /// <param name="fromNode"></param>
    /// <param name="toNode"></param>
    /// <returns></returns>
    private int GetDistance(Node fromNode, Node toNode)
    {
        int xLength = Mathf.Abs(fromNode.position.x - toNode.position.x);
        int yLength = Mathf.Abs(fromNode.position.y = toNode.position.y);
        int diff = Mathf.Abs(xLength - yLength);
        return DIAGONAL_COST * Mathf.Min(xLength, yLength) + STRAIGHT_COST * diff;
    }

    /// <summary>
    /// Очистка сетки
    /// </summary>
    private void ClearGrid()
    {
        if (gridNodes == null || gridNodes.Length == 0) return;

        foreach (var node in gridNodes)
        {
            if (node.plane)
            {
                Destroy(node.plane);
            }
        }
        gridNodes = null;
    }

    /// <summary>
    /// Возвращает Bounds сетки
    /// </summary>
    /// <returns></returns>
    private Bounds GetBoundsOfGrid()
    {
        Bounds bounds = new Bounds();
        foreach (var node in gridNodes)
        {
            bounds.Encapsulate(node.plane.GetComponent<Renderer>().bounds);
        }
        return bounds;
    }

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            Debug.LogError("Only one instance");
            Destroy(this.gameObject);
        }

        Singleton = this;
    }
}
