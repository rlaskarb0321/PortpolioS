using Fusion;
using UnityEngine;

/// <summary>
/// 현재 진행 중인 액션(노멀 공격, 회피, 스킬 등)의 시뮬레이션 상태.
/// FSM 이 한 번에 하나의 액션만 돌리므로 액션별로 컴포넌트를 나누지 않고 여기 하나에 모은다.
/// </summary>
public class ActionComponent : NetworkBehaviour
{
    // ─── Networked Properties ────────
    [Networked] private int ActionStartTick { get; set; }
    [Networked] private NetworkBool QueuedNext { get; set; }

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
}
