using System;
using UnityEngine;

public class FreeFlowCell : MonoBehaviour
{
    [SerializeField] private float bgAlpha = default;
    [SerializeField] SpriteRenderer bg;
    [SerializeField] SpriteRenderer circle;

    public int Index { get; private set; }
    public int OccupiedId { get; set; }
    public FreeFlowCellData CellData { get; private set; }

    public void SetData(int index, FreeFlowCellData data)
    {
        Index = index;
        CellData = data;
        OccupiedId = -1;
        circle.color = CellData.color;
        if(CellData.id == -1)
        {
            circle.enabled = false;
        }
    }

    public void UpdateBackgroundColor(Color color)
    {
        color.a = bgAlpha;
        bg.color = color;
    }
}