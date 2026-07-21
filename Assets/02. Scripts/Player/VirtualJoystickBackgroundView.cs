using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class VirtualJoystickBackgroundView : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    public UnityAction<Vector2> OnInteractJoystick;

    private RectTransform rectTransform;
    
    public void OnDrag(PointerEventData eventData)
    {
        OnInteractJoystick?.Invoke(CalculateLocalPosition(eventData));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnInteractJoystick?.Invoke(CalculateLocalPosition(eventData));
    }

    private Vector2 CalculateLocalPosition(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPos))
        {
            Vector2 center = rectTransform.rect.center;
            Vector2 offset = localPos - center;
            float radius = Mathf.Min(rectTransform.rect.width, rectTransform.rect.height) * 0.5f;
            Vector2 inputDir = offset / radius;

            if (inputDir.magnitude > 1f)
            {
                inputDir = inputDir.normalized;
            }

            return inputDir;
        }

        return Vector2.zero;
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        OnInteractJoystick += arg0 =>
        {
            Debug.Log($"방향 벡터: {arg0} (길이: {arg0.magnitude:F2})");
        };
    }
}
