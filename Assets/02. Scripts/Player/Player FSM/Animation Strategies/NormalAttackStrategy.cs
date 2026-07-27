using UnityEngine;

public class NormalAttackStrategy : PlayerAnimationStrategyBase
{
    public NormalAttackStrategy
    (
        PlayerNetworkedAnimatorController inOwner,
        Animator inAnimator,
        ComboAttackableComponent inComboAttackable
    ) : base(inOwner, inAnimator) { }

    public override EPlayerStateType StrategyType { get => EPlayerStateType.NormalAttack; }

    public override void RenderStrategy()
    {
    }
}