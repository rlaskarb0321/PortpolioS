using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerNetworkedAnimatorController : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float dampTime = 0.1f;

    private Dictionary<EPlayerStateType, PlayerAnimationStrategyBase> strategies;

    [Networked] public PlayerNetworkedAnimatorData AnimatorData { get; set; }
    [Networked] private EPlayerStateType AnimatorState { get; set; }

    public void SetAnimatorState(EPlayerStateType state) => AnimatorState = state;

    public override void Render()
    {
        if (AnimatorState == EPlayerStateType.None)
            return;
        
        strategies[AnimatorState].RenderStrategy();
    }

    private void Awake()
    {
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