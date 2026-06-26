using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MenuSceneGameModeManager : GameModeBase, ISceneLoadCallback
{
    [Header("Canvases")]
    [SerializeField] private Canvas menuSceneCanvas;
    [SerializeField] private Canvas stageTypeCanvas;

    [Header("UI Buttons")]
    [SerializeField] private Button stageTypeButton;
    [SerializeField] private Button multiplayTypeButton;
    
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
        
        // Set Network Runner, allocate event method
        BootstrapSceneInstance.Instance.TrySetNetworkRunner();
        await UniTask.CompletedTask;

        menuSceneCanvas.enabled = true;
        stageTypeCanvas.enabled = false;
        
        stageTypeButton.onClick.RemoveAllListeners();
        stageTypeButton.onClick.AddListener(OnClickStageModeButton);
        multiplayTypeButton.onClick.RemoveAllListeners();
        multiplayTypeButton.onClick.AddListener(OnClickMultiplayerModeButton);
    }

    public void OnSceneCompletelyLoaded()
    {
        Debug.Log($"[MenuSceneManager] OnSceneLoaded");
    }

    private void OnClickStageModeButton()
    {
        menuSceneCanvas.enabled = false;
        stageTypeCanvas.enabled = true;
    }

    private void OnClickMultiplayerModeButton()
    {
        
    }
}