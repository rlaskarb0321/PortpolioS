using System;
using System.Collections.Generic;
using BackEnd;
using BackEnd.Content;
using UnityEngine;

public abstract class ChartDataSOBase : ScriptableObject
{
    [SerializeField] private string chartId;

    protected abstract void DeserializeFlattenRows(LitJson.JsonData flattenRows);

    protected abstract void OnEndedParse();
    
    [ContextMenu("Test")]
    public void LoadChart()
    {
        CustomLogin();
        
        var table = Backend.CDN.Content.Table.Get();
        var chartList = Backend.CDN.Content.Get(table.GetContentTableItemList());
        var chartDict = chartList.GetContentDictionarySortByChartId();
        
        foreach (string key in chartDict.Keys)
        {
            ContentItem content = chartDict[key];
            if (content.selectedChartFileId != chartId.ToString())
                continue;

            string loadedData = content.contentString;
            LitJson.JsonData flattenRows = LitJson.JsonMapper.ToObject(loadedData);
            
            DeserializeFlattenRows(flattenRows);
            break;
        }
        
        OnEndedParse();
    }

    protected virtual void OnValidate()
    {
        
    }

    protected void CustomLogin()
    {
        BackendLogin backendLogin = new BackendLogin();

        Backend.Initialize();
        backendLogin.CustomLogin("user1", "1234");
    }
}
