using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


public class MouseMovementController : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap collisionTilemap;
    [SerializeField] private Tilemap finishTilemap;
    [SerializeField] private TileBase finishTile;
    [SerializeField] private Grid tileGrid;

    private MouseActions actions;

    [SerializeField] private float speed = 1.4f;

    private Vector3Int? previousFinishPosition;
    private ModalEventBus modalEventBus = ModalEventBus.Instance;

    void Awake()
    {
        actions = new MouseActions();
    }

    void OnEnable()
    {
        actions.Enable();
    }

    void OnDisable()
    {
        actions.Disable();
    }

    void Start()
    {
        actions.ActionMap.LMB.performed += ctx =>
        {
            Debug.Log("CLICK POSITION - " + GetMousePosition());
            Move(GetMousePosition());
        };
    }

    Vector3 GetMousePosition()
    {
        Vector3 mouseScreenPosition = new Vector3(
            (float)Mouse.current.position.x.ReadValue(),
            (float)Mouse.current.position.y.ReadValue(),
            0.0f
        );
        return Camera.main.ScreenToWorldPoint(mouseScreenPosition);
    }

    void HighlightFinishPosition(Vector3Int finishPosition)
    {
        if ((finishPosition).Equals(previousFinishPosition)) return;
        if (previousFinishPosition != null)
            finishTilemap.SetTile((Vector3Int)previousFinishPosition, null);
        finishTilemap.SetTile(finishPosition, finishTile);
        finishTilemap.SetColor(finishPosition, new Color(0, 0, 0, 125.0f));
        previousFinishPosition = finishPosition;
    }

    void Move(Vector3 position)
    {
        if (CanMove(position))
        {
            HighlightFinishPosition(GetGridCellPosition(position));
            Vector3 cellCenterPosition = tileGrid.GetCellCenterWorld(GetGridCellPosition(position));
            transform.DOMove(cellCenterPosition, 10 / speed);
        }
        else
        {
            modalEventBus.ShowNotification("НЕЛЬЗЯ");
        }
    }

    Vector3Int GetGridCellPosition(Vector3 position)
    {
        return tileGrid.WorldToCell(position);
    }

    bool CanMove(Vector3 position)
    {
        Vector3Int gridPosition = GetGridCellPosition(position);
        if (!groundTilemap.HasTile(gridPosition) || collisionTilemap.HasTile(gridPosition))
            return false;
        return true;
    }
}