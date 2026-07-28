using UnityEngine;

public class NormalAttackStrategy : PlayerAnimationStrategyBase
{
    private readonly int hashNormalCombo = Animator.StringToHash("Normal Combo");

    private int lastComboCount = -1;

    public NormalAttackStrategy(PlayerNetworkedAnimatorController inOwner, Animator inAnimator)
        : base(inOwner, inAnimator) { }

    public override EPlayerStateType StrategyType { get => EPlayerStateType.NormalAttack; }
    public override ERenderStrategyType RenderStrategyType { get => ERenderStrategyType.OneShot; }

    protected override void RenderOneShot()
    {
        lastComboCount = Data.normalComboIndex;
        Animator.SetInteger(hashNormalCombo, Data.normalComboIndex);
        Debug.Log($"[NormalAttackStrategy] RenderStrategy combo={Data.normalComboIndex}");
    }

    public override void OnExitStrategy()
    {
        lastComboCount = -1;
        Animator.SetInteger(hashNormalCombo, 0);
    }

    public override bool CanEnterStrategy()
    {
        return lastComboCount != Data.normalComboIndex;
    }
}