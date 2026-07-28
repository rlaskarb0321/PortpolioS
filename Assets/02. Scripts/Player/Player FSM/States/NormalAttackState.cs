using Fusion;
using UnityEngine;

/// <summary>
/// 노멀 콤보 공격 상태.
///
/// 콤보 진행에 필요한 모든 판정은 CharacterCombatConfig 에 구워진 타이밍(초)과
/// NormalComboComponent 의 SwingStartTick 을 이용한 파생 계산으로 이뤄진다.
/// 애니메이션 이벤트에는 일절 의존하지 않는다 — Render 도메인이라 클라이언트마다 타이밍이 달라
/// 시뮬레이션 판정에 쓰면 결정론이 깨지기 때문.
///
/// 상태를 저장하지 않고 매번 계산하므로 CanExitState / OnUpdateState 의 호출 순서와 무관하게
/// 항상 같은 답이 나오고, 재시뮬레이션에서도 안전하다.
/// </summary>
public class NormalAttackState : PlayerStateBase
{
    public NormalAttackState(in PlayerFSMContext context) : base(context) { }

    public override EPlayerStateType StateType { get => EPlayerStateType.NormalAttack; }

    /// <summary>현재 콤보 타수. 1-based, 0 = 미공격.</summary>
    private int ComboIndex => NetworkedAnimatorController.AnimatorData.normalComboIndex;

    /// <summary>현재 타의 타이밍 데이터.</summary>
    private NormalComboStep CurrentStep => Config.GetComboStep(ComboIndex - 1);

    public override void OnEnterState()
    {
        StartSwing(1);
    }

    public override void OnUpdateState(in PlayerInput input, NetworkButtons pressed = default)
    {
        if (ComboIndex == 0)
            return;

        var step = CurrentStep;
        float elapsed = NormalCombo.Elapsed;

        // 입력창 안에서 공격 버튼을 누르면 다음 타를 예약한다.
        if (IsWindowOpen(step, elapsed) == true && pressed.IsSet(EPlayerButton.NormalAttack) == true)
            NormalCombo.QueuedNext = true;

        // 판정 시점 전이면 아직 스윙 중.
        if (elapsed < step.comboDecisionTime)
            return;

        if (CanChain(step) == true)
            StartSwing(ComboIndex + 1);
        else
            EndCombo();
    }

    public override bool CanEnterState()
    {
        return true;
    }

    public override bool CanExitState()
    {
        // 저장된 플래그가 아니라 파생 계산이라 호출 순서와 무관하다.
        if (ComboIndex == 0)
            return true;

        var step = CurrentStep;

        // 판정 시점 전에는 스윙을 끊을 수 없다.
        if (NormalCombo.Elapsed < step.comboDecisionTime)
            return false;

        // 이어갈 타가 남아 있으면 여기서 나가지 않고 다음 타로 넘어간다.
        return CanChain(step) == false;
    }

    public override void OnExitState()
    {
        // CanExitState 가 true 가 된 틱에는 OnUpdateState 가 실행되지 않고 곧바로 전이될 수 있다.
        // 그 경로에서도 애니메이터 파라미터가 남지 않도록 여기서 정리한다.
        EndCombo();
    }

    // ─── Private Methods ────────

    private void StartSwing(int index)
    {
        SetComboIndex(index);
        NormalCombo.BeginSwing();
    }

    private void EndCombo()
    {
        SetComboIndex(0);
        NormalCombo.QueuedNext = false;
    }

    /// <summary>다음 타로 이어갈 수 있는가. inputWindowStart 가 음수면 더 못 잇는 마지막 타.</summary>
    private bool CanChain(in NormalComboStep step)
    {
        return step.inputWindowStart >= 0f
            && NormalCombo.QueuedNext
            && ComboIndex < Config.NormalComboStepCount;
    }

    private bool IsWindowOpen(in NormalComboStep step, float elapsed)
    {
        if (step.inputWindowStart < 0f)
            return false;

        return step.inputWindowStart <= elapsed && elapsed < step.inputWindowEnd;
    }

    private void SetComboIndex(int index)
    {
        var data = NetworkedAnimatorController.AnimatorData;

        data.normalComboIndex = index;
        NetworkedAnimatorController.AnimatorData = data;
    }
}
