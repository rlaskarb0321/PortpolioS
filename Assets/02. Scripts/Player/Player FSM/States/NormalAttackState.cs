using UnityEngine;

public class NormalAttackState : PlayerStateBase
{
    public NormalAttackState(in PlayerFSMContext context) : base(context) { }

    public override EPlayerStateType StateType { get => EPlayerStateType.NormalAttack; }
    
    public override void OnEnterState()
    {
        NetworkedAnimator.SetNormalAttackTrigger();
    }

    public override bool CanEnterState()
    {
        return true;
    }

    public override bool CanExitState()
    {
        return true;
    }

    public override void OnExitState()
    {
        base.OnExitState();
    }
}