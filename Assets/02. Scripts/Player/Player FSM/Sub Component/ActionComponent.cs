using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// 현재 진행 중인 액션(노멀 공격, 회피, 스킬 등)의 시뮬레이션 상태.
/// FSM 이 한 번에 하나의 액션만 돌리므로 액션별로 컴포넌트를 나누지 않고 여기 하나에 모은다.
/// </summary>
public class ActionComponent : NetworkBehaviour
{
    [Header("Stat Config")]
    [SerializeField] private string statConfigForm = "Data/Config/Stat/{0}";

    private CharacterStatConfig statConfig;
    private AsyncOperationHandle<CharacterStatConfig> statConfigHandle;

    public CharacterStatConfig StatConfig => statConfig;

    // ─── Networked Properties ────────
    [Networked] private int ActionStartTick { get; set; }
    [Networked] private NetworkBool QueuedNext { get; set; }
    [Networked] public PlayerCurrentStatData CurrentStat { get; set; }

    public float Elapsed => ((int)Runner.Tick - ActionStartTick) * Runner.DeltaTime;

    public void Begin()
    {
        ActionStartTick = (int)Runner.Tick;
        QueuedNext      = false;
    }

    public bool IsNextQueued()
    {
        return QueuedNext;
    }

    public void SetNextQueued(bool queued)
    {
        QueuedNext = queued;
    }

    // ──── Private Methods ────────

    private async UniTaskVoid LoadStatConfig()
    {
        string address = string.Format(statConfigForm, GetComponent<PlayerFSMController>().CharacterName.ToString());

        statConfigHandle = Addressables.LoadAssetAsync<CharacterStatConfig>(address);
        await statConfigHandle.Task;

        if (statConfigHandle.Status != AsyncOperationStatus.Succeeded || statConfigHandle.Result == null)
        {
            Debug.LogError($"[ActionComponent] StatConfig 로드 실패: {address}");
            return;
        }

        statConfig = statConfigHandle.Result;
    }
    
    public override void Spawned()
    {
        base.Spawned();
        LoadStatConfig().Forget();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (statConfigHandle.IsValid())
            Addressables.Release(statConfigHandle);
    }
}

/// <summary>
/// 전투 중 변하는 캐릭터 현재값. 최대치·회복량 등 고정 튜닝값 SO(CharacterStatConfig)
/// 값을 기준으로 매 순간 바뀌는 실제 수치만 담는다.
/// </summary>
[System.Serializable]
public struct PlayerCurrentStatData : INetworkStruct
{
    public float currentHp;
    public float currentEnergy;
}
