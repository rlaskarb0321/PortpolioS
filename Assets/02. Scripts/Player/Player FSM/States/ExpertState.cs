using Fusion;
using UnityEngine;

public class ExpertState : PlayerStateBase
{
    public ExpertState(in PlayerFSMContext inContext) : base(in inContext) { }

    public override EPlayerStateType StateType { get => EPlayerStateType.Expert; }

    public override AnimationTimeline CurrentStep => CombatConfig.GetStep
    (
        NetworkedAnimatorController.AnimatorData.isEnhancedExpert == true ? EAnimStepKey.ExpertEnhanced : EAnimStepKey.ExpertNormal
    );

    public override void OnEnterState()
    {
        Debug.Log($"[ExpertState] OnEnterState");
        Action.Begin();
        SetExpertState();
    }

    public override void OnUpdateState(in PlayerInput input, NetworkButtons pressed = default)
    {
        var step = CurrentStep;
        float elapsed = Action.Elapsed;

        if (step.HasPassed(EAnimMarker.Trigger, elapsed) == false)
            return;
        
        // 이제 여기서 추상화 호출이 또 들어가야함. 캐릭터별로 Normal/Enhance Expert 가 다 다르니까!
    }

    public override bool CanEnterState()
    {
        if (NetworkedAnimatorController.AnimatorData.isNormalExpert ||
            NetworkedAnimatorController.AnimatorData.isEnhancedExpert)
            return false;
        
        return true;
    }

    public override bool CanExitState()
    {
        bool finished = Action.Elapsed >= CurrentStep.EffectiveClipLength;
        Debug.Log($"[DodgeState] CanExitState {finished}");

        return finished; 
    }
    
    // ─── Private Methods ─────────

    private void SetExpertState()
    {
        var data = NetworkedAnimatorController.AnimatorData;
        float reduce = StatConfig.EnhancedSkillEnergyCost;
        float currentEnergy = CurrentStat.currentEnergy;

        data.isEnhancedExpert = currentEnergy >= reduce;
        data.isNormalExpert = !(currentEnergy >= reduce);
        NetworkedAnimatorController.AnimatorData = data;
    }
}