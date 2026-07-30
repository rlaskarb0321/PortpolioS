using Fusion;
using UnityEngine;

public class DodgeState : PlayerStateBase
{
    public DodgeState(in PlayerFSMContext inContext) : base(in inContext) { }

    public override EPlayerStateType StateType { get => EPlayerStateType.Dodge; }

    public override void OnEnterState()
    {
        Debug.Log($"[DodgeState] OnEnterState");
    }

    public override void OnUpdateState(in PlayerInput input, NetworkButtons pressed = default)
    {
        Debug.Log($"[DodgeState] OnUpdateState");
    }

    public override void OnExitState()
    {
        Debug.Log($"[DodgeState] OnExitState");
    }

    public override bool CanEnterState()
    {
        return true;
    }

    public override bool CanExitState()
    {
        return true;
    }
}