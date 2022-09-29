using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;


public class MouseMovementController : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap collisionTilemap;
    [SerializeField] private Tilemap finishTilemap;
    [SerializeField] private Grid tileGrid;

    private MouseActions actions;
    public Animator anim;

    [SerializeField] private float speed = 10.0f;

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
        anim = GetComponent<Animator>();
        DrawRadius(GetGridCellPosition(transform.position));
        actions.ActionMap.LMB.performed += ctx =>
        {
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

    void HighlightFinishPosition(Vector3Int finishPosition, bool clear = false)
    {
        var colorToFill = clear ? new Color(255, 255, 255, 1f) : new Color(75, 0, 0, 1f);
        groundTilemap.SetTileFlags(finishPosition, TileFlags.None);
        groundTilemap.SetColor(finishPosition, colorToFill);
    }

    void Move(Vector3 position)
    {
        if (CanMove(position))
        {
            var gridCellPosition = GetGridCellPosition(position);
            Vector3 cellCenterPosition = tileGrid.GetCellCenterWorld(gridCellPosition);

            HighlightFinishPosition(GetGridCellPosition(transform.position), true);
            DrawRadius(GetGridCellPosition(transform.position), true);
            
            
            HighlightFinishPosition(gridCellPosition);
            DrawRadius(GetGridCellPosition(position));
            transform.DOMove(cellCenterPosition, 10 / speed)
            .OnStart(() =>
            {
                anim.SetInteger("AnimState", 2);
                //
                // if (position.y > transform.position.y)
                //     transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                // else
                //     transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                
            }).OnComplete(() =>
            {
                anim.SetInteger("AnimState", 0);
            });
        }
        else
        {
            modalEventBus.ShowNotification("НЕЛЬЗЯ");
        }
    }

    private readonly Vector3Int[] neighbourOddVectorPositions =
    {
        new Vector3Int(1, 1, 0), // odd
        new Vector3Int(1, -1, 0), // odd
    };
    private readonly Vector3Int[] neighbourEvenVectorPositions =
    {
         
        new Vector3Int(-1, 1, 0), // even
        new Vector3Int(-1, -1, 0), // even
    };
    private readonly Vector3Int[] neighbourCommonVectorPositions =
    {
        new Vector3Int(0, 1, 0),
        new Vector3Int(0, -1, 0),
        new Vector3Int(1, 0, 0),
        new Vector3Int(-1, 0, 0),
    };

    public List<Vector3Int> FindAllTileNeighborPositions(Vector3Int gridPosition)
    {
        var extraNeighbours = Mathf.Abs(gridPosition.y) % 2 ==  1 ? neighbourOddVectorPositions : neighbourEvenVectorPositions;
        return neighbourCommonVectorPositions.Concat(extraNeighbours).Select(neighbourPosition => gridPosition + neighbourPosition).ToList();
    }

    void DrawRadius(Vector3Int position, bool clear = false)
    {
        var colorToFill = clear ? new Color(255, 255, 255, 1f) : new Color(0, 125, 0, 1f);
        foreach (var tilePosition in FindAllTileNeighborPositions(position))
        {
            if(!groundTilemap.HasTile(tilePosition)) continue;
            groundTilemap.SetTileFlags(tilePosition, TileFlags.None);
            groundTilemap.SetColor(tilePosition, colorToFill);
        }
    }

    Vector3Int GetGridCellPosition(Vector3 position)
    {
        return tileGrid.WorldToCell(position);
    }

    bool CanMove(Vector3 position)
    {
        Vector3Int gridPosition = GetGridCellPosition(position);
        if (!FindAllTileNeighborPositions(GetGridCellPosition(transform.position)).Contains(gridPosition)) 
            return false;
        if (!groundTilemap.HasTile(gridPosition) || collisionTilemap.HasTile(gridPosition))
            return false;
        return true;
    }
}