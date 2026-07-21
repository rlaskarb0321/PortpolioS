using Fusion;
using UnityEngine;

public class VirtualJoystickPresenter : MonoBehaviour
{
    [SerializeField] private VirtualJoystickBackgroundView joystickBackgroundView;
    [SerializeField] private VirtualJoystickHandleView joystickHandleView;

    private Vector2 latestDir;
    private NetworkRunnerController runnerController;

    private void OnEnable()
    {
        joystickBackgroundView.OnInteractJoystick += HandleInteractJoystick;

        runnerController = BootstrapSceneInstance.Instance.RunnerController;
        if (runnerController != null)
            runnerController.InputPolling += HandleInputPolling;
    }

    private void HandleInteractJoystick(Vector2 normalizedDir)
    {
        latestDir = normalizedDir;
        joystickHandleView.SetPosition(normalizedDir);
    }

    private void HandleInputPolling(NetworkRunner runner, NetworkInput input)
    {
        var data = new PlayerInput
        {
            direction = new Vector3(latestDir.x, 0f, latestDir.y)
        };
        
        input.Set(data);
    }

    private void OnDisable()
    {
        joystickBackgroundView.OnInteractJoystick -= HandleInteractJoystick;

        if (runnerController != null)
            runnerController.InputPolling -= HandleInputPolling;
    }
}
