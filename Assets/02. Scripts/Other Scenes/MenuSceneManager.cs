using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MenuSceneManager : MonoBehaviour, ISceneLoadCallback
{
    [Header("Tasks")]
    [SerializeField] private EMenuSceneActivateTask completedTask;

    [Header("UI Buttons")]
    [SerializeField] private Button stageModeButton;
    [SerializeField] private Button multiplayerModeButton;
    
    private void Awake()
    {
        var loadingSceneManager =
            BootstrapSceneInstance.Instance
            .GetBootstrapInstance<LoadingSceneManager>(EBootstrapInstance.LoadingSceneManager);

        loadingSceneManager.OnSceneActivated += OnSceneActivated;
        loadingSceneManager.OnCompleteLoad += OnSceneCompletlyLoaded;
    }

    public async UniTask OnSceneActivated()
    {
        Debug.Log($"[MenuSceneManager] OnSceneActivated");
        BootstrapSceneInstance.Instance.TrySetNetworkRunner();
        await UniTask.CompletedTask;

        completedTask |= EMenuSceneActivateTask.NetworkRunner;
        stageModeButton.onClick.RemoveAllListeners();
        stageModeButton.onClick.AddListener(OnClickStageModeButton);
        multiplayerModeButton.onClick.RemoveAllListeners();
        multiplayerModeButton.onClick.AddListener(OnClickMultiplayerModeButton);
    }

    public void OnSceneCompletlyLoaded()
    {
        Debug.Log($"[MenuSceneManager] OnSceneLoaded");
    }

    private void OnClickStageModeButton()
    {
        
    }

    private void OnClickMultiplayerModeButton()
    {
        
    }
}

[System.Flags]
public enum EMenuSceneActivateTask
{
    NetworkRunner = 1 << 0,
}