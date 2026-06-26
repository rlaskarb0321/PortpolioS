using Cysharp.Threading.Tasks;
using UnityEngine;

public enum EGameModeType
{
    MenuScene,
    StageModeScene,
    MultiplayModeScene,
    Count
}

public abstract class GameModeInstanceBase : SceneSingleton<GameModeInstanceBase>
{
    [SerializeField] private EGameModeType gameMode = EGameModeType.MenuScene;
    
    public EGameModeType GameModeType { get => gameMode; }

    protected override void Awake()
    {
        base.Awake();
        BootstrapSceneInstance.Instance.TrySetGameModeInstance(this);
    }
}
