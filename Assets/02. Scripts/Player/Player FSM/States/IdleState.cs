using UnityEngine;

public class IdleState : PlayerStateBase
{
    public IdleState(in PlayerFSMContext context) : base(context) { }

    public override EPlayerStateType StateType { get => EPlayerStateType.Idle; }
    
    public override void OnEnterState()
    {

    }

    public override void OnUpdateState(in PlayerInput input)
    {
        Animator.UpdateLocomotion();
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