using UnityEngine;

public class DevelopSceneInitializer : MonoBehaviour
{
    [SerializeField] private VirtualJoystickPresenter joystickPresenter;

    public void InitDevelopScene()
    {
        joystickPresenter.gameObject.SetActive(true);
    }
}
