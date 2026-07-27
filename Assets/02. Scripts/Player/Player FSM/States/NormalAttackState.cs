using UnityEngine;

public class NormalAttackState : PlayerStateBase
{
    private ComboAttackableComponent comboAttackable;
    private int currentCombo;
    private int maxComboCount;
    
    public NormalAttackState(in PlayerFSMContext context) : base(context)
    {
        comboAttackable = context.NetworkedAnimatorController.GetComponent<ComboAttackableComponent>();
        maxComboCount = context.controller.Config.NormalAttackMaxCombo;

        comboAttackable.ComboInput += HandleComboInput;
    }

    public override EPlayerStateType StateType { get => EPlayerStateType.NormalAttack; }
    
    public override void OnEnterState()
    {
        var data = NetworkedAnimatorController.AnimatorData;
        
        data.normalComboCount = ++currentCombo % maxComboCount;
        NetworkedAnimatorController.AnimatorData = data;
        comboAttackable.InitComboInputState();
    }

    public override void OnUpdateState(in PlayerInput input)
    {
        if (comboAttackable.CanInput == EComboInputtableState.CannotInput)
            return;
        if (comboAttackable.CurrentInputState == EComboInputState.InputReceived)
            return;
        
        Debug.Log("Normal Attack Button Pressed");
        comboAttackable.SetComboReceived();
    }

    public override bool CanEnterState()
    {
        return true;
    }

    public override bool CanExitState()
    {
        return comboAttackable.IsComboEnd;
    }

    public override void OnExitState()
    {
        currentCombo = 0;
        
        var data = NetworkedAnimatorController.AnimatorData;

        data.normalComboCount = 0;
        NetworkedAnimatorController.AnimatorData = data;
    }

    private void HandleComboInput()
    {
        if (comboAttackable.CurrentInputState != EComboInputState.InputReceived)
        {
            comboAttackable.SetComboEnd();
            return;
        }
        
        var data = NetworkedAnimatorController.AnimatorData;
        
        data.normalComboCount = ++currentCombo % maxComboCount;
        NetworkedAnimatorController.AnimatorData = data;
        comboAttackable.InitComboInputState();
    }
}