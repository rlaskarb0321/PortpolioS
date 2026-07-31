using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class PlayerNetworkedAnimatorController : NetworkBehaviour
{
    private Animator animator;
    private PlayerFSMController controller;
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
        strategies.Add(EPlayerStateType.Dodge, new DodgeStrategy(this, animator));
        strategies.Add(EPlayerStateType.Expert, new ExpertStrategy(this, animator));
    }
}

/// <summary>
/// Render 가 소비하는 애니메이션 파라미터. 시뮬레이션이 쓰고 Render 가 읽는다.
/// 시뮬레이션 전용 상태(ActionStartTick, QueuedNext 등)는 여기 넣지 않는다 — ActionComponent 소관.
/// </summary>
[System.Serializable]
public struct PlayerNetworkedAnimatorData : INetworkStruct
{
    public float locomotionSpeed;           // 이동속도 관련 스피드
    public int normalComboIndex;            // 현재 노멀 콤보 타수. 1-based, 0 = 미공격
    public NetworkBool isRoll;              // 구르기 동작 여부
    public NetworkBool isEnhancedExpert;    // 강화된 특수 공격 여부
    public NetworkBool isNormalExpert;      // 일반 특수 공격 여부
}