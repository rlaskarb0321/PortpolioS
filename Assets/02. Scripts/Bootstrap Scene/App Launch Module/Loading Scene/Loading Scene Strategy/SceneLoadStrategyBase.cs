using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

public abstract class SceneLoadStrategyBase : MonoBehaviour
{
    [Header("Scene Load Strategy")]
    [SerializeField] private ESceneLoadStrategy loadStrategy;
    [SerializeField] private LoadingSceneManager loadingSceneManager;

    public ESceneLoadStrategy LoadStrategy { get => loadStrategy; }
    protected LoadingSceneManager LoadingSceneManager { get => loadingSceneManager; }

    public virtual void Init(ELoadingSceneType sceneType) { }

    public virtual void SubscribeNetworkSceneEvent() { }

    public abstract UniTask LoadSceneInBackground(ELoadingSceneType sceneType);

    public void ActivateLoadingCanvas(ELoadingSceneType targetCanvas)
    {
        foreach (ELoadingSceneType loadingSceneType in loadingSceneManager.Canvases.Keys)
        {
            if (targetCanvas == loadingSceneType)
            {
                loadingSceneManager.Canvases[loadingSceneType].loadingCanvas.ToggleCanvas(true);
 
                // subscribes a callback to turn off the enabled Canvas upon successful scene load.
                loadingSceneManager.OnCompleteLoad += () =>
                    loadingSceneManager.Canvases[loadingSceneType].loadingCanvas.ToggleCanvas(false);
            }
            else
            {
                loadingSceneManager.Canvases[loadingSceneType].loadingCanvas.ToggleCanvas(false);
            }
        }
    }

}

public enum ESceneLoadStrategy
{
    LocalSceneLoad,
    NetworkSceneLoad,
    Count
}

[System.Serializable]
public struct SceneCatalogEntry
{
    public ELoadingSceneType typeKey;
    public LoadingCanvas loadingCanvas;
    public AssetReference addressableScene;
    public string sceneAddress;
}