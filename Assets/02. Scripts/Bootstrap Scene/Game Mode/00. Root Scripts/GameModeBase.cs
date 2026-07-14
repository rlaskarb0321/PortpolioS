using Cysharp.Threading.Tasks;
using UnityEngine;

public enum EGameModeType
{
    MenuScene,
    StageModeScene,
    MultiplayLobbyScene,
    InGame_MultiplayScene,
    Count
}

public abstract class GameModeBase : MonoBehaviour
{
    [Header("Game Mode")]
    [SerializeField] private EGameModeType gameMode = EGameModeType.MenuScene;

    [Header("Sub Game Modes")]
    [SerializeField] private SubManagerBase[] subManagers;

    public EGameModeType GameModeType { get => gameMode; }
    protected SubManagerBase[] SubManagers { get => subManagers; }

    protected virtual void Awake()
    {
        BootstrapSceneInstance.Instance.SetGameModeInstance(this);
        var loadingSceneManager =
            BootstrapSceneInstance.Instance
            .GetBootstrapInstance<LoadingSceneManager>(EBootstrapInstance.LoadingSceneManager);

        loadingSceneManager.OnSceneActivated += OnSceneActivated;
        loadingSceneManager.OnCompleteLoad += OnSceneCompletelyLoaded;
    }

    /// <summary>
    /// 01. The point at which scene asset activation is complete
    /// </summary>
    public abstract UniTask OnSceneActivated();
    
    /// <summary>
    /// 02. When all tasks are finished after activating the scene
    /// </summary>
    public abstract void OnSceneCompletelyLoaded();
}
