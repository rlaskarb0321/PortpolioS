using Cysharp.Threading.Tasks;
using UnityEngine;

public class AppLaunchManager : MonoBehaviour, IBootstrapLifecycle
{
    [Header("Load Loading Scene")]
    [SerializeField] private AppLaunchModule firstLaunchModule;
    
    [Header("App Launch Modules")]
    [SerializeField] private AppLaunchModule[] appLaunchModules;

    public void Start()
    {
        AllocateToBootstrapInstance();
        Execute();
    }

    public EBootstrapInstance GetInstanceType()
    {
        return EBootstrapInstance.AppLaunchManager;
    }

    public void AllocateToBootstrapInstance()
    {
        if (BootstrapSceneInstance.Instance == null)
        {
            Debug.LogError("[AppLaunchManager] Failed to assign to BootstrapSceneInstance");
            return;
        }
        
        BootstrapSceneInstance.Instance.AllocateToBootstrapInstance(GetInstanceType(), this);
    }

    public void Execute()
    {
        LoadLoadingScene();
        ExecuteLaunchAsync(this.GetCancellationTokenOnDestroy()).Forget();
    }

    private void LoadLoadingScene()
    {
        firstLaunchModule.appLaunchModuleBase.ExecuteSync();
    }
    
    private async UniTask ExecuteLaunchAsync(System.Threading.CancellationToken ct)
    {
        foreach (var module in appLaunchModules)
        {
            if (module.appLaunchModuleBase == null)
                continue;
            
            await module.appLaunchModuleBase.ExecuteAsync(ct);
        }
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        if (appLaunchModules == null) return;

        if (firstLaunchModule.moduleName == string.Empty && firstLaunchModule.appLaunchModuleBase != null)
        {
            firstLaunchModule.moduleName = firstLaunchModule.appLaunchModuleBase.ModuleName;
        }

        for (int i = 0; i < appLaunchModules.Length; i++)
        {
            if (appLaunchModules[i].moduleName != string.Empty) continue;
            if (appLaunchModules[i].appLaunchModuleBase == null) continue;

            appLaunchModules[i].moduleName = appLaunchModules[i].appLaunchModuleBase.ModuleName;
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