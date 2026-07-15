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
    
    [SerializeField] private LoadingCanvas[] loadingCanvases;
    [SerializeField] private AssetReference[] scenes;
    [SerializeField] private float loadDelay = 0.8f;

    private Dictionary<ELoadingSceneType, LoadingCanvas> canvases;
    private Dictionary<ESceneLoadStrategy, SceneLoadStrategyBase> sceneLoadStrategyDict;
    private AsyncOperationHandle<SceneInstance> sceneHandle;

    public void Start()
    {
        for (int i = 0; i < loadingCanvases.Length; i++)
        {
            canvases.Add(loadingCanvases[i].LoadingCanvasType, loadingCanvases[i]);
        }

        for (int i = 0; i < sceneLoadStrategies.Count; i++)
        {
            sceneLoadStrategyDict.Add(sceneLoadStrategies[i].LoadStrategy, sceneLoadStrategies[i]);
        }

        AllocateToBootstrapInstance();
        Execute();
    }
    
    public void Execute()
    {
        ActivateLoadingCanvas(ELoadingSceneType.SelectModeLoading).Forget();
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
        
        OnSceneActivated = null;
        OnCompleteLoad = null;
        foreach (ELoadingSceneType loadingSceneType in canvases.Keys)
        {
            if (targetCanvas == loadingSceneType)
            {
                canvases[loadingSceneType].ToggleCanvas(true);
                
                // subscribes a callback to turn off the enabled Canvas upon successful scene load.
                OnCompleteLoad += () => canvases[loadingSceneType].ToggleCanvas(false);
            }
            else
            {
                canvases[loadingSceneType].ToggleCanvas(false);
            }
        }

        // After Scene Load finishes, each Scene instance subscribes to OnCompleteLoad.
        // if (isMultiplay == false)
        //     await LoadSceneInBackground(targetCanvas);
        // else
        // {
        //     string address = "Assets/01. Scenes/Game Mode - InGame Multiplay.unity";
        //     
        //     await LoadNetworkSceneInBackground(address);
        // }
        
        await LoadSceneInBackground(targetCanvas);
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

    private async UniTask LoadSceneInBackground(ELoadingSceneType sceneType)
    {
        if (sceneHandle.IsValid())
            await Addressables.UnloadSceneAsync(sceneHandle).ToUniTask();

        sceneHandle = Addressables.LoadSceneAsync(scenes[(int)sceneType], LoadSceneMode.Additive);
        
        await sceneHandle.ToUniTask();
        await UniTask.Delay(TimeSpan.FromSeconds(loadDelay));

        if (OnSceneActivated != null)
            await OnSceneActivated.Invoke();
    }
    
    private void OnDestroy()
    {
        if (sceneHandle.IsValid() == false)
            return;

        if (sceneHandle.IsDone)
            Addressables.UnloadSceneAsync(sceneHandle);
        else
            Addressables.Release(sceneHandle);
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
        canvases = new Dictionary<ELoadingSceneType, LoadingCanvas>();
        sceneLoadStrategyDict = new Dictionary<ESceneLoadStrategy, SceneLoadStrategyBase>();
    }
}