using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MenuSceneGameModeManager : GameModeInstanceBase, ISceneLoadCallback
{
    [Header("Canvases")]
    [SerializeField] private Canvas menuSceneCanvas;
    [SerializeField] private Canvas stageModeCanvas;

    [Header("UI Buttons")]
    [SerializeField] private Button stageModeButton;
    [SerializeField] private Button multiplayerModeButton;
    
    protected override void Awake()
    {
        base.Awake();
        var loadingSceneManager =
            BootstrapSceneInstance.Instance
            .GetBootstrapInstance<LoadingSceneManager>(EBootstrapInstance.LoadingSceneManager);

        loadingSceneManager.OnSceneActivated += OnSceneActivated;
        loadingSceneManager.OnCompleteLoad += OnSceneCompletelyLoaded;
    }

    public async UniTask OnSceneActivated()
    {
        Debug.Log($"[MenuSceneManager] OnSceneActivated");
        BootstrapSceneInstance.Instance.TrySetNetworkRunner();
        await UniTask.CompletedTask;

        menuSceneCanvas.enabled = true;
        stageModeButton.interactable = false;
        
        stageModeButton.onClick.RemoveAllListeners();
        stageModeButton.onClick.AddListener(OnClickStageModeButton);
        multiplayerModeButton.onClick.RemoveAllListeners();
        multiplayerModeButton.onClick.AddListener(OnClickMultiplayerModeButton);
    }

    public void OnSceneCompletelyLoaded()
    {
        Debug.Log($"[MenuSceneManager] OnSceneLoaded");
    }

    private void OnClickStageModeButton()
    {
        menuSceneCanvas.enabled = false;
        stageModeButton.interactable = true;
    }

    private void OnClickMultiplayerModeButton()
    {
        
    }
}