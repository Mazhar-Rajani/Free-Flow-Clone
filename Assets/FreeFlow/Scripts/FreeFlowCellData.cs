using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class FreeFlowCellData
{
    public int id = default;
    public Color color = default;

    [DisableIf("CheckActiveButton")]
    [Button("Select")]
    public void Select()
    {
        FreeFlowLevel.activeColor = this;
    }

    public bool CheckActiveButton()
    {
        if(FreeFlowLevel.activeColor != null)
        {
            if (FreeFlowLevel.activeColor.id == id)
            {
                return true;
            }
        }
        return false;
    }
}