using Cysharp.Threading.Tasks;
using UnityEngine;

public class MultiplayerInGameMode : GameModeBase
{
    [Header("Managers")]
    [SerializeField] private VirtualJoystickBackgroundView joystickBackgroundView;
    
    private Transform[] playerRespawnPoints;

    public Transform[] PlayerRespawnPoints => playerRespawnPoints;
    public VirtualJoystickBackgroundView JoystickBackgroundView => joystickBackgroundView;

    public void SetRespawnPoints(Transform[] inPlayerRespawnPoints)
    {
        playerRespawnPoints = inPlayerRespawnPoints;
    }
    
    public override async UniTask OnSceneActivated()
    {
        Debug.Log($"[MultiplayerInGameMode] OnSceneActivated");

        // Addressable 로 UI-Image 에셋과 BGM, 3d Model 등등 불러오기를 SubManager 에서 구현
        foreach (var managers in SubManagers)
        {
            await managers.DoInit();
        }
    }

    public override void OnSceneCompletelyLoaded()
    {
        Debug.Log($"[MultiplayerInGameMode] OnSceneCompletelyLoaded");
    }
}
