using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class SceneLoadStrategyBase : MonoBehaviour
{
    [Header("Scene Load Strategy")]
    [SerializeField] private ESceneLoadStrategy loadStrategy;
    
    [Header("Loading Canvases & Scene Refers")]
    [SerializeField] private LoadingCanvas[] loadingCanvases;
    [SerializeField] private SceneCatalogEntry[] sceneCatalogEntries;
    
    private LoadingSceneManager loadingSceneManager;
    
    public ESceneLoadStrategy LoadStrategy { get => loadStrategy; }
    protected LoadingSceneManager LoadingSceneManager { get => loadingSceneManager; }
    protected LoadingCanvas[] LoadingCanvases { get => loadingCanvases; }
    protected SceneCatalogEntry[] SceneCatalogEntries { get => sceneCatalogEntries;}

    protected virtual void Init()
    {
        loadingSceneManager = 
            BootstrapSceneInstance.Instance
            .GetBootstrapInstance<LoadingSceneManager>(EBootstrapInstance.LoadingSceneManager);
    }

    public abstract void ActivateLoadingCanvas(ELoadingSceneType targetCanvas);

    public abstract UniTask LoadSceneInBackground(ELoadingSceneType sceneType);
}

public enum ESceneLoadStrategy
{
    LocalSceneLoad,
    NetworkSceneLoad,
    Count
}

[System.Serializable]
public struct SceneCatalogEntry
{
    public AssetReference addressableScene;
    public string sceneAddress;
}