using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu
(
    fileName = "Multiplay Stage Data",
    menuName = "Scriptable Objects/Multiplay Stage Data",
    order = int.MaxValue
)]
public class MultiplayStageDataSO : ChartDataSOBase
{
    [SerializeField] private List<MultiplayStageDefinition> multiplayDefinitions;
    [SerializeField] private List<string> columIds;
    
    public List<MultiplayStageDefinition> MultiplayDefinitions { get => multiplayDefinitions; }
    
    protected override void DeserializeFlattenRows(LitJson.JsonData flattenRows)
    {
        multiplayDefinitions.Clear();

        foreach (LitJson.JsonData gameData in flattenRows)
        {
            int colIndex = 0;
            MultiplayStageDefinition definition = new MultiplayStageDefinition();

            definition.sessionName = gameData[columIds[colIndex++]].ToString();
            definition.stageIndex = int.Parse(gameData[columIds[colIndex++]].ToString());
            definition.multiplayMapType = (EMultiplayType)Enum.Parse(
                typeof(EMultiplayType), gameData[columIds[colIndex++]].ToString());
            definition.multiplayMapTypeIndex = int.Parse(gameData[columIds[colIndex++]].ToString());
            definition.bgmAddress = gameData[columIds[colIndex++]].ToString();
            multiplayDefinitions.Add(definition);
        }
    }
}