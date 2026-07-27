using UnityEngine;

public class NormalAttackStrategy : PlayerAnimationStrategyBase
{
    public NormalAttackStrategy(PlayerNetworkedAnimatorController inOwner, Animator inAnimator)
        : base(inOwner, inAnimator) { }

    public override EPlayerStateType StrategyType { get => EPlayerStateType.NormalAttack; }

    public override void RenderStrategy()
    {
    }
}