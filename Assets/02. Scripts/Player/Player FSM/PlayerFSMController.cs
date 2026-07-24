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
    
    // ─── Networked Properties ────────
    [Networked] private NetworkButtons PreviousButtons { get; set; }
    [Networked] private EPlayerStateType CurrentState { get; set; }

    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer(Object.InputAuthority, out PlayerInput input) == true)
        {
            if (input.direction.sqrMagnitude > Mathf.Epsilon)
            {
                kcc.SetLookRotation(Quaternion.LookRotation(input.direction));
                kcc.SetInputDirection(input.direction);  
            }
            else
            {
                kcc.SetInputDirection(Vector3.zero);       
            }

            var pressed = input.buttons.GetPressed(PreviousButtons);
            
            PreviousButtons = input.buttons;
            // if (pressed.IsSet(EPlayerButton.Dodge))    Dodge();
            // if (pressed.IsSet(EPlayerButton.Expert))   Expert();
            // if (pressed.IsSet(EPlayerButton.Ultimate)) Ultimate();
            // if (pressed.IsSet(EPlayerButton.Interact)) Interact();
        }
    }

    public override void Spawned()
    {
        base.Spawned();
        stateDict = new Dictionary<EPlayerStateType, PlayerStateBase>();
    }
    
    private void ConvertState(EPlayerStateType state)
    {
        stateDict[CurrentState].OnExitState();
        stateDict[state].OnEnterState();
        CurrentState = state;
    }

    private void Awake()
    {
        kcc = GetComponent<KCC>();
        foreach (EPlayerStateType flag in Enum.GetValues(typeof(EPlayerStateType)))
        {
            if (flag == EPlayerStateType.None)
                continue;

            // stateDict.Add(flag, );
        }
    }
}
