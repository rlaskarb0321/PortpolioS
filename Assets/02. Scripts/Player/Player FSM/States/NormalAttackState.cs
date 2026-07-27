using UnityEngine;

public class NormalAttackState : PlayerStateBase
{
    private ComboAttackableComponent comboAttackable;
    
    public NormalAttackState(in PlayerFSMContext context) : base(context)
    {
        comboAttackable = context.NetworkedAnimatorController.GetComponent<ComboAttackableComponent>();
    }

    public override EPlayerStateType StateType { get => EPlayerStateType.NormalAttack; }
    
    public override void OnEnterState()
    {
        
    }

    public override void OnUpdateState(in PlayerInput input)
    {
        
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