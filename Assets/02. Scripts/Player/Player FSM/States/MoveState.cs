using UnityEngine;
using Fusion.Addons.KCC;

public class MoveState : PlayerStateBase
{
    public MoveState(in PlayerFSMContext context) : base(context) { }

    public override EPlayerStateType StateType { get => EPlayerStateType.Move; }

    public override void OnUpdateState(in PlayerInput input)
    {
        Kcc.SetLookRotation(Quaternion.LookRotation(input.direction));
        Kcc.SetInputDirection(input.direction);

        NetworkedAnimator.UpdateLocomotion();
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