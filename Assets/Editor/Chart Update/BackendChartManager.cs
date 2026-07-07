using BackEnd;
using UnityEditor;
using UnityEngine;

public class BackendChartManager
{
    [MenuItem("Refresh/Refresh Chart Definition _F5")]
    private static void RefreshDataInstancesMenuItem()
    {
        CustomLogin();
        
        foreach (string guid in AssetDatabase.FindAssets($"t:{nameof(ChartDataSOBase)}"))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ChartDataSOBase chartData = AssetDatabase.LoadAssetAtPath<ChartDataSOBase>(path);
            chartData.LoadChart();
            EditorUtility.SetDirty(chartData);
        }

        AssetDatabase.SaveAssets();
        OnEndedParse();
    }
    
    private static void CustomLogin()
    {
        BackendLogin backendLogin = new BackendLogin();
        
        Backend.Initialize();
        backendLogin.CustomLogin("user1", "1234");
    }

    private static void OnEndedParse()
    {
        GameObject backendManager = GameObject.Find("BackendManager");
        if (backendManager == null)
        {
            Debug.LogWarning("BackendManager not found — nothing to destroy.");
            return;
        }
        
        Object.DestroyImmediate(backendManager);
        Debug.Log("BackendManager destroyed");
    }
}
