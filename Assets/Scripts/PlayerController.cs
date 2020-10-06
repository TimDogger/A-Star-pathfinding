using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private EventSystem eventSystem;

    [SerializeField]
    private Camera camera;

    private Node startNode;

    private Node endNode;

    private bool isPathReady = false;
    private bool isSettingWalkable = false;

    public void Awake()
    {
        if (!eventSystem)
        {
            eventSystem = FindObjectOfType<EventSystem>();
            if (!eventSystem) Debug.LogError("EventSystem not found in scene");
        }

        if (!camera)
        {
            camera = Camera.main;
            if (!camera) Debug.LogError("No cameras in scene");
        }
    }

    /// <summary>
    /// При ЛКМ переключить состояние нода (препятствие или нет)
    /// </summary>
    private void OnLeftClick()
    {
        if (eventSystem.IsPointerOverGameObject()) return;

        if (TryGetNodePlaneUnderCursor(out Node node) && node.isWalkable != isSettingWalkable)
        {
            if (node.isWalkable) isSettingWalkable = false;
            else isSettingWalkable = true;
            NavGrid.Singleton.SetNodeWakable(node, isSettingWalkable);
        }
    }

    /// <summary>
    /// При ПКМ установи
    /// </summary>
    private void OnRightClick()
    {
        Debug.Log("1");
        if (eventSystem.IsPointerOverGameObject()) return;

        if (TryGetNodePlaneUnderCursor(out Node node))
        {
            if (isPathReady)
            {
                ClearGrid();
                return;
            }

            // Задаем начало
            if (startNode == null)
            {
                SetStart(node);
            }
            // задаем конец
            else if (endNode == null)
            {
                SetEnd(node);
                return;
            }
        }
    }

    /// <summary>
    /// Задать конец маршрута
    /// </summary>
    /// <param name="node"></param>
    private void SetEnd(Node node)
    {
        if (startNode == node)
        {
            Debug.Log("No path nedeed. Start and end are equal");
            ClearGrid();
            return;
        }

        NavGrid.Singleton.SetEndNode(node);
        endNode = node;
        Pathfinding.Singleton.FindPath(startNode, endNode);
        isPathReady = true;
    }

    /// <summary>
    /// Задать начало маршрута
    /// </summary>
    /// <param name="node"></param>
    private void SetStart(Node node)
    {
        NavGrid.Singleton.SetStartNode(node);
        startNode = node;
    }

    public void ClearGrid()
    {
        startNode = null;
        endNode = null;
        isPathReady = false;
        NavGrid.Singleton.Clear();
    }

    /// <summary>
    /// Возвращает нод под курсором
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    private bool TryGetNodePlaneUnderCursor(out Node node)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (NavGrid.Singleton.TryGetNodeAt(hit.transform.position, out node))
            {
                return true;
            }
        }
        node = null;
        return false;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
            OnLeftClick();
        if (Input.GetMouseButtonDown(1)) 
            OnRightClick();
    }
}
