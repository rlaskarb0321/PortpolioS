using UnityEngine;

public class NormalAttackStrategy : PlayerAnimationStrategyBase
{
    private readonly int hashNormalCombo = Animator.StringToHash("Normal Combo");
    
    public NormalAttackStrategy(PlayerNetworkedAnimatorController inOwner, Animator inAnimator)
        : base(inOwner, inAnimator) { }

    public override EPlayerStateType StrategyType { get => EPlayerStateType.NormalAttack; }
    public override ERenderStrategyType RenderStrategyType { get => ERenderStrategyType.OneShot; }

    protected override void RenderOneShot()
    {
        Animator.SetInteger(hashNormalCombo, Data.normalComboCount);
        Debug.Log($"[NormalAttackStrategy] RenderStrategy");
    }

    public override bool CanEnterStrategy()
    {
        return base.CanEnterStrategy();
    }
}