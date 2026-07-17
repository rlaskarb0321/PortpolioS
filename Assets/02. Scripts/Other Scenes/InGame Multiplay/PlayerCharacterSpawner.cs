using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerCharacterSpawner : SubManagerBase
{
    [SerializeField] private string playerAddressTemplate;
    [SerializeField] private string playableCharacterInfoDataAddress;

    private AsyncOperationHandle<PlayableCharacterInfoSO> infoHandle;
    
    public override async UniTask DoInit()
    {
        var gameMode = BootstrapSceneInstance.Instance.GetCurrentGameMode<MultiplayerInGameMode>();
        var firstSpawnPoints = gameMode.PlayerRespawnPoints;
        
        infoHandle = Addressables.LoadAssetAsync<PlayableCharacterInfoSO>(playableCharacterInfoDataAddress);
        await infoHandle.Task;

        string loadCharacterAddress = GetPlayerAddress(infoHandle.Result);
    }

    private string GetPlayerAddress(PlayableCharacterInfoSO infoSO)
    {
        return string.Empty;
        // return string.Format(playerAddressTemplate, );
    }
}
