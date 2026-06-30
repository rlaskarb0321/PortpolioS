using Cysharp.Threading.Tasks;
using UnityEngine;

public enum EGameModeType
{
    MenuScene,
    StageModeScene,
    MultiplayModeScene,
    Count
}

public abstract class GameModeBase : MonoSingleton<GameModeBase>
{
    [Header("Game Mode")]
    [SerializeField] private EGameModeType gameMode = EGameModeType.MenuScene;
    
    [Header("Game Type Managers")]
    [SerializeField] private GameTypeManagerBase[] gameTypeManagers;
    
    public EGameModeType GameModeType { get => gameMode; }
    protected GameTypeManagerBase[] GameTypeManagers { get => gameTypeManagers; }

    protected override void Awake()
    {
        base.Awake();
        BootstrapSceneInstance.Instance.SetGameModeInstance(this);
        var loadingSceneManager =
            BootstrapSceneInstance.Instance
            .GetBootstrapInstance<LoadingSceneManager>(EBootstrapInstance.LoadingSceneManager);

        loadingSceneManager.OnSceneActivated += OnSceneActivated;
        loadingSceneManager.OnCompleteLoad += OnSceneCompletelyLoaded;
    }

    /// <summary>
    /// The point at which scene asset activation is complete
    /// </summary>
    public abstract UniTask OnSceneActivated();
    
    /// <summary>
    /// When all tasks are finished after activating the scene
    /// </summary>
    public abstract void OnSceneCompletelyLoaded();
}
