using UnityEngine;

public class VirtualJoystickPresenter : MonoBehaviour
{
    [SerializeField] private VirtualJoystickBackgroundView joystickBackgroundView;
    [SerializeField] private VirtualJoystickHandleView joystickHandleView;

    private void OnEnable()
    {
        joystickBackgroundView.OnInteractJoystick += HandleInteractJoystick;
    }

    private void HandleInteractJoystick(Vector2 normalizedDir)
    {
        joystickHandleView.SetPosition(normalizedDir);
    }

    private void OnDisable()
    {
        joystickBackgroundView.OnInteractJoystick -= HandleInteractJoystick;
    }
}
