using UnityEngine;

public class NormalAttackStrategy : PlayerAnimationStrategyBase
{
    private ComboAttackableComponent comboAttackable;
    
    public NormalAttackStrategy(PlayerNetworkedAnimatorController inOwner, Animator inAnimator)
        : base(inOwner, inAnimator) { }

    public override EPlayerStateType StrategyType { get => EPlayerStateType.NormalAttack; }

    public override void RenderStrategy()
    {
        
    }
}