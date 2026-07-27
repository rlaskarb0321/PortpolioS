using UnityEngine;

public abstract class PlayerAnimationStrategyBase
{
    private readonly PlayerNetworkedAnimatorController owner;
    private readonly Animator animator;

    protected PlayerNetworkedAnimatorData Data => owner.AnimatorData;
    protected Animator Animator => animator;

    protected PlayerAnimationStrategyBase(PlayerNetworkedAnimatorController inOwner, Animator inAnimator)
    {
        owner = inOwner;
        animator = inAnimator;
    }

    public abstract EPlayerStateType StrategyType { get; }

    public abstract void RenderStrategy();
}
