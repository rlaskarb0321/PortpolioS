using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class VirtualJoystickHandleView : MonoBehaviour
{
    [SerializeField] private float moveRadius = 0f;

    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        if (moveRadius <= 0f && rectTransform.parent is RectTransform parentRect)
        {
            moveRadius = Mathf.Min(parentRect.rect.width, parentRect.rect.height) * 0.5f;
        }
    }

    public void SetPosition(Vector2 normalizedDir)
    {
        rectTransform.anchoredPosition = normalizedDir * moveRadius;
    }
}
