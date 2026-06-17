using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public enum ELoadingSceneType
{
    SelectModeLoading,
    StageModeLoading,
    MultiplayModeLoading,
    Count
}

public class LoadingSceneManager : MonoBehaviour, IBootstrapInstance
{
    [SerializeField] private LoadingCanvas[] loadingCanvases;
    [SerializeField] private AssetReference[] scenes;
    [SerializeField] private float loadDelay = 0.8f;

    private Dictionary<ELoadingSceneType, LoadingCanvas> canvases;
    private LoadingCanvas activatedCanvas;
    private AsyncOperationHandle<SceneInstance> sceneHandle;
    private WaitForSeconds ws;

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

    public void Execute()
    {
        ActivateLoadingCanvas(ELoadingSceneType.SelectModeLoading);
    }

    public void ActivateLoadingCanvas(ELoadingSceneType targetCanvas)
    {
        foreach (ELoadingSceneType loadingSceneType in canvases.Keys)
        {
            if (targetCanvas == loadingSceneType)
            {
                canvases[loadingSceneType].ToggleCanvas(true);
                activatedCanvas = canvases[loadingSceneType];
            }
            else
            {
                canvases[loadingSceneType].ToggleCanvas(false);
            }
        }

        StartCoroutine(LoadSceneInBackground(targetCanvas));
    }

    private IEnumerator LoadSceneInBackground(ELoadingSceneType sceneType)
    {
        if (sceneHandle.IsValid())
            yield return Addressables.UnloadSceneAsync(sceneHandle);

        sceneHandle = Addressables.LoadSceneAsync(scenes[(int)sceneType], LoadSceneMode.Additive, false);
        yield return ws;
        yield return sceneHandle.Result.ActivateAsync();
        
        activatedCanvas.ToggleCanvas(false);
    }
    
    private void Awake()
    {
        canvases = new Dictionary<ELoadingSceneType, LoadingCanvas>();
        ws = new WaitForSeconds(loadDelay);
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