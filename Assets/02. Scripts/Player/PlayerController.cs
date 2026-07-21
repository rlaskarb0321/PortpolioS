using System;
using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    private KCC kcc;
    
    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer(Object.InputAuthority, out PlayerInput input) == true)
        {
            if (input.direction.sqrMagnitude > Mathf.Epsilon)
                kcc.SetLookRotation(Quaternion.LookRotation(input.direction));
            
            Vector3 inputDirection = kcc.Data.TransformRotation * new Vector3(input.direction.x, 0.0f, input.direction.y);
            
            kcc.SetInputDirection(inputDirection);
        }
    }

    private void Awake()
    {
        kcc = GetComponent<KCC>();
    }
}
