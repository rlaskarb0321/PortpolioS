using System;
using System.Collections.Generic;
using Codice.Client.BaseCommands;
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
    
    protected override void DeserializeFlattenRows(LitJson.JsonData flattenRows)
    {
        multiplayDefinitions.Clear();

        foreach (LitJson.JsonData gameData in flattenRows)
        {
            int colIndex = 0;

            MultiplayStageDefinition definition = new MultiplayStageDefinition
            {
                sessionName = (string)gameData[columIds[colIndex++]],
                stageIndex = (int)gameData[columIds[colIndex++]],
                multiplayMapType = (EMultiplayType)Enum.Parse(
                    typeof(EMultiplayType), (string)gameData[columIds[colIndex++]]),
                multiplayMapTypeIndex = (int)gameData[columIds[colIndex++]],
                bgmAddress = (string)gameData[columIds[colIndex++]],
            };

            multiplayDefinitions.Add(definition);
        }
    }
}