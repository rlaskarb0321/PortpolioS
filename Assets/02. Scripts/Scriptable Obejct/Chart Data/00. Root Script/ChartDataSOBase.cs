using UnityEngine;
#if UNITY_EDITOR
using BackEnd;
using BackEnd.Content;
#endif

public abstract class ChartDataSOBase : ScriptableObject
{
    [SerializeField] private string chartId;

    protected abstract void DeserializeFlattenRows(LitJson.JsonData flattenRows);

#if UNITY_EDITOR
    public void LoadChart()
    {
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
    }
#endif
}
