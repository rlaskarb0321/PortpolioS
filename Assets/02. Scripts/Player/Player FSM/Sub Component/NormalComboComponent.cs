using Fusion;
using UnityEngine;

/// <summary>
/// 노멀 콤보의 네트워크 시뮬레이션 상태를 보유한다.
///
/// 콤보 타이밍(입력창 개폐 / 분기 판정 시점)은 CharacterCombatConfig 의 NormalComboStep 에
/// 초 단위로 구워져 있고, 이 컴포넌트는 "언제 시작했는지" 와 "다음 타가 예약됐는지" 만 갖는다.
///
/// 경과 시간은 저장하지 않고 매번 틱 차이로 계산한다.
/// 누적(+=)이 아니라 파생이므로 재시뮬레이션에서 같은 틱이 여러 번 실행돼도 항상 같은 값이 나온다.
/// </summary>
public class NormalComboComponent : NetworkBehaviour
{
    // ─── Networked Properties ────────
    [Networked] public int         SwingStartTick { get; set; }
    [Networked] public NetworkBool QueuedNext     { get; set; }

    /// <summary>현재 스윙이 시작된 뒤 흐른 시간(초). 파생값이라 틱 안에서의 호출 순서와 무관하다.</summary>
    public float Elapsed => ((int)Runner.Tick - SwingStartTick) * Runner.DeltaTime;

    /// <summary>새 스윙을 시작한다. 원점을 현재 틱으로 잡고 예약을 비운다.</summary>
    public void BeginSwing()
    {
        SwingStartTick = (int)Runner.Tick;
        QueuedNext     = false;
    }

    // ─── Animation Events ────────
    // 콤보 타이밍의 오써링 소스이자 CharacterCombatConfig 베이크 마커.
    // 런타임 판정에는 관여하지 않는다 (Render 도메인이라 클라이언트마다 타이밍이 달라 비결정적).
    // 수신부가 없으면 Unity 가 재생마다 경고를 내므로 빈 메서드로 남긴다.
    // 추후 이펙트 / 사운드 훅을 붙일 자리.
    public void SetCanInput()      { }
    public void SetCannotInput()   { }
    public void HandleComboInput() { }
}
