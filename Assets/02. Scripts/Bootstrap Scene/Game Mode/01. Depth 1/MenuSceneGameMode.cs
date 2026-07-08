using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MenuSceneGameMode : GameModeBase
{
    [Header("Canvases")]
    [SerializeField] private Canvas menuSceneCanvas;
    [SerializeField] private Canvas stageTypeCanvas;

    [Header("UI Buttons")]
    [SerializeField] private Button stageTypeButton;
    [SerializeField] private Button multiplayerTypeButton;
    
    public override void OnSceneCompletelyLoaded()
    {
        // Init view(canvas)
        Debug.Log($"[MenuSceneManager] OnSceneLoaded");
        
        menuSceneCanvas.enabled = true;
        stageTypeCanvas.enabled = false;
    }

    public override async UniTask OnSceneActivated()
    {
        Debug.Log($"[MenuSceneManager] OnSceneActivated");
        
        // Init GameTypeManagers
        await UniTask.WhenAll(SubManagers.Select(m => m.DoInit()));

        // Allocate event method
        stageTypeButton.onClick.RemoveAllListeners();
        stageTypeButton.onClick.AddListener(OnClickStageModeButton);
        multiplayerTypeButton.onClick.RemoveAllListeners();
        multiplayerTypeButton.onClick.AddListener(OnClickMultiplayerModeButton);
        await UniTask.CompletedTask;
    }

    private void OnClickStageModeButton()
    {
        menuSceneCanvas.enabled = false;
        stageTypeCanvas.enabled = true;
    }

    private void OnClickMultiplayerModeButton()
    {
        var loadingSceneManager =
            BootstrapSceneInstance.Instance
            .GetBootstrapInstance<LoadingSceneManager>(EBootstrapInstance.LoadingSceneManager);

        loadingSceneManager.ActivateLoadingCanvas(ELoadingSceneType.MultiplayModeLoading).Forget();
    }
}