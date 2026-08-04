using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CharacterConfigPreloader : SubManagerBase
{
    [SerializeField] private string playableCharacterInfoDataAddress;
    [SerializeField] private string combatConfigForm = "Data/Config/Combat/{0}";
    [SerializeField] private string statConfigForm = "Data/Config/Stat/{0}";

    private readonly List<AsyncOperationHandle> handles = new List<AsyncOperationHandle>();

    public override async UniTask DoInit()
    {
        var infoHandle = Addressables.LoadAssetAsync<PlayableCharacterInfoSO>(playableCharacterInfoDataAddress);
        handles.Add(infoHandle);
        await infoHandle.Task;

        if (infoHandle.Status != AsyncOperationStatus.Succeeded || infoHandle.Result == null)
        {
            Debug.LogError($"[CharacterConfigPreloader] PlayableCharacterInfoSO 로드 실패: {playableCharacterInfoDataAddress}");
            return;
        }

        var runner = BootstrapSceneInstance.Instance.NetworkRunner;
        var userDatas = BootstrapSceneInstance.Instance.RunnerController.SessionContext.UserData;
        int playerCount = runner.ActivePlayers.Count();

        for (int i = 0; i < playerCount; ++i)
        {
            string characterName = infoHandle.Result.Infos[userDatas[i].mainCharacterIndex].name.ToString();
            await LoadConfigsFor(characterName);
        }

        Debug.Log($"[CharacterConfigPreloader] {CharacterConfigRegistry.Count}종 캐릭터 Config 프리로드 완료");
    }

    // ──── Private Methods ────────

    private async UniTask LoadConfigsFor(string characterName)
    {
        // 여러 플레이어가 같은 캐릭터를 고를 수 있다.
        if (CharacterConfigRegistry.Contains(characterName) == true)
            return;

        string combatAddress = string.Format(combatConfigForm, characterName);
        string statAddress = string.Format(statConfigForm, characterName);

        var combatHandle = Addressables.LoadAssetAsync<CharacterCombatConfig>(combatAddress);
        var statHandle = Addressables.LoadAssetAsync<CharacterStatConfig>(statAddress);
        handles.Add(combatHandle);
        handles.Add(statHandle);

        await combatHandle.Task;
        await statHandle.Task;

        if (combatHandle.Status != AsyncOperationStatus.Succeeded || combatHandle.Result == null)
            Debug.LogError($"[CharacterConfigPreloader] CombatConfig 로드 실패: {combatAddress}");

        if (statHandle.Status != AsyncOperationStatus.Succeeded || statHandle.Result == null)
            Debug.LogError($"[CharacterConfigPreloader] StatConfig 로드 실패: {statAddress}");

        CharacterConfigRegistry.Register(characterName, combatHandle.Result, statHandle.Result);
    }

    private void OnDestroy()
    {
        // 레지스트리는 참조만 들고 있으므로, 핸들 해제 전에 먼저 비운다.
        CharacterConfigRegistry.Clear();

        foreach (AsyncOperationHandle handle in handles)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }

        handles.Clear();
    }
}
