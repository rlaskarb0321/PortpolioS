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
    [Networked] public NetworkBool QueuedNext { get; set; }

    /// <summary>액션 시작 이후 경과 시간. 시간을 재지 않고 틱 차이로 계산해 리심에 안전하다.</summary>
    public float Elapsed => ((int)Runner.Tick - ActionStartTick) * Runner.DeltaTime;

    public void Begin()
    {
        ActionStartTick = (int)Runner.Tick;
        QueuedNext      = false;
    }
}
