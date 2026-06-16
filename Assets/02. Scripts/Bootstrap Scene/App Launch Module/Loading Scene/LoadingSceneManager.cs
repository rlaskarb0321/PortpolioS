using System;
using System.Collections.Generic;
using UnityEngine;

public enum ELoadingSceneType
{
    AppInitLoading,
    StageModeLoading,
    MultiplayModeLoading,
    Count
}

public class LoadingSceneManager : MonoBehaviour, IBootstrapInstance
{
    [SerializeField] private LoadingCanvas[] loadingCanvases;

    private Dictionary<ELoadingSceneType, LoadingCanvas> canvases;

    public EBootstrapInstance GetInstanceType()
    {
        return EBootstrapInstance.LoadingSceneManager;
    }

    public void AllocateToBootstrapInstance()
    {
        BootstrapSceneInstance.Instance.AllocateToBootstrapInstance(GetInstanceType(), this);
    }

    public void Execute()
    {
    }

    public void ActivateLoadingCanvas(ELoadingSceneType targetCanvas)
    {
        foreach (ELoadingSceneType loadingSceneType in canvases.Keys)
        {
            canvases[loadingSceneType].ToggleCanvas(targetCanvas != loadingSceneType);
        }
    }
    
    
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
}