using UnityEngine;

public class NormalAttackState : PlayerStateBase
{
    public NormalAttackState(PlayerFSMController inController, PlayerAnimatorNMA inAnimator) 
        : base(inController, inAnimator) { }

    public override EPlayerStateType StateType { get => EPlayerStateType.NormalAttack; }
    
    public override void OnEnterState()
    {
        
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