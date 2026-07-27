using Fusion;
using UnityEngine;

public class PlayerNetworkedAnimator : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float dampTime = 0.1f;

    [Networked] private float LocomotionSpeed { get; set; }

    private readonly int hashSpeed = Animator.StringToHash("Speed");
    private readonly int hashNormalAttackTrigger = Animator.StringToHash("Normal Trigger");

    public void SetNormalAttackTrigger() => animator.SetTrigger(hashNormalAttackTrigger);
    
    public void SetLocomotionSpeed(float normalized) => LocomotionSpeed = normalized;

    public override void Render()
    {
        animator.SetFloat(hashSpeed, LocomotionSpeed, dampTime, Time.deltaTime);
    }
}
