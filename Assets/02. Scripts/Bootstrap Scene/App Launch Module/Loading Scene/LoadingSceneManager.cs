using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private Dictionary<ELoadingSceneType, LoadingCanvas> canvases;

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
            canvases[loadingSceneType].ToggleCanvas(targetCanvas == loadingSceneType);
        }

        StartCoroutine(LoadSceneInBackground(targetCanvas));
    }

    private IEnumerator LoadSceneInBackground(ELoadingSceneType targetScene)
    {
        yield return null;
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