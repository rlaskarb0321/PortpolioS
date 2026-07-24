using UnityEngine;

public class MoveState : PlayerStateBase
{
    public MoveState(PlayerFSMController inController, PlayerAnimatorNMA inAnimator) 
        : base(inController, inAnimator) { }

    public override EPlayerStateType StateType { get => EPlayerStateType.Move; }
    
    public override void OnEnterState()
    {
        
    }

    public override bool CanExitState()
    {
        return true;
    }
}