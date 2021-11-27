using System;
using UnityEngine;

public class FreeFlowInput : MonoBehaviour
{
    public static event Action OnLevelComplete;

    [SerializeField] private FreeFlowLevelGenerator levelGenerator = default;

    private Camera mainCam;
    private FreeFlowCell currCell;
    private FreeFlowCell prevCell;
    private FreeFlowPath selectedPath;
    private bool isSelected;

    public bool IsInputEnabled { get; set; }

    private void Awake()
    {
        mainCam = Camera.main;
        FreeFlowLevelGenerator.OnCreateGrid += OnCreateGrid;
    }

    private void OnDestroy()
    {
        FreeFlowLevelGenerator.OnCreateGrid -= OnCreateGrid;
    }

    private void OnCreateGrid()
    {
        IsInputEnabled = true;
    }

    private void Update()
    {
        if (!IsInputEnabled)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseDown();
        }
        if (Input.GetMouseButton(0))
        {
            HandleMouseDrag();
        }
        if (Input.GetMouseButtonUp(0))
        {
            HandleMouseUp();
        }
    }

    private void HandleMouseDown()
    {
        currCell = TryGetCell(GetWorldMousePos());
        prevCell = currCell;

        if (currCell != null)
        {
            if (currCell.CellData.id == -1)
            {
                if (currCell.OccupiedId != -1)
                {
                    FreeFlowPath path = levelGenerator.Paths.Find(x => x.CellData.id == currCell.OccupiedId);
                    if (path.PathCells[path.PathCells.Count - 1].Index == currCell.Index)
                    {
                        path.UpdateLine(currCell);
                        selectedPath = path;
                        isSelected = true;
                    }
                }
            }
            else
            {
                FreeFlowPath path = levelGenerator.Paths.Find(x => x.CellData.id == currCell.CellData.id);
                path.ResetPath();
                path.UpdateLine(currCell);
                path.StartCell = currCell;
                selectedPath = path;
                isSelected = true;
            }
        }
    }

    private void HandleMouseDrag()
    {
        if (!isSelected)
            return;

        currCell = TryGetCell(GetWorldMousePos());

        if (currCell == null)
            return;

        if (currCell.Index != prevCell.Index && IsNeighbourCell())
        {
            if (!IsIntersectingWithSamePath())
            {
                if (currCell.CellData.id == -1)
                {
                    if (currCell.OccupiedId != -1)
                    {
                        FreeFlowPath intersectingPath = levelGenerator.Paths.Find(x => x.CellData.id == currCell.OccupiedId);
                        intersectingPath.CutPath(currCell);
                    }
                    selectedPath.UpdateLine(currCell);
                    prevCell = currCell;
                }
                else
                {
                    if (currCell.CellData.id == selectedPath.CellData.id)
                    {
                        selectedPath.UpdateLine(currCell);
                        prevCell = currCell;
                    }
                }

                if (IsLevelComplete())
                {
                    OnLevelComplete?.Invoke();
                }
            }
        }
    }

    private void HandleMouseUp()
    {
        currCell = null;
        prevCell = null;
        selectedPath = null;
        isSelected = false;

        for (int i = 0; i < levelGenerator.Paths.Count; i++)
        {
            levelGenerator.Paths[i].UpdatePathColor();
        }
    }

    private bool IsIntersectingWithSamePath()
    {
        if (selectedPath.PathCells.Contains(currCell))
        {
            return true;
        }
        return false;
    }

    private bool IsNeighbourCell()
    {
        float prevX = prevCell.transform.position.x;
        float prevY = prevCell.transform.position.y;
        float currX = currCell.transform.position.x;
        float currY = currCell.transform.position.y;
        float x = levelGenerator.XSpacing;
        float y = levelGenerator.YSpacing;

        bool left = (prevX == currX + x && prevY == currY);
        bool right = (prevX == currX - x && prevY == currY);
        bool up = (prevY == currY - y && prevX == currX);
        bool down = (prevY == currY + y && prevX == currX);

        if (left || right || up || down)
        {
            return true;
        }
        return false;
    }

    private bool IsLevelComplete()
    {
        int cellCount = levelGenerator.Cells.Count;
        for (int i = 0; i < cellCount; i++)
        {
            if (levelGenerator.Cells[i].OccupiedId == -1)
            {
                return false;
            }
        }
        return true;
    }

    private Vector2 GetWorldMousePos()
    {
        if (mainCam == null)
            return Vector2.zero;

        return mainCam.ScreenToWorldPoint(Input.mousePosition);
    }

    private FreeFlowCell TryGetCell(Vector2 point)
    {
        RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);
        if (hit.collider != null)
        {
            if (hit.collider.TryGetComponent(out FreeFlowCell cell))
            {
                return cell;
            }
        }
        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        Gizmos.DrawSphere(GetWorldMousePos(), 0.3f);

        if (currCell != null)
        {
            Gizmos.DrawSphere(currCell.transform.position, 0.3f);
        }
    }
}