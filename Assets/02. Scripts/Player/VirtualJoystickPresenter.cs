using Fusion;
using UnityEngine;
using UnityEngine.UI;

public class VirtualJoystickPresenter : MonoBehaviour
{
    [Header("Player Move Related")]
    [SerializeField] private VirtualJoystickBackgroundView joystickBackgroundView;
    [SerializeField] private VirtualJoystickHandleView joystickHandleView;

    [Header("Player Action Buttons")]
    [SerializeField] private Button normalAttackButton;
    [SerializeField] private Button dodgeButton;
    [SerializeField] private Button expertButton;
    [SerializeField] private Button ultimateButton;
    [SerializeField] private Button interactButton;

    private NetworkRunnerController runnerController;
    private Vector2 latestDir;
    private NetworkButtons accumulatedButtons;

    private void OnEnable()
    {
        joystickBackgroundView.OnInteractJoystick += HandleInteractJoystick;

        runnerController = BootstrapSceneInstance.Instance.RunnerController;
        if (runnerController != null)
            runnerController.InputPolling += HandleInputPolling;
        
        normalAttackButton.onClick.AddListener(() => HandlePressActionButton(EPlayerButton.NormalAttack));
        dodgeButton.onClick.AddListener(() => HandlePressActionButton(EPlayerButton.Dodge));
        expertButton.onClick.AddListener(() => HandlePressActionButton(EPlayerButton.Expert));
        ultimateButton.onClick.AddListener(() => HandlePressActionButton(EPlayerButton.Ultimate));
        interactButton.onClick.AddListener(() => HandlePressActionButton(EPlayerButton.Interact));
    }

    private void HandleInteractJoystick(Vector2 normalizedDir)
    {
        latestDir = normalizedDir;
        joystickHandleView.SetPosition(normalizedDir);
    }

    private void HandlePressActionButton(EPlayerButton button)
    {
        accumulatedButtons.Set((int)button, true);
    }

    private void HandleInputPolling(NetworkRunner runner, NetworkInput input)
    {
        var data = new PlayerInput
        {
            direction = new Vector3(latestDir.x, 0f, latestDir.y),
            buttons = accumulatedButtons
        };
        
        input.Set(data);
        accumulatedButtons = default;
    }

    private void OnDisable()
    {
        joystickBackgroundView.OnInteractJoystick -= HandleInteractJoystick;

        if (runnerController != null)
            runnerController.InputPolling -= HandleInputPolling;
    }
}
