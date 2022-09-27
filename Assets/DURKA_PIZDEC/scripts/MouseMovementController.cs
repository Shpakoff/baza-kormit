using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;


public class MouseMovementController : MonoBehaviour
{
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap collisionTilemap;
    [SerializeField] private Tilemap finishTilemap ;
    [SerializeField] private TileBase finishTile;
    [SerializeField] private Grid tileGrid;

    private MouseActions actions;

    [SerializeField] private float speed = 1.4f;
    private Vector3? targetPosition;
    
    private Vector3Int? previousFinishPosition;

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
            Debug.Log("CLICK POSITION - " +  GetMousePosition());
            Move(GetMousePosition());
        };
    }

    void Update()
    {
        MoveToTargetPosition();
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
        if(previousFinishPosition != null)
            finishTilemap.SetTile((Vector3Int)previousFinishPosition, null);
        finishTilemap.SetTile(finishPosition, finishTile);
        finishTilemap.SetColor(finishPosition, new Color(0, 0,0, 125.0f));
        previousFinishPosition = finishPosition;
    }

    void SetTargetPosition(Vector3 position)
    {
        Vector3 cellCenterPosition = tileGrid.GetCellCenterWorld(GetGridCellPosition(position));
        targetPosition = cellCenterPosition;
    }

    void MoveToTargetPosition()
    {
        if (targetPosition == null) return;
        Vector3 position = Vector3.Lerp(transform.position, (Vector3)targetPosition, Time.deltaTime * speed);
        transform.position = position;
        if (transform.position.Equals(targetPosition)) targetPosition = null;
    }

    void Move(Vector3 position)
    {
        if (CanMove(position))
        {
            HighlightFinishPosition(GetGridCellPosition(position));
            SetTargetPosition(position);
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