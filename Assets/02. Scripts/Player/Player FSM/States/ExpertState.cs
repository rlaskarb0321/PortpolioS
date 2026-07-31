using UnityEngine;

public class ExpertState : PlayerStateBase
{
    public ExpertState(in PlayerFSMContext inContext) : base(in inContext) { }

    public override EPlayerStateType StateType { get => EPlayerStateType.Expert; }

    public override void OnEnterState()
    {
        Debug.Log($"[ExpertState] OnEnterState");
        Action.Begin();
    }

    public override bool CanEnterState()
    {
        throw new System.NotImplementedException();
    }

    public override bool CanExitState()
    {
        throw new System.NotImplementedException();
    }
}