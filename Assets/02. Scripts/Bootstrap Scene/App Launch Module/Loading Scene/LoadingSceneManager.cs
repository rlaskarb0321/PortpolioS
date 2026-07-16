using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public enum ELoadingSceneType
{
    SelectModeLoading,
    StageModeLoading,
    MultiplayLobbyLoading,
    InGame_MultiplayLoading,
    Count
}

public class LoadingSceneManager : MonoBehaviour, IBootstrapLifecycle
{
    public Func<UniTask> OnSceneActivated;
    public Action OnCompleteLoad;

    [Header("Scene Load Strategies")]
    [SerializeField] private List<SceneLoadStrategyBase> sceneLoadStrategies;
    
    [Header("Loading Canvases & Scene Refers")]
    [SerializeField] private SceneCatalogEntry[] sceneCatalogEntries;
    [SerializeField] private float loadDelay = 0.8f;

    private Dictionary<ELoadingSceneType, SceneCatalogEntry> canvases;
    private Dictionary<ESceneLoadStrategy, SceneLoadStrategyBase> sceneLoadStrategyDict;
    private AsyncOperationHandle<SceneInstance> currentSceneHandle; 
    private SceneRef currentNetworkScene;                           

    public float LoadDelay { get => loadDelay; }
    public IReadOnlyDictionary<ELoadingSceneType, SceneCatalogEntry> Canvases { get => canvases; }
    public AsyncOperationHandle<SceneInstance> CurrentSceneHandle { get => currentSceneHandle; set => currentSceneHandle = value; }
    public SceneRef CurrentNetworkScene { get => currentNetworkScene; set => currentNetworkScene = value; }

    public void Start()
    {
        for (int i = 0; i < sceneLoadStrategies.Count; i++)
        {
            sceneLoadStrategyDict.Add(sceneLoadStrategies[i].LoadStrategy, sceneLoadStrategies[i]);
        }
        
        if (canvases.Count == 0)
        {
            for (int i = 0; i < sceneCatalogEntries.Length; i++)
            {
                canvases.Add(sceneCatalogEntries[i].loadingCanvas.LoadingCanvasType, sceneCatalogEntries[i]);
            }
        }

        AllocateToBootstrapInstance();
        Execute();
    }
    
    public void Execute()
    {
        ActivateLoadingCanvas(ELoadingSceneType.SelectModeLoading).Forget();
        BootstrapSceneInstance.Instance.OnRunnerCreated += OnNetworkRunnerCreated;
    }

    public async UniTask ActivateLoadingCanvas
        (
            ELoadingSceneType targetCanvas,
            ESceneLoadStrategy loadStrategy = ESceneLoadStrategy.LocalSceneLoad
        )
    {
        if (targetCanvas == ELoadingSceneType.Count)
        {
            Debug.LogError($"TargetCanvas Type is {targetCanvas}");
            return;
        }

        SceneLoadStrategyBase strategy = sceneLoadStrategyDict[loadStrategy];

        if (loadStrategy == ESceneLoadStrategy.NetworkSceneLoad)
        {
            var multiplaySessionContext = 
                BootstrapSceneInstance.Instance
                .GetBootstrapInstance<MultiplaySessionContext>(EBootstrapInstance.MultiplaySessionContext);
            
            multiplaySessionContext.LoadingSceneType = targetCanvas;
            multiplaySessionContext.SceneRef = SceneRef.FromPath(canvases[targetCanvas].sceneAddress);
        }
        
        strategy.Init(targetCanvas);
        await strategy.LoadSceneInBackground(targetCanvas);
    }

    public void ClearSceneEvent()
    {
        OnSceneActivated = null;
        OnCompleteLoad = null;
    }

    public EBootstrapInstance GetInstanceType()
    {
        return EBootstrapInstance.LoadingSceneManager;
    }

    public void AllocateToBootstrapInstance()
    {
        if (BootstrapSceneInstance.Instance == null)
        {
            Debug.LogError("[LoadingSceneManager] Failed to assign to BootstrapSceneInstance");
            return;
        }
        
        BootstrapSceneInstance.Instance.AllocateToBootstrapInstance(GetInstanceType(), this);
    }
    
    private void Awake()
    {
        canvases = new Dictionary<ELoadingSceneType, SceneCatalogEntry>();
        sceneLoadStrategyDict = new Dictionary<ESceneLoadStrategy, SceneLoadStrategyBase>();
    }

    private void OnNetworkRunnerCreated()
    {
        foreach (var strategy in sceneLoadStrategyDict.Values)
        {
            strategy.SubscribeNetworkSceneEvent();
        }
    }

    private void OnDestroy()
    {
        if (currentSceneHandle.IsValid())
            currentSceneHandle.Release();
    }
}