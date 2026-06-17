using System;
using UnityEngine;
using DG.Tweening;

public class LoadingCanvas : MonoBehaviour
{
    [Header("Loading Canvas Type")]
    [SerializeField] private ELoadingSceneType loadingCanvasType;

    [Header("Loading DotTween Animation")]
    [SerializeField] private GameObject loadingImage;
    [SerializeField] private int targetRot = 360;
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float rotateInterval = 1.5f;

    private Canvas canvas;
    private Sequence rotateSequence;

    public ELoadingSceneType LoadingCanvasType => loadingCanvasType;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();
    }

    private void OnDestroy()
    {
        rotateSequence?.Kill();
    }

    public void ToggleCanvas(bool isActive)
    {
        canvas.enabled = isActive;
        rotateSequence?.Kill();

        if (isActive)
        {
            loadingImage.transform.localEulerAngles = Vector3.zero;
            rotateSequence = DOTween.Sequence();
            rotateSequence.Append(
                loadingImage.transform.DOLocalRotate(
                    new Vector3(0f, 0f, targetRot), 
                    duration, 
                    RotateMode.FastBeyond360)
            );
            rotateSequence.AppendInterval(rotateInterval);
            rotateSequence.SetLoops(-1, LoopType.Restart);
        }
    }
}
