using UnityEngine;

public class IdleState : PlayerStateBase
{
    public IdleState(in PlayerFSMContext context) : base(context) { }

    public override EPlayerStateType StateType { get => EPlayerStateType.Idle; }

    public override void OnEnterState()
    {
        Kcc.SetInputDirection(Vector3.zero);
    }

    public override void OnUpdateState(in PlayerInput input)
    {
        NetworkedAnimator.SetLocomotionSpeed(0f);
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
