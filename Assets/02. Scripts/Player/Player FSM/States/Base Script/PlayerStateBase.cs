using Fusion;
using UnityEngine;
using Fusion.Addons.KCC;

public abstract class PlayerStateBase
{
    private readonly PlayerFSMContext context;

    // ──── Properties ────────────
    public abstract EPlayerStateType StateType { get; }
    public PlayerFSMController Controller => context.controller;
    public PlayerNetworkedAnimatorController NetworkedAnimatorController => context.NetworkedAnimatorController;
    public KCC Kcc => context.kcc;
    public ActionComponent Action => context.action;
    public CharacterCombatConfig Config => Controller.Config;

    // ──── Constructor ────────────
    public PlayerStateBase(in PlayerFSMContext inContext)
    {
        context = inContext;
    }
    
    // ──── Abstract Methods ────────────

    public abstract bool CanEnterState();
    public abstract bool CanExitState();
    
    // ──── Virtual Methods ────────────
    public virtual void OnEnterState() {}
    public virtual void OnUpdateState(in PlayerInput input, NetworkButtons pressed = default) {}
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