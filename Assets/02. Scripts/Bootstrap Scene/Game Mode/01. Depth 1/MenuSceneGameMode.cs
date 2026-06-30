using BackEnd;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MenuSceneGameMode : GameModeBase, ISceneLoadCallback
{
    [Header("Game Type Managers")]
    [SerializeField] private GameTypeManagerBase[] gameTypeManagers;
    
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

    public void OnSceneCompletelyLoaded()
    {
        Debug.Log($"[MenuSceneManager] OnSceneLoaded");
        // StartGoogleLogin();
    }

    public async UniTask OnSceneActivated()
    {
        Debug.Log($"[MenuSceneManager] OnSceneActivated");
        
        // Set Network Runner
        BootstrapSceneInstance.Instance.SetNetworkRunner();
        await UniTask.CompletedTask;

        // Init GameTypeManagers
        await UniTask.WhenAll(gameTypeManagers.Select(m => m.DoInit()));

        // Init view(canvas), allocate event method
        menuSceneCanvas.enabled = true;
        stageTypeCanvas.enabled = false;
        
        stageTypeButton.onClick.RemoveAllListeners();
        stageTypeButton.onClick.AddListener(OnClickStageModeButton);
        multiplayTypeButton.onClick.RemoveAllListeners();
        multiplayTypeButton.onClick.AddListener(OnClickMultiplayerModeButton);
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