using Fusion;
using UnityEngine;

public class NormalAttackState : PlayerStateBase
{
    public NormalAttackState(in PlayerFSMContext context) : base(context) { }

    public override EPlayerStateType StateType { get => EPlayerStateType.NormalAttack; }
    private int ComboIndex { get => NetworkedAnimatorController.AnimatorData.normalComboIndex; }
    private AnimationTimeline CurrentStep { get => Config.GetComboStep(ComboIndex - 1); }

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
        if (step.IsActive(EAnimMarker.ComboInput, elapsed) == true && pressed.IsSet(EPlayerButton.NormalAttack) == true)
            NormalCombo.QueuedNext = true;

        // 판정 시점 전이면 아직 스윙 중.
        if (step.HasPassed(EAnimMarker.ComboDecision, elapsed) == false)
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
        if (ComboIndex == 0)
            return true;

        var step = CurrentStep;

        if (step.HasPassed(EAnimMarker.ComboDecision, NormalCombo.Elapsed) == false)
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

    /// <summary>다음 타로 이어갈 수 있는가. ComboInput 마커가 없는 클립이면 더 못 잇는 마지막 타.</summary>
    private bool CanChain(in AnimationTimeline step)
    {
        return step.Has(EAnimMarker.ComboInput) == true
            && NormalCombo.QueuedNext
            && ComboIndex < Config.NormalComboStepCount;
    }

    private void SetComboIndex(int index)
    {
        var data = NetworkedAnimatorController.AnimatorData;

        data.normalComboIndex = index;
        NetworkedAnimatorController.AnimatorData = data;
    }
}
