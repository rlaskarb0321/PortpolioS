using UnityEngine;

public class ExpertStrategy : PlayerAnimationStrategyBase
{
    public ExpertStrategy(PlayerNetworkedAnimatorController inOwner, Animator inAnimator) : base(inOwner, inAnimator)
    {
    }

    public override EPlayerStateType StrategyType { get => EPlayerStateType.Expert; }
    public override ERenderStrategyType RenderStrategyType { get => ERenderStrategyType.OneShot; }
}