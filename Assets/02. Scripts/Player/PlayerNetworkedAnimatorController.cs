using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerNetworkedAnimatorController : NetworkBehaviour
{
    private Animator animator;
    private PlayerFSMController controller;
    private ComboAttackableComponent comboAttackableComponent;
    private Dictionary<EPlayerStateType, PlayerAnimationStrategyBase> strategies;
    private EPlayerStateType currentState;

    [Header("Network Strategies")]
    [Networked] public PlayerNetworkedAnimatorData AnimatorData { get; set; }

    public override void Render()
    {
        // 애니메이터 상태의 진실의 원천은 FSM의 CurrentState (SSOT) — 별도로 복제하지 않고 직접 읽는다.
        EPlayerStateType animatorState = controller.CurrentState;

        if (currentState != animatorState)
        {
            if (currentState != EPlayerStateType.None)
                strategies[currentState].OnExitStrategy();

            currentState = animatorState;
        }

        if (animatorState == EPlayerStateType.None)
            return;
        if (strategies[animatorState].CanEnterStrategy() == false)
            return;

        strategies[animatorState].RenderStrategy();
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<PlayerFSMController>();
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
    public int normalComboCount;
}