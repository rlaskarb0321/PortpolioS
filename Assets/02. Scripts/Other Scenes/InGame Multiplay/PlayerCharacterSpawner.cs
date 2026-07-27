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

    private AsyncOperationHandle<PlayableCharacterInfoSO> infoHandle;
    private AsyncOperationHandle<GameObject> playerModelHandle;
    
    public override async UniTask DoInit()
    {
        if (BootstrapSceneInstance.Instance.IsServer() == false)
            return;
        
        // Character Info Data SO
        infoHandle = Addressables.LoadAssetAsync<PlayableCharacterInfoSO>(playableCharacterInfoDataAddress);
        await infoHandle.Task;

        var gameMode = BootstrapSceneInstance.Instance.GetCurrentGameMode<MultiplayerInGameMode>();
        var firstSpawnPoints = gameMode.PlayerRespawnPoints;
        // for (int i = 0; i < BootstrapSceneInstance.Instance.NetworkRunner.ActivePlayers.Count(); i++)
        // {
        //     // Which model needs to be loaded
        //     string loadCharacterAddress = GetPlayerAddress(infoHandle.Result, i);
        //     
        //     playerModelHandle = Addressables.LoadAssetAsync<GameObject>(loadCharacterAddress);
        //     await playerModelHandle.Task;
        //     
        //     Transform spawnPos = firstSpawnPoints[i];
        //
        //     // Spawn Player Character
        //     await BootstrapSceneInstance.Instance.NetworkRunner.SpawnAsync
        //     (
        //         playerModelHandle.Result,
        //         spawnPos.position,
        //         inputAuthority: BootstrapSceneInstance.Instance.NetworkRunner.ActivePlayers.ElementAt(i)
        //     );
        //
        //     // Set Character Combat Config
        //     var fsmController = playerModelHandle.Result.GetComponent<PlayerFSMController>();
        //     if (fsmController == null)
        //     {
        //         Debug.LogError($"[PlayerCharacterSpawner] PlayerFSMController not found");
        //         return;
        //     }
        //
        //     string characterCombatConfig = GetCombatConfigAddress(infoHandle.Result, i);
        //     
        //     combatConfigHandle = Addressables.LoadAssetAsync<CharacterCombatConfig>(characterCombatConfig);
        //     await combatConfigHandle.Task;
        //     
        //     fsmController.InitCharacterCombatConfig(combatConfigHandle.Result);
        // }
        
        for (int i = 0; i < BootstrapSceneInstance.Instance.NetworkRunner.ActivePlayers.Count(); i++)
        {
            // 1. 캐릭터 모델 주소 / 이름 가져오기 (config 주소는 이름으로 각 피어가 스스로 로드)
            string loadCharacterAddress = GetPlayerAddress(infoHandle.Result, i);
            string characterName = GetCharacterName(infoHandle.Result, i);

            // 2. 캐릭터 모델 에셋 비동기 로드
            playerModelHandle = Addressables.LoadAssetAsync<GameObject>(loadCharacterAddress);
            await playerModelHandle.Task;

            Transform spawnPos = firstSpawnPoints[i];
            PlayerRef playerRef = BootstrapSceneInstance.Instance.NetworkRunner.ActivePlayers.ElementAt(i);

            // 3. 스폰: onBeforeSpawned 에서 캐릭터 이름을 Networked 프로퍼티로 주입 → 모든 피어 복제
            BootstrapSceneInstance.Instance.NetworkRunner.Spawn
            (
                playerModelHandle.Result,
                spawnPos.position,
                spawnPos.rotation,
                inputAuthority: playerRef,
                onBeforeSpawned: (runner, obj) =>
                {
                    var fsmController = obj.GetComponent<PlayerFSMController>();
                    if (fsmController != null)
                        fsmController.SetCharacterName(characterName);
                    else
                        Debug.LogError($"[PlayerCharacterSpawner] PlayerFSMController를 찾을 수 없습니다: {obj.name}");
                }
            );
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

    private string GetCharacterName(PlayableCharacterInfoSO infoSO, int localIndex)
    {
        var userDatas = BootstrapSceneInstance.Instance.RunnerController.SessionContext.UserData;
        var userData = userDatas[localIndex];
        return infoSO.Infos[userData.mainCharacterIndex].name.ToString();
    }

    private void OnDestroy()
    {
        if (infoHandle.IsValid())
            infoHandle.Release();
        if (playerModelHandle.IsValid())
            playerModelHandle.Release();
    }
}
