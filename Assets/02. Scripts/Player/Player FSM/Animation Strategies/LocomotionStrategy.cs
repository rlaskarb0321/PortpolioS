using UnityEngine;

public class LocomotionStrategy : PlayerAnimationStrategyBase
{
    private readonly int hashSpeed = Animator.StringToHash("Speed");

    public LocomotionStrategy(PlayerNetworkedAnimatorController inOwner, Animator inAnimator)
        : base(inOwner, inAnimator) { }

    public override EPlayerStateType StrategyType { get => EPlayerStateType.Idle | EPlayerStateType.Move; }

    public override void RenderStrategy()
    {
        Animator.SetFloat(hashSpeed, Data.locomotionSpeed, 0.1f, Time.deltaTime);
    }
}