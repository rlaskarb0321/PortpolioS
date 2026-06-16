using Cysharp.Threading.Tasks;
using UnityEngine;

public class AppLaunchManager : MonoBehaviour
{
    [SerializeField] private AppLaunchModule[] appLaunchModules;
    [SerializeField] private AppLaunchModule lastLaunchModule;

    private void Start()
    {
        ExecuteAllAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTask ExecuteAllAsync(System.Threading.CancellationToken ct)
    {
        foreach (var module in appLaunchModules)
        {
            if (module.appLaunchModuleBase == null) continue;
            await module.appLaunchModuleBase.Execute(ct);
        }

        if (lastLaunchModule.appLaunchModuleBase != null)
            await lastLaunchModule.appLaunchModuleBase.Execute(ct);
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (appLaunchModules == null) return;

        for (int i = 0; i < appLaunchModules.Length; i++)
        {
            if (appLaunchModules[i].moduleName != string.Empty) continue;
            if (appLaunchModules[i].appLaunchModuleBase == null) continue;

            appLaunchModules[i].moduleName = appLaunchModules[i].appLaunchModuleBase.ModuleName;
        }

        if (lastLaunchModule.moduleName == string.Empty && lastLaunchModule.appLaunchModuleBase != null)
        {
            lastLaunchModule.moduleName = lastLaunchModule.appLaunchModuleBase.ModuleName;
        }
    }
    #endif
}

[System.Serializable]
public struct AppLaunchModule
{
    public string moduleName;
    public AppLaunchModuleBase appLaunchModuleBase;
    public float delay;
}