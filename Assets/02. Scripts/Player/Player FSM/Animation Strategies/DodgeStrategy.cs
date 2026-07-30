using UnityEngine;

public class DodgeStrategy : PlayerAnimationStrategyBase
{
    private readonly int hashIsRoll = Animator.StringToHash("isRoll");
    
    public DodgeStrategy(PlayerNetworkedAnimatorController inOwner, Animator inAnimator) : base(inOwner, inAnimator)
    {
    }

    public override EPlayerStateType StrategyType { get => EPlayerStateType.Dodge; }
    public override ERenderStrategyType RenderStrategyType { get => ERenderStrategyType.OneShot; }

    protected override void RenderOneShot()
    {
        Animator.SetBool(hashIsRoll, true);
    }

    public override void OnExitStrategy()
    {
        Animator.SetBool(hashIsRoll, false);
    }

    public override bool CanEnterStrategy()
    {
        return true;
    }
}