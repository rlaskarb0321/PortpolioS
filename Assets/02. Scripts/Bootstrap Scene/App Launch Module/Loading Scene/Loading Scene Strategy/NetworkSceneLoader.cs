using System;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class NetworkSceneLoader : SceneLoadStrategyBase
{
    private ELoadingSceneType targetScene;
    private SceneRef targetNetworkScene;
    private bool isDone;

    public override async UniTask LoadSceneInBackground(ELoadingSceneType sceneType)
    {
        await BootstrapSceneInstance.Instance.NetworkRunner.LoadScene(targetNetworkScene, LoadSceneMode.Additive);
        await UniTask.WaitUntil(() => isDone);

        Debug.Log($"[NetworkSceneLoader] Load NetworkScene");
        isDone = false;
    }

    public override void Init(ELoadingSceneType sceneType)
    {
        base.Init(sceneType);

        targetScene = sceneType;
        targetNetworkScene = SceneRef.FromPath(LoadingSceneManager.Canvases[sceneType].sceneAddress);
    }

    private void OnSceneLoadStart(NetworkRunner runner)
    {
        ActivateLoadingCanvas(targetScene);
    }

    private async void OnSceneLoadDone(NetworkRunner runner)
    {
        if (LoadingSceneManager.CurrentSceneHandle.IsValid())
        {
            await Addressables.UnloadSceneAsync(LoadingSceneManager.CurrentSceneHandle).ToUniTask();
            LoadingSceneManager.CurrentSceneHandle = default;
        }
        if (LoadingSceneManager.CurrentNetworkScene.IsValid)
            await runner.UnloadScene(LoadingSceneManager.CurrentNetworkScene);

        LoadingSceneManager.CurrentNetworkScene = targetNetworkScene;
        if (LoadingSceneManager.OnSceneActivated != null)
            await LoadingSceneManager.OnSceneActivated.Invoke();

        await UniTask.Delay(TimeSpan.FromSeconds(LoadingSceneManager.LoadDelay));
        isDone = true;
        Debug.Log($"[NetworkSceneLoader] Set isDone True");
    }

    public override void SubscribeNetworkSceneEvent()
    {
        base.SubscribeNetworkSceneEvent();
        NetworkRunnerController runnerController = BootstrapSceneInstance.Instance.RunnerController;
        
        runnerController.SceneLoadStart -= OnSceneLoadStart;
        runnerController.SceneLoadStart += OnSceneLoadStart;
        runnerController.SceneLoadDone -= OnSceneLoadDone;
        runnerController.SceneLoadDone += OnSceneLoadDone;
    }
}
