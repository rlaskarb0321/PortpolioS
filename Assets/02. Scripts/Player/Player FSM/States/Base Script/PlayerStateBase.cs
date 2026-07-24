using UnityEngine;

public abstract class PlayerStateBase
{
    private PlayerFSMController controller;
    private PlayerAnimatorNMA animator;
    
    // ──── Properties ────────────
    public abstract EPlayerStateType StateType { get; }
    public PlayerFSMController Controller => controller;
    public PlayerAnimatorNMA Animator => animator;

    // ──── Constructor ────────────
    public PlayerStateBase(PlayerFSMController inController, PlayerAnimatorNMA inAnimator)
    {
        controller = inController;
        animator = inAnimator;
    }
    
    // ──── Abstract Methods ────────────
    public abstract void OnEnterState();

    public abstract bool CanEnterState();

    public abstract bool CanExitState();
    
    // ──── Virtual Methods ────────────
    public virtual void OnExitState() {}
    
    // ──── Normal Methods ────────────
    // protected void ConvertState(EPlayerStateType newState) => controller.ConvertState(newState);
    // protected void AddState(EPlayerStateType newState) => controller.AddState(newState);
    // protected void RemoveState(EPlayerStateType state) => controller.RemoveState(state);
}

[System.Flags]
public enum EPlayerStateType
{
    None            = 0,
    Idle            = 1 << 0,
    Move            = 1 << 1,
    NormalAttack    = 1 << 2,
    Dodge           = 1 << 3,
    Expert          = 1 << 4,
    Ultimate        = 1 << 5,
    Interact        = 1 << 6,
    OnHit           = 1 << 7,
    Dead            = 1 << 8,
}