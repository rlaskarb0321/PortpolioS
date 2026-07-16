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
    [SerializeField] private List<PlayableCharacterInfo> playableCharacterInfos;
    [SerializeField] private List<string> columIds;
    
    protected override void DeserializeFlattenRows(LitJson.JsonData flattenRows)
    {
        playableCharacterInfos.Clear();

        foreach (LitJson.JsonData gameData in flattenRows)
        {
            int colIndex = 0;
            PlayableCharacterInfo info = new PlayableCharacterInfo();
            
            info.name = gameData[columIds[colIndex++]].ToString();
            info.starCount = int.Parse(gameData[colIndex++].ToString());
            playableCharacterInfos.Add(info);
        }
    }
}