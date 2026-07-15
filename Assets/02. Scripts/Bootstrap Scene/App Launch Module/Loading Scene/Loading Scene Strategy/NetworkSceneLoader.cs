using System;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class NetworkSceneLoader : SceneLoadStrategyBase
{
    private SceneRef currentNetworkScene;
    private bool isDone;

    public override async UniTask LoadSceneInBackground(ELoadingSceneType sceneType)
    {
        var multiplaySessionContext = 
            BootstrapSceneInstance.Instance
            .GetBootstrapInstance<MultiplaySessionContext>(EBootstrapInstance.MultiplaySessionContext);
        
        await BootstrapSceneInstance.Instance.NetworkRunner.LoadScene(multiplaySessionContext.SceneRef, LoadSceneMode.Additive);
        await UniTask.WaitUntil(() => isDone);

        Debug.Log($"[NetworkSceneLoader] Load NetworkScene");
        isDone = false;
    }

    public override void Init(ELoadingSceneType sceneType)
    {
        base.Init(sceneType);

        // targetNetworkScene = SceneRef.FromPath(LoadingSceneManager.Canvases[sceneType].sceneAddress);
    }

    private void OnSceneLoadStart(NetworkRunner runner)
    {
        MultiplaySessionContext sessionContext = BootstrapSceneInstance.Instance
            .GetBootstrapInstance<MultiplaySessionContext>
            (EBootstrapInstance.MultiplaySessionContext);
        
        ActivateLoadingCanvas(sessionContext.LoadingSceneType);
        currentNetworkScene = sessionContext.SceneRef;
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

        LoadingSceneManager.CurrentNetworkScene = currentNetworkScene;
        if (LoadingSceneManager.OnSceneActivated != null)
            await LoadingSceneManager.OnSceneActivated.Invoke();

        await UniTask.Delay(TimeSpan.FromSeconds(LoadingSceneManager.LoadDelay));
        isDone = true;
        LoadingSceneManager.OnCompleteLoad?.Invoke();
        Debug.Log($"[NetworkSceneLoader] LoadingSceneManager.OnCompleteLoad?.Invoke()");
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
