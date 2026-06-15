using System;
using UnityEngine;

public class LoadingCanvas : MonoBehaviour
{
    [SerializeField] private ELoadingSceneType loadingCanvasType;

    private Canvas canvas;
    
    public ELoadingSceneType LoadingCanvasType => loadingCanvasType;

    public void ToggleCanvas(bool isActive)
    {
        canvas.enabled = isActive;
    }
    
    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }
}
