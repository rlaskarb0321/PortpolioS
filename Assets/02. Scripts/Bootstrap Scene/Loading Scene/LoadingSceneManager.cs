using System;
using System.Collections.Generic;
using UnityEngine;

public enum ELoadingSceneType
{
    AppInitLoading,
    Count
}

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField] private LoadingCanvas[] loadingCanvases;

    private Dictionary<ELoadingSceneType, LoadingCanvas> canvases;

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

    private void Start()
    {
        for (int i = 0; i < loadingCanvases.Length; i++)
        {
            canvases.Add(loadingCanvases[i].LoadingCanvasType, loadingCanvases[i]);
        }
    }
}