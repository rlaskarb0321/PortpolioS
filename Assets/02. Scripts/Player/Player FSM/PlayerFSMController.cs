using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

public class PlayerFSMController : NetworkBehaviour
{
    private PlayerFSMContext context;
    private Dictionary<EPlayerStateType, PlayerStateBase> stateDict;
    private CharacterCombatConfig combatConfig;

#if UNITY_EDITOR
    [SerializeField] private EPlayerStateType currentState;
#endif
    
    // ─── Networked Properties ────────
    [Networked] private NetworkButtons PreviousButtons { get; set; }
    [Networked] private EPlayerStateType CurrentState { get; set; }
    
    // ─── Properties ────────
    public CharacterCombatConfig CombatConfig => combatConfig;

    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer(Object.InputAuthority, out PlayerInput input) == true)
        {
            var pressed = input.buttons.GetPressed(PreviousButtons);
            PreviousButtons = input.buttons;

            EPlayerStateType desired = ResolveDesiredState(input, pressed);
            ConvertState(desired);

            stateDict[CurrentState].OnUpdateState(input);
        }
    }

    private EPlayerStateType ResolveDesiredState(in PlayerInput input, NetworkButtons pressed)
    {
        if (pressed.IsSet(EPlayerButton.Ultimate))     return EPlayerStateType.Ultimate;
        if (pressed.IsSet(EPlayerButton.Expert))       return EPlayerStateType.Expert;
        if (pressed.IsSet(EPlayerButton.Dodge))        return EPlayerStateType.Dodge;
        if (pressed.IsSet(EPlayerButton.NormalAttack)) return EPlayerStateType.NormalAttack;
        if (pressed.IsSet(EPlayerButton.Interact))     return EPlayerStateType.Interact;

        if (input.direction.sqrMagnitude > Mathf.Epsilon) return EPlayerStateType.Move;

        return EPlayerStateType.Idle;
    }

    public override void Spawned()
    {
        base.Spawned();

        if (HasStateAuthority)
            CurrentState = EPlayerStateType.Idle;

        stateDict[EPlayerStateType.Idle].OnEnterState();
    }

    public void InitCharacterCombatConfig(CharacterCombatConfig inConfig)
    {
        combatConfig = inConfig;
    }
    
    private void ConvertState(EPlayerStateType newState)
    {
        if (newState == CurrentState)
            return;
        if (stateDict[CurrentState].CanExitState() == false)
            return;
        if (stateDict[newState].CanEnterState() == false)
            return;
        
        stateDict[CurrentState].OnExitState();
        stateDict[newState].OnEnterState();
        CurrentState = newState;
    }

    private void Awake()
    {
        var animator = GetComponent<PlayerNetworkedAnimator>();
        var kcc = GetComponent<KCC>();

        context = new PlayerFSMContext(this, animator, kcc);
        stateDict = new Dictionary<EPlayerStateType, PlayerStateBase>();
        AddState(new IdleState(context));
        AddState(new MoveState(context));
        AddState(new NormalAttackState(context));
    }

    private void AddState(PlayerStateBase state) => stateDict.Add(state.StateType, state);

#if UNITY_EDITOR
    private void Update()
    {
        currentState = CurrentState;
    }
#endif
}

public readonly struct PlayerFSMContext
{
    public readonly PlayerFSMController controller;
    public readonly PlayerNetworkedAnimator NetworkedAnimator;
    public readonly KCC kcc;

    public PlayerFSMContext(PlayerFSMController inController, PlayerNetworkedAnimator inNetworkedAnimator, KCC inKcc)
    {
        this.controller = inController;
        this.NetworkedAnimator = inNetworkedAnimator;
        this.kcc = inKcc;
    }
}