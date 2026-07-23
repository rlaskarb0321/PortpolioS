using UnityEngine;
using Fusion;

public enum EPlayerButton
{
    NormalAttack,
    Dodge,
    Expert,
    Ultimate,
    Interact,
    Count
}

public struct PlayerInput : INetworkInput
{
    public Vector3 direction;
    public NetworkButtons buttons;
}
