using System.Runtime.CompilerServices;
using UnityEngine;

public class MoveState : PlayerStateBase
{
    public MoveState(in PlayerFSMContext context) : base(context) { }

    public override EPlayerStateType StateType { get => EPlayerStateType.Move; }

    public override void OnUpdateState(in PlayerInput input)
    {
        Kcc.SetLookRotation(Quaternion.LookRotation(input.direction));
        Kcc.SetInputDirection(input.direction);

        var horizontal = Kcc.Data.RealVelocity;
        horizontal.y = 0f;
        
        float normalized = Mathf.Clamp01(horizontal.magnitude / Controller.Config.MaxMoveSpeed);
        NetworkedAnimator.SetLocomotionSpeed(normalized);
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
