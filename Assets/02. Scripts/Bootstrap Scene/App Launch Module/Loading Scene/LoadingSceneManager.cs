using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
    Count
}

public class LoadingSceneManager : MonoBehaviour, IBootstrapInstance
{
    public Func<UniTask> OnSceneActivated;
    public Action OnCompleteLoad;
    
    [SerializeField] private LoadingCanvas[] loadingCanvases;
    [SerializeField] private AssetReference[] scenes;
    [SerializeField] private float loadDelay = 0.8f;

    private Dictionary<ELoadingSceneType, LoadingCanvas> canvases;
    private AsyncOperationHandle<SceneInstance> sceneHandle;

    private void Awake()
    {
        canvases = new Dictionary<ELoadingSceneType, LoadingCanvas>();
    }

    public void Start()
    {
        for (int i = 0; i < loadingCanvases.Length; i++)
        {
            canvases.Add(loadingCanvases[i].LoadingCanvasType, loadingCanvases[i]);
        }

        AllocateToBootstrapInstance();
        Execute();
    }
    
    public void Execute()
    {
        ActivateLoadingCanvas(ELoadingSceneType.SelectModeLoading).Forget();
    }

    public async UniTask ActivateLoadingCanvas(ELoadingSceneType targetCanvas)
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
        await LoadSceneInBackground(targetCanvas);
        OnCompleteLoad?.Invoke();
    }

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
}