using Fusion;
using UnityEngine;
using Fusion.Addons.KCC;

public abstract class PlayerStateBase
{
    private readonly PlayerFSMContext context;

    // ──── Properties ────────────
    public abstract EPlayerStateType StateType { get; }

    /// <summary>OnEnterState 가 확정한 애니메이션 스텝. "재생 중인 스텝" 개념이 없는 state(Idle, Move 등)는 override 하지 않는다.</summary>
    public virtual AnimationTimeline CurrentStep => default;
    protected PlayerFSMController Controller => context.controller;
    protected PlayerNetworkedAnimatorController NetworkedAnimatorController => context.NetworkedAnimatorController;
    protected KCC Kcc => context.kcc;
    protected ActionComponent Action => context.action;
    protected CharacterCombatConfig CombatConfig => Controller.CombatConfig;
    protected CharacterStatConfig StatConfig => Action.StatConfig;
    protected PlayerCurrentStatData CurrentStat => Action.CurrentStat;

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