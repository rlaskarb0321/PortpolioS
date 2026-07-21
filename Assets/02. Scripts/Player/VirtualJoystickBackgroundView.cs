using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class VirtualJoystickBackgroundView : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
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

    public void OnPointerUp(PointerEventData eventData)
    {
        OnInteractJoystick?.Invoke(Vector2.zero);
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
            Debug.Log($"Dir: {arg0} (Length: {arg0.magnitude:F2})");
        };
    }
}
