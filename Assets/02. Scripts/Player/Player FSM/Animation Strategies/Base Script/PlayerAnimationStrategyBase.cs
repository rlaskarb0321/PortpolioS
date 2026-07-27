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
    public abstract ERenderStrategyType RenderStrategyType { get; }

    public void RenderStrategy()
    {
        switch (RenderStrategyType)
        {
            case ERenderStrategyType.Continuous:
                RenderContinuous();
                break;
            
            case ERenderStrategyType.OneShot:
                RenderOneShot();
                break;
        }
    }

    public virtual bool CanEnterStrategy() { return true; }
    protected virtual void RenderContinuous() {}
    protected virtual void RenderOneShot() {}
}

public enum ERenderStrategyType
{
    Continuous,
    OneShot
}