using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using Sirenix.Utilities;

[CreateAssetMenu(fileName = "FreeFlowLevel_", menuName = "Create Free Flow Level")]
public class FreeFlowLevel : SerializedScriptableObject
{
    public static FreeFlowCellData activeColor;

    [SerializeField] private int rows = default;
    [SerializeField] private int cols = default;
    [TableMatrix(DrawElementMethod = "DrawCell", SquareCells = true, HideRowIndices = true, HideColumnIndices = true)]
    [SerializeField] private FreeFlowCellData[,] cells = default;
    [SerializeField] [TableList(AlwaysExpanded = true, ShowPaging = false, ShowIndexLabels = false)] private List<FreeFlowCellData> colors = default;

    public int Rows => rows;
    public int Cols => cols;
    public FreeFlowCellData[,] Cells => cells;
    public List<FreeFlowCellData> Colors => colors;

#if UNITY_EDITOR
    private static FreeFlowCellData DrawCell(Rect rect, FreeFlowCellData value)
    {
        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            if (activeColor != null)
            {
                value = activeColor;
                GUI.changed = true;
                Event.current.Use();
            }
        }

        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.normal.textColor = Color.black;
        style.fontSize = 24;
        EditorGUI.DrawRect(rect.Padding(1.5f), value.color);
        EditorGUI.LabelField(rect.Padding(1), value.id == -1 ? string.Empty : value.id.ToString(), style);

        return value;
    }
#endif

    [PropertyOrder(-1)]
    [Button("Reset Grid")]
    private void CreateGrid()
    {
        activeColor = null;

        cells = new FreeFlowCellData[rows, cols];
        for (int c = 0; c < rows; c++)
        {
            for (int r = 0; r < cols; r++)
            {
                FreeFlowCellData cell = new FreeFlowCellData();
                cell.id = -1;
                cell.color = Color.white;
                cells[r, c] = cell;
            }
        }
    }

    [PropertyOrder(1)]
    [Button("Clear Selection")]
    private void ClearSelection()
    {
        activeColor = null;
    }
}