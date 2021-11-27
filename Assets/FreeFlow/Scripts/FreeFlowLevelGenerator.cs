using System;
using System.Collections.Generic;
using UnityEngine;

public class FreeFlowLevelGenerator : MonoBehaviour
{
    public static event Action OnCreateGrid;

    [SerializeField] private bool showDebug = default;
    [SerializeField] private float debugAlpha = default;
    [SerializeField] private Vector2 size = default;
    [SerializeField] private FreeFlowLevel level = default;
    [SerializeField] private FreeFlowCell cellPrefab = default;
    [SerializeField] private FreeFlowPath pathPrefab = default;
    [SerializeField] private SpriteRenderer background = default;
    [SerializeField] private Transform cellHolder = default;

    public List<FreeFlowCell> Cells { get; private set; }
    public List<FreeFlowPath> Paths { get; private set; }
    public FreeFlowLevel FreeFlowLevel { get => level; }
    public float XSpacing { get; private set; }
    public float YSpacing { get; private set; }

    private void Start()
    {
        CreateGrid(level);
    }

    public void CreateGrid(FreeFlowLevel freeFlowLevel)
    {
        Cleanup();

        XSpacing = size.x / (float)level.Rows;
        YSpacing = size.y / (float)level.Cols;
        Vector3 cellSize = new Vector3(XSpacing, YSpacing, 1f);
        Vector3 cellPos = transform.position;

        Cells = new List<FreeFlowCell>();
        int k = 0;
        for (int r = 0; r < level.Rows; r++)
        {
            for (int c = 0; c < level.Cols; c++)
            {
                FreeFlowCell cell = Instantiate(cellPrefab, cellHolder);
                cell.transform.position = cellPos;
                cell.transform.localScale = cellSize;
                cell.SetData(k, level.Cells[c, r]);
                cellPos.x += XSpacing;
                Cells.Add(cell);
                k++;
            }
            cellPos.x = transform.position.x;
            cellPos.y -= YSpacing;
        }

        Paths = new List<FreeFlowPath>();
        for (int i = 0; i < level.Colors.Count; i++)
        {
            FreeFlowCellData cellData = level.Colors[i];
            if (cellData.id != -1)
            {
                FreeFlowPath path = Instantiate(pathPrefab, transform);
                FreeFlowCell a = Cells.Find(x => x.CellData.id == cellData.id);
                FreeFlowCell b = Cells.FindLast(x => x.CellData.id == cellData.id);
                path.Init(cellData, a, b);
                Paths.Add(path);
            }
        }
        Vector3 bgPos = transform.position;
        bgPos.x -= XSpacing / 2f;
        bgPos.y += YSpacing / 2f;
        background.transform.position = bgPos;
        background.transform.localScale = new Vector3(size.x / background.bounds.size.x, size.y / background.bounds.size.y, 1f);

        OnCreateGrid?.Invoke();
    }

    private void Cleanup()
    {
        for (int i = 0; i < cellHolder.childCount; i++)
        {
            Destroy(cellHolder.GetChild(i).gameObject);
        }

        if (Paths != null)
        {
            foreach (FreeFlowPath path in Paths)
            {
                Destroy(path.gameObject);
            }
            Paths.Clear();
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebug)
            return;

        if (level == null)
            return;

        float xSpacing = size.x / (float)level.Rows;
        float ySpacing = size.y / (float)level.Cols;
        Vector3 cellSize = new Vector3(xSpacing, ySpacing, 1f);
        Vector3 pos = transform.position;

        for (int r = 0; r < level.Rows; r++)
        {
            for (int c = 0; c < level.Cols; c++)
            {
                Color color = level.Cells[c, r].color;
                color.a = debugAlpha;
                Gizmos.color = color;
                Gizmos.DrawCube(pos, cellSize);
                pos.x += xSpacing;
            }
            pos.x = transform.position.x;
            pos.y -= ySpacing;
        }
    }
}