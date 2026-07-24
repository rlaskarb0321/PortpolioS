using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerCharacterSpawner : SubManagerBase
{
    [SerializeField] private string playerAddressTemplate;
    [SerializeField] private string playableCharacterInfoDataAddress;
    [SerializeField] private string combatConfigForm = "Data/Config/Combat/{0}";
    
    private AsyncOperationHandle<PlayableCharacterInfoSO> infoHandle;
    private AsyncOperationHandle<GameObject> playerModelHandle;
    private AsyncOperationHandle<CharacterCombatConfig> combatConfigHandle;
    
    public override async UniTask DoInit()
    {
        if (BootstrapSceneInstance.Instance.IsServer() == false)
            return;
        
        // Character Info Data SO
        infoHandle = Addressables.LoadAssetAsync<PlayableCharacterInfoSO>(playableCharacterInfoDataAddress);
        await infoHandle.Task;

        var gameMode = BootstrapSceneInstance.Instance.GetCurrentGameMode<MultiplayerInGameMode>();
        var firstSpawnPoints = gameMode.PlayerRespawnPoints;
        for (int i = 0; i < BootstrapSceneInstance.Instance.NetworkRunner.ActivePlayers.Count(); i++)
        {
            // Which model needs to be loaded
            string loadCharacterAddress = GetPlayerAddress(infoHandle.Result, i);
            
            playerModelHandle = Addressables.LoadAssetAsync<GameObject>(loadCharacterAddress);
            await playerModelHandle.Task;
            
            Transform spawnPos = firstSpawnPoints[i];

            // Spawn Player Character
            await BootstrapSceneInstance.Instance.NetworkRunner.SpawnAsync
            (
                playerModelHandle.Result,
                spawnPos.position,
                inputAuthority: BootstrapSceneInstance.Instance.NetworkRunner.ActivePlayers.ElementAt(i)
            );

            // Set Character Combat Config
            var fsmController = playerModelHandle.Result.GetComponent<PlayerFSMController>();
            if (fsmController == null)
            {
                Debug.LogError($"[PlayerCharacterSpawner] PlayerFSMController not found");
                return;
            }

            string characterCombatConfig = GetCombatConfigAddress(infoHandle.Result, i);
            
            combatConfigHandle = Addressables.LoadAssetAsync<CharacterCombatConfig>(characterCombatConfig);
            await combatConfigHandle.Task;
            
            fsmController.InitCharacterCombatConfig(combatConfigHandle.Result);
        }

        await UniTask.Yield();
    }

    private string GetPlayerAddress(PlayableCharacterInfoSO infoSO, int localIndex)
    {
        var userDatas = BootstrapSceneInstance.Instance.RunnerController.SessionContext.UserData;
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

    private string GetCombatConfigAddress(PlayableCharacterInfoSO infoSO, int localIndex)
    {
        var userDatas = BootstrapSceneInstance.Instance.RunnerController.SessionContext.UserData;
        var userData = userDatas[localIndex];
        string result = string.Format
        (
            combatConfigForm,
            infoSO.Infos[userData.mainCharacterIndex].name
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
