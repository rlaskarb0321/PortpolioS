using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu
(
    fileName = "Playable Character Info Data",
    menuName = "Scriptable Objects/Playable Character Info Data",
    order = int.MaxValue
)]
public class PlayableCharacterInfoSO : ChartDataSOBase
{
    [SerializeField] private List<string> columIds;

    private Dictionary<int, PlayableCharacterInfo> infos;
    
    protected override void DeserializeFlattenRows(LitJson.JsonData flattenRows)
    {
        if (infos == null) infos = new Dictionary<int, PlayableCharacterInfo>();
        
        infos.Clear();
        foreach (LitJson.JsonData gameData in flattenRows)
        {
            int colIndex = 0;
            PlayableCharacterInfo info = new PlayableCharacterInfo();

            info.index = int.Parse(gameData[colIndex++].ToString());
            info.name = gameData[columIds[colIndex++]].ToString();
            info.starCount = int.Parse(gameData[colIndex++].ToString());
            infos.Add(info.index, info);
        }
    }
}