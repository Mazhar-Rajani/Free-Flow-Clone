using System;
using System.Collections.Generic;
using UnityEngine;

public class FreeFlowPath : MonoBehaviour
{
    [SerializeField] private LineRenderer linePrefab = default;
    [SerializeField] private float lineWidth = default;

    public FreeFlowCellData CellData { get; private set; }
    public LineRenderer LineRenderer { get; private set; }
    public List<FreeFlowCell> PathCells { get; private set; }
    public FreeFlowCell PointA { get; private set; }
    public FreeFlowCell PointB { get; private set; }
    public FreeFlowCell StartCell { get; set; }

    public void Init(FreeFlowCellData data, FreeFlowCell a, FreeFlowCell b)
    {
        PathCells = new List<FreeFlowCell>();

        CellData = data;

        LineRenderer = Instantiate(linePrefab, transform);
        LineRenderer.startColor = data.color;
        LineRenderer.endColor = data.color;
        LineRenderer.startWidth = lineWidth;
        LineRenderer.endWidth = lineWidth;

        PointA = a;
        PointB = b;
    }

    public void ResetPath()
    {
        foreach (FreeFlowCell cell in PathCells)
        {
            cell.UpdateBackgroundColor(Color.white);
            cell.OccupiedId = -1;
        }
        PathCells.Clear();
        LineRenderer.positionCount = 0;
    }

    public void UpdateLine(FreeFlowCell cell)
    {
        if (!PathCells.Contains(cell))
        {
            cell.OccupiedId = CellData.id;
            PathCells.Add(cell);
            LineRenderer.positionCount++;
            LineRenderer.SetPosition(LineRenderer.positionCount - 1, cell.transform.position);
        }
    }

    public void UpdatePathColor()
    {
        for (int i = 0; i < PathCells.Count; i++)
        {
            PathCells[i].UpdateBackgroundColor(CellData.color);
        }
    }

    public void CutPath(FreeFlowCell cell)
    {
        if (PathCells.Contains(cell))
        {
            int index = PathCells.IndexOf(cell);
            LineRenderer.positionCount = index;
            for (int i = PathCells.Count - 1; i >= index; i--)
            {
                PathCells[i].UpdateBackgroundColor(Color.white);
                PathCells[i].OccupiedId = -1;
                PathCells.RemoveAt(i);
            }
        }
    }
}