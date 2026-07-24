using Fusion;
using UnityEngine;

public class PlayerNetworkedAnimator : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    
    private readonly int hashSpeed = Animator.StringToHash("Speed");

    public void UpdateLocomotion()
    {
        // var horizontal = kcc.Data.RealVelocity;
        // horizontal.y = 0.0f;
        //
        // float speed = Mathf.Clamp01(horizontal.magnitude / maxSpeed);
        // animator.SetFloat(hashSpeed, speed, dampTime, Runner.DeltaTime);
    }
}
