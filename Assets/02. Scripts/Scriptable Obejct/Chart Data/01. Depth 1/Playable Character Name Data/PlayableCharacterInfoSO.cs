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
    [SerializeField] private List<PlayableCharacterInfo> infos;

    public List<PlayableCharacterInfo> Infos => infos;

    protected override void DeserializeFlattenRows(LitJson.JsonData flattenRows)
    {
        if (infos == null) infos = new List<PlayableCharacterInfo>();

        infos.Clear();
        foreach (LitJson.JsonData gameData in flattenRows)
        {
            int colIndex = 0;
            PlayableCharacterInfo info = new PlayableCharacterInfo();

            info.index = int.Parse(gameData[colIndex++].ToString());
            info.name = gameData[columIds[colIndex++]].ToString();
            info.starCount = int.Parse(gameData[colIndex++].ToString());
            infos.Add(info);
        }
        
        Debug.Log($"[PlayableCharacterInfoSO] Playable Character Info Count: {infos.Count}");
    }
}