using Fusion;
using UnityEngine;

/// <summary>
/// 현재 진행 중인 액션(노멀 공격, 회피, 스킬 등)의 시뮬레이션 상태.
/// FSM 이 한 번에 하나의 액션만 돌리므로 액션별로 컴포넌트를 나누지 않고 여기 하나에 모은다.
/// </summary>
public class ActionComponent : NetworkBehaviour
{
    private CharacterStatConfig statConfig;

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

    /// <summary>
    /// Config 는 CharacterConfigPreloader 가 스폰 이전에 로드해 두었으므로 동기 조회만 한다.
    /// 예전처럼 여기서 비동기 로드를 걸면 PlayerFSMController 의 로드와 완료 순서가
    /// 보장되지 않아, FSM 이 도는 동안 statConfig 가 아직 null 일 수 있었다.
    /// </summary>
    public override void Spawned()
    {
        base.Spawned();

        string characterName = GetComponent<PlayerFSMController>().CharacterName.ToString();
        statConfig = CharacterConfigRegistry.GetStatConfig(characterName);
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
