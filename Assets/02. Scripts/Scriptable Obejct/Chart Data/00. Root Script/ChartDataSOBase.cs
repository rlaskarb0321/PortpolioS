using System;
using System.Collections.Generic;
using BackEnd;
using UnityEngine;

public abstract class ChartDataSOBase : ScriptableObject
{
    [SerializeField] private string chartId;

    protected abstract void DeserializeFlattenRows(LitJson.JsonData flattenRows);
    
    [ContextMenu("Test")]
    public void LoadChart()
    {
        CustomLogin();
        
        var bro2 = Backend.CDN.Content.Table.Get();
        var bro3 = Backend.CDN.Content.Get(bro2.GetContentTableItemList());
        Dictionary<string, BackEnd.Content.ContentItem> dic = bro3.GetContentDictionarySortByChartId();
        
        foreach (string keyName in dic.Keys)
        {
            Debug.Log(dic[keyName].ToString());
        }
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
