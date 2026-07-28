using Fusion;
using UnityEngine;

public class NormalComboComponent : NetworkBehaviour
{
    // ─── Networked Properties ────────
    [Networked] private int SwingStartTick { get; set; }
    [Networked] public NetworkBool QueuedNext { get; set; }

    public float Elapsed => ((int)Runner.Tick - SwingStartTick) * Runner.DeltaTime;

    public void BeginSwing()
    {
        SwingStartTick = (int)Runner.Tick;
        QueuedNext     = false;
    }

    // ─── Animation Events ────────
    // 콤보 타이밍의 오써링 소스이자 CharacterCombatConfig 베이크 마커.
    // 런타임 판정에는 관여하지 않는다 (Render 도메인이라 클라이언트마다 타이밍이 달라 비결정적).
    public void SetCanInput()      { }
    public void SetCannotInput()   { }
    public void HandleComboInput() { }
}
