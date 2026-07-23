using System;
using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

public class PlayerFSMController : NetworkBehaviour
{
    private KCC kcc;
    
    [Networked] private NetworkButtons PreviousButtons { get; set; }

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

    private void Awake()
    {
        kcc = GetComponent<KCC>();
    }
}
