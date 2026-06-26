using UnityEngine;

public enum EStageType
{
    Forest,
    Desert,
    Ridge,
    Glacier,
    Count
}

public enum EStageClearState
{
    Locked,
    Cleared,
    Opened,
    Count
}

[System.Serializable]
public struct StageButtonViewData
{
    [Header("Stage Data")]
    public EStageType stageType;
    public EStageClearState clearState;
    public int level;

    [Header("Stage Text Color")]
    [HideInInspector] public Color lockedTextColor;
    [HideInInspector] public Color clearedTextColor;
    [HideInInspector] public Color openedTextColor;
     
    [Header("Stage Te xt Color")]
    [HideInInspector] public Color forestStageColor;
    [HideInInspector] public Color desertStageColor;
    [HideInInspector] public Color ridgeStageColor;
    [HideInInspector] public Color glacierStageColor;
}
