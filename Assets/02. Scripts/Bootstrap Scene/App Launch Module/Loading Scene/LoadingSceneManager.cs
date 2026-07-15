using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

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
        
        OnSceneActivated = null;
        OnCompleteLoad = null;
        strategy.Init(targetCanvas);
        // foreach (ELoadingSceneType loadingSceneType in canvases.Keys)
        // {
        //     if (targetCanvas == loadingSceneType)
        //     {
        //         canvases[loadingSceneType].ToggleCanvas(true);
        //         
        //         // subscribes a callback to turn off the enabled Canvas upon successful scene load.
        //         OnCompleteLoad += () => canvases[loadingSceneType].ToggleCanvas(false);
        //     }
        //     else
        //     {
        //         canvases[loadingSceneType].ToggleCanvas(false);
        //     }
        // }
        
        // await LoadSceneInBackground(targetCanvas);
        await strategy.LoadSceneInBackground(targetCanvas);
        OnCompleteLoad?.Invoke();
    }

    // private async UniTask LoadNetworkSceneInBackground(string address)
    // {
    //     var runner = BootstrapSceneInstance.Instance.NetworkRunner;
    //     
    //     if (networkScene.IsValid)
    //         await runner.UnloadScene(networkScene);
    //     if (sceneHandle.IsValid())
    //         await Addressables.UnloadSceneAsync(sceneHandle).ToUniTask();
    //
    //     networkScene = SceneRef.FromPath(address);
    //     
    //     await runner.LoadScene(networkScene, LoadSceneMode.Additive);
    //     await UniTask.Delay(TimeSpan.FromSeconds(loadDelay));
    //     
    //     if (OnSceneActivated != null)
    //         await OnSceneActivated.Invoke();
    // }

    // private async UniTask LoadSceneInBackground(ELoadingSceneType sceneType)
    // {
    //     if (sceneHandle.IsValid())
    //         await Addressables.UnloadSceneAsync(sceneHandle).ToUniTask();
    //
    //     sceneHandle = Addressables.LoadSceneAsync(scenes[(int)sceneType], LoadSceneMode.Additive);
    //     
    //     await sceneHandle.ToUniTask();
    //     await UniTask.Delay(TimeSpan.FromSeconds(loadDelay));
    //
    //     if (OnSceneActivated != null)
    //         await OnSceneActivated.Invoke();
    // }
    
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