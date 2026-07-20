using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerCharacterSpawner : SubManagerBase
{
    [SerializeField] private string playerAddressTemplate;
    [SerializeField] private string playableCharacterInfoDataAddress;

    private AsyncOperationHandle<PlayableCharacterInfoSO> infoHandle;
    private AsyncOperationHandle<GameObject> playerModelHandle;
    
    public override async UniTask DoInit()
    {
        // Character Info Data SO
        infoHandle = Addressables.LoadAssetAsync<PlayableCharacterInfoSO>(playableCharacterInfoDataAddress);
        await infoHandle.Task;

        // Which model needs to be loaded
        string loadCharacterAddress = GetPlayerAddress(infoHandle.Result);
        
        playerModelHandle = Addressables.LoadAssetAsync<GameObject>(loadCharacterAddress);
        await playerModelHandle.Task;
        
        // Runner.Spawn 은 Server Only 임. Client 는 생성권한이 없음
        // var gameMode = BootstrapSceneInstance.Instance.GetCurrentGameMode<MultiplayerInGameMode>();
        // var firstSpawnPoints = gameMode.PlayerRespawnPoints;
        // int localIndex = BootstrapSceneInstance.Instance.NetworkRunner.LocalPlayer.PlayerId - 1;
        // Transform spawnPos = firstSpawnPoints[localIndex];
        //
        // BootstrapSceneInstance.Instance.NetworkRunner.Spawn(playerModelHandle.Result, spawnPos.position);
    }

    private string GetPlayerAddress(PlayableCharacterInfoSO infoSO)
    {
        var userDatas = BootstrapSceneInstance.Instance.RunnerController.SessionContext.UserData;
        int localIndex = BootstrapSceneInstance.Instance.NetworkRunner.LocalPlayer.PlayerId - 1;
        var userData = userDatas[localIndex];
        string result = string.Format
        (
            playerAddressTemplate,
            infoSO.Infos[userData.mainCharacterIndex].name,
            userData.mainCharacterSkinIndex.ToString()
        );
            
        Debug.Log(result);
        return result;
    }

    private void OnDestroy()
    {
        if (infoHandle.IsValid())
            infoHandle.Release();
        if (playerModelHandle.IsValid())
            playerModelHandle.Release();
    }
}
