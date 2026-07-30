using UnityEngine;

public class DodgeStrategy : PlayerAnimationStrategyBase
{
    private readonly int hashIsRoll = Animator.StringToHash("isRoll");

    private bool lastRoll = false;
    
    public DodgeStrategy(PlayerNetworkedAnimatorController inOwner, Animator inAnimator) : base(inOwner, inAnimator)
    {
    }

    public override EPlayerStateType StrategyType { get => EPlayerStateType.Dodge; }
    public override ERenderStrategyType RenderStrategyType { get => ERenderStrategyType.OneShot; }

    protected override void RenderOneShot()
    {
        // Debug.Log($"[DodgeStrategy] RenderOneShot");
        
        lastRoll = Data.isRoll;
        Animator.SetBool(hashIsRoll, true);
    }

    public override void OnExitStrategy()
    {
        // Debug.Log($"[DodgeStrategy] OnExitStrategy");
        
        lastRoll = false;
        Animator.SetBool(hashIsRoll, false);
    }

    public override bool CanEnterStrategy()
    {
        // Debug.Log($"[DodgeStrategy] CanEnterStrategy: {lastRoll != Data.isRoll}");
        
        return lastRoll != Data.isRoll;
    }
}