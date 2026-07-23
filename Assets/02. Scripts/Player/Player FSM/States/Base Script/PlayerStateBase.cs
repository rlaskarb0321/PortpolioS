using UnityEngine;

public abstract class PlayerStateBase
{
    // ──── Properties ────────────
    public abstract EPlayerStateType StateType { get; }

    // ──── Abstract Methods ────────────
    public abstract void OnEnterState();
    
    // ──── Virtual Methods ────────────
    public virtual void OnExitState() {}
}

public enum EPlayerStateType
{
    Idle,
    Move,
    NormalAttack,
    Expert,
    Ultimate,
    Interact,
    OnHit,
    Dead,
    Count
}