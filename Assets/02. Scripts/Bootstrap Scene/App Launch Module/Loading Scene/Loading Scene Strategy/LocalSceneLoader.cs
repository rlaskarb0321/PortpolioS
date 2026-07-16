using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class LocalSceneLoader : SceneLoadStrategyBase
{
    public override void Init(ELoadingSceneType sceneType)
    {
        base.Init(sceneType);
        ActivateLoadingCanvas(sceneType);
    }

    public override async UniTask LoadSceneInBackground(ELoadingSceneType sceneType)
    {
        if (LoadingSceneManager.CurrentSceneHandle.IsValid())
            await Addressables.UnloadSceneAsync(LoadingSceneManager.CurrentSceneHandle).ToUniTask();

        LoadingSceneManager.CurrentSceneHandle = Addressables.LoadSceneAsync
            (LoadingSceneManager.Canvases[sceneType].addressableScene, LoadSceneMode.Additive);

        await LoadingSceneManager.CurrentSceneHandle.ToUniTask();
        await UniTask.Delay(TimeSpan.FromSeconds(LoadingSceneManager.LoadDelay));

        if (LoadingSceneManager.OnSceneActivated != null)
            await LoadingSceneManager.OnSceneActivated.Invoke();

        LoadingSceneManager.OnCompleteLoad?.Invoke();

        ClearLoadingSceneEvent();
    }
}
