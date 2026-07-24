using System;
using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

public class PlayerAnimatorNMA : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float dampTime = 0.1f;
    [SerializeField] private float maxSpeed = 5.0f;
    
    private NetworkMecanimAnimator networkMecanimAnimator;
    private KCC kcc;
    
    private readonly int hashSpeed = Animator.StringToHash("Speed");

    private void Awake()
    {
        kcc = GetComponent<KCC>();
        networkMecanimAnimator = GetComponent<NetworkMecanimAnimator>();
    }

    public void UpdateLocomotion()
    {
        var horizontal = kcc.Data.RealVelocity;
        horizontal.y = 0.0f;

        float speed = Mathf.Clamp01(horizontal.magnitude / maxSpeed);
        animator.SetFloat(hashSpeed, speed, dampTime, Runner.DeltaTime);
    }
}
