using Fusion;
using UnityEngine;

public class DodgeState : PlayerStateBase
{
    public DodgeState(in PlayerFSMContext inContext) : base(in inContext) { }

    public override EPlayerStateType StateType { get => EPlayerStateType.Dodge; }

    public override void OnEnterState()
    {
        Debug.Log($"[DodgeState] OnEnterState");
        Action.Begin();
        SetRollData(true);
        Kcc.SetInputDirection(Vector3.zero);
    }

    public override void OnUpdateState(in PlayerInput input, NetworkButtons pressed = default)
    {
        AnimationTimeline animation = CombatConfig.GetDodgeComboStep();
        float t = Mathf.Clamp01(Action.Elapsed / animation.EffectiveClipLength);
        float speed = CombatConfig.DodgeSpeedCurve.Evaluate(t) * CombatConfig.DodgeSpeedMultiplier * animation.playbackSpeed;
        Vector3 direction = Kcc.Data.TransformDirection;

        Kcc.SetDynamicVelocity(direction * speed);
    }

    public override void OnExitState()
    {
        Debug.Log($"[DodgeState] OnExitState");
        Action.SetNextQueued(false);
        SetRollData(false);
    }

    public override bool CanEnterState()
    {
        Debug.Log($"[DodgeState] CanEnterState {NetworkedAnimatorController.AnimatorData.isRoll}");
        if (NetworkedAnimatorController.AnimatorData.isRoll)
            return false;
        
        return true;
    }

    public override bool CanExitState()
    {
        bool finished = Action.Elapsed >= CombatConfig.GetDodgeComboStep().EffectiveClipLength;
        Debug.Log($"[DodgeState] CanExitState {finished}");

        return finished;
    }
    
    // ──── Private Methods ────────

    private void SetRollData(bool isRoll)
    {
        var data = NetworkedAnimatorController.AnimatorData;

        data.isRoll = isRoll;
        NetworkedAnimatorController.AnimatorData = data;
    }
}