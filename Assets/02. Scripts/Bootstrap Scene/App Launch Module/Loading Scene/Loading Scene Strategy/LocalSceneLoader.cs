using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class LocalSceneLoader : SceneLoadStrategyBase
{
    private Dictionary<ELoadingSceneType, LoadingCanvas> canvases;
    private AsyncOperationHandle<SceneInstance> sceneHandle;
    
    public override void ActivateLoadingCanvas(ELoadingSceneType targetCanvas)
    {
        if (canvases.Count <= 0)
        {
            Init();
        }
        
        foreach (ELoadingSceneType loadingSceneType in canvases.Keys)
        {
            if (targetCanvas == loadingSceneType)
            {
                canvases[loadingSceneType].ToggleCanvas(true);
 
                // subscribes a callback to turn off the enabled Canvas upon successful scene load.
                LoadingSceneManager.OnCompleteLoad += () => canvases[loadingSceneType].ToggleCanvas(false);
            }
            else
            {
                canvases[loadingSceneType].ToggleCanvas(false);
            }
        }
        // await LoadSceneInBackground(targetCanvas);
    }

    public override async UniTask LoadSceneInBackground(ELoadingSceneType sceneType)
    {
        if (sceneHandle.IsValid())
            await Addressables.UnloadSceneAsync(sceneHandle).ToUniTask();

        sceneHandle = Addressables.LoadSceneAsync(SceneCatalogEntries[(int)sceneType].addressableScene, LoadSceneMode.Additive);
     
        await sceneHandle.ToUniTask();
        await UniTask.Delay(TimeSpan.FromSeconds(LoadingSceneManager.LoadDelay));

        if (LoadingSceneManager.OnSceneActivated != null)
            await LoadingSceneManager.OnSceneActivated.Invoke();
    }

    private void Awake()
    {
        canvases = new Dictionary<ELoadingSceneType, LoadingCanvas>();
    }

    protected override void Init()
    {
        base.Init();
        for (int i = 0; i < LoadingCanvases.Length; i++)
        {
            canvases.Add(LoadingCanvases[i].LoadingCanvasType, LoadingCanvases[i]);
        }
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
}
