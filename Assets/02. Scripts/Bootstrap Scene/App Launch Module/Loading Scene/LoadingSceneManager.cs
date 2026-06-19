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

        LoadSceneInBackground(targetCanvas).Forget();
    }

    private async UniTaskVoid LoadSceneInBackground(ELoadingSceneType sceneType)
    {
        if (sceneHandle.IsValid())
            await Addressables.UnloadSceneAsync(sceneHandle).ToUniTask();

        sceneHandle = Addressables.LoadSceneAsync(scenes[(int)sceneType], LoadSceneMode.Additive, false);
        await UniTask.Delay(TimeSpan.FromSeconds(loadDelay));
        await sceneHandle.Result.ActivateAsync().ToUniTask();

        // You must turn off the Loading Canvas after completing the tasks for each scene.
        // 각 로드 목적지 씬의 static 에게 OnCompleteLoad 와 OnSceneActivated 델리게이트를 달아두고
        // OnSceneActivated 에 UniTask 비동기 메소드들을 구독시킨뒤 실행, OnSceneActivated 의 작업이 다 끝나면 OnCompleteLoad Broadcast
        // OnCompleteLoad 는 아래에 있는 ToggleCanvas 만 하면 되긴하는데 이상한 냄새가남

        activatedCanvas.ToggleCanvas(false);
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