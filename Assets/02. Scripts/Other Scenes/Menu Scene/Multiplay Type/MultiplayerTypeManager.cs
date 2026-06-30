using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerTypeManager : GameTypeManagerBase
{
    [SerializeField] private Button multiplayerButton;
    
    public override async UniTask DoInit()
    {
        multiplayerButton.onClick.AddListener(OnClickMultiplayerButton);
        await UniTask.CompletedTask;
    }

    private void OnClickMultiplayerButton()
    {
        Debug.Log("Clicked");
        var loadingSceneManager =
            BootstrapSceneInstance.Instance
            .GetBootstrapInstance<LoadingSceneManager>(EBootstrapInstance.LoadingSceneManager);

        loadingSceneManager.ActivateLoadingCanvas(ELoadingSceneType.MultiplayModeLoading).Forget();
    }
}
