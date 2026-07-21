using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class VirtualJoystickHandleView : MonoBehaviour
{
    [Tooltip("핸들이 중심에서 벗어날 수 있는 최대 반경(px). 0 이하이면 부모 배경 크기로 자동 계산합니다.")]
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

    /// <summary>
    /// 정규화된 방향값(-1~1)을 받아 핸들을 해당 위치로 이동시킵니다.
    /// </summary>
    public void SetPosition(Vector2 normalizedDir)
    {
        rectTransform.anchoredPosition = normalizedDir * moveRadius;
    }
}
