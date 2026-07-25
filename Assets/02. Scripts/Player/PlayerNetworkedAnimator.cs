using Fusion;
using UnityEngine;

/// <summary>
/// 순수 렌더 레이어. State 가 정제해서 넘겨준 값만 [Networked] 로 들고,
/// Render() 에서 Animator 파라미터에 반영한다. KCC/input/Config 에 직접 접근하지 않는다.
/// </summary>
public class PlayerNetworkedAnimator : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float dampTime = 0.1f;

    [Networked] private float LocomotionSpeed { get; set; }

    private readonly int hashSpeed = Animator.StringToHash("Speed");

    // ─── State 가 정제한 값을 직접 수신 ────────
    public void SetLocomotionSpeed(float normalized) => LocomotionSpeed = normalized;

    public override void Render()
    {
        animator.SetFloat(hashSpeed, LocomotionSpeed, dampTime, Time.deltaTime);
    }
}
