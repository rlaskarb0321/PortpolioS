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
            {
                kcc.SetLookRotation(Quaternion.LookRotation(input.direction));
                kcc.SetInputDirection(input.direction);  
            }
            else
            {
                kcc.SetInputDirection(Vector3.zero);       
            }
        }
    }

    private void Awake()
    {
        kcc = GetComponent<KCC>();
    }
}
