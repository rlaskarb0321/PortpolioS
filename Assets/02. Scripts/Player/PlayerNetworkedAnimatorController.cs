using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerNetworkedAnimatorController : NetworkBehaviour
{
    private Animator animator;
    private ComboAttackableComponent comboAttackableComponent;
    private Dictionary<EPlayerStateType, PlayerAnimationStrategyBase> strategies;

    [Header("Network Strategies")]
    [Networked] public EPlayerStateType AnimatorState { get; set; }
    [Networked] public PlayerNetworkedAnimatorData AnimatorData { get; set; }

    public void SetAnimatorState(EPlayerStateType state) => AnimatorState = state;

    public override void Render()
    {
        if (AnimatorState == EPlayerStateType.None)
            return;
        
        strategies[AnimatorState].RenderStrategy();
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        strategies = new Dictionary<EPlayerStateType, PlayerAnimationStrategyBase>();

        strategies.Add(EPlayerStateType.Idle, new LocomotionStrategy(this, animator));
        strategies.Add(EPlayerStateType.Move, new LocomotionStrategy(this, animator));
        strategies.Add(EPlayerStateType.NormalAttack, new NormalAttackStrategy(this, animator));
    }
}

[System.Serializable]
public struct PlayerNetworkedAnimatorData : INetworkStruct
{
    public float locomotionSpeed;
}