using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

public class PlayerFSMController : NetworkBehaviour
{
    private KCC kcc;
    private Dictionary<EPlayerStateType, PlayerStateBase> stateDict;
    private CharacterCombatConfig combatConfig;
    
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
    
    private void ConvertState(EPlayerStateType state)
    {
        if (stateDict[CurrentState].CanExitState() == false)
            return;
        
        stateDict[CurrentState].OnExitState();
        stateDict[state].OnEnterState();
        CurrentState = state;
    }

    private void Awake()
    {
        kcc = GetComponent<KCC>();
        var animator = GetComponent<PlayerAnimatorNMA>();

        stateDict = new Dictionary<EPlayerStateType, PlayerStateBase>();
        AddState(new IdleState(this, animator));
        AddState(new MoveState(this, animator));
        AddState(new NormalAttackState(this, animator));
    }

    private void AddState(PlayerStateBase state) => stateDict.Add(state.StateType, state);
}
