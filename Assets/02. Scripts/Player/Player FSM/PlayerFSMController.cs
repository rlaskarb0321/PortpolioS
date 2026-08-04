using System.Collections.Generic;
using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;

public class PlayerFSMController : NetworkBehaviour
{
    private PlayerFSMContext context;
    private Dictionary<EPlayerStateType, PlayerStateBase> stateDict;
    private CharacterCombatConfig combatCombatConfig;

    public CharacterCombatConfig CombatConfig => combatCombatConfig;

#if UNITY_EDITOR
    [SerializeField] private EPlayerStateType currentState;
#endif

    // ─── Networked Properties ────────
    [Networked] private NetworkButtons PreviousButtons { get; set; }
    [Networked] public EPlayerStateType CurrentState { get; private set; }
    [Networked] public NetworkString<_32> CharacterName { get; private set; }

    public void SetCharacterName(string inName) => CharacterName = inName;

    public override void FixedUpdateNetwork()
    {
        // 로드 완료 여부로 시뮬레이션을 분기하지 않는다 — Config 는 스폰 이전에 프리로드된다.
        // 여기 남은 것은 "설정 오류로 영영 초기화되지 않은 경우"를 위한 상수 가드다.
        // (시간에 따라 값이 변하지 않으므로 재시뮬레이션 결과를 바꾸지 않는다.
        //  실제 실패는 CharacterConfigPreloader / Spawned 에서 이미 에러로 보고된다)
        if (stateDict == null)
            return;

        if (Runner.TryGetInputForPlayer(Object.InputAuthority, out PlayerInput input) == true)
        {
            var pressed = input.buttons.GetPressed(PreviousButtons);
            PreviousButtons = input.buttons;

            EPlayerStateType desired = ResolveDesiredState(input, pressed);
            ConvertState(desired);

            stateDict[CurrentState].OnUpdateState(input, pressed);
        }
    }

    /// <summary>
    /// CharacterName 은 onBeforeSpawned 에서 주입되므로 이 시점에 모든 피어에서 유효하다.
    /// Config 는 CharacterConfigPreloader 가 스폰 이전에 로드해 두었으므로 여기서는 동기 조회만 한다.
    /// </summary>
    public override void Spawned()
    {
        base.Spawned();

        combatCombatConfig = CharacterConfigRegistry.GetCombatConfig(CharacterName.ToString());
        if (combatCombatConfig == null)
            return;

        GetComponent<EnvironmentProcessor>().KinematicSpeed = combatCombatConfig.MaxMoveSpeed;

        var animator = GetComponent<PlayerNetworkedAnimatorController>();
        var kcc = GetComponent<KCC>();
        var action = GetComponent<ActionComponent>();

        context = new PlayerFSMContext(this, animator, kcc, action);
        stateDict = new Dictionary<EPlayerStateType, PlayerStateBase>();
        stateDict.Add(EPlayerStateType.Idle, new IdleState(context));
        stateDict.Add(EPlayerStateType.Move, new MoveState(context));
        stateDict.Add(EPlayerStateType.NormalAttack, new NormalAttackState(context));
        stateDict.Add(EPlayerStateType.Dodge, new DodgeState(context));
        stateDict.Add(EPlayerStateType.Expert, new ExpertState(context));
    }

    private void ConvertState(EPlayerStateType newState)
    {
        if (newState == CurrentState)
            return;
        if (CurrentState != EPlayerStateType.None && stateDict[CurrentState].CanExitState() == false)
            return;
        if (stateDict[newState].CanEnterState() == false)
            return;

        if (CurrentState != EPlayerStateType.None)
            stateDict[CurrentState].OnExitState();
        
        stateDict[newState].OnEnterState();
        CurrentState = newState;
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

#if UNITY_EDITOR
    private void Update()
    {
        currentState = CurrentState;
    }
#endif
}

public readonly struct PlayerFSMContext
{
    public readonly PlayerFSMController controller;
    public readonly PlayerNetworkedAnimatorController NetworkedAnimatorController;
    public readonly KCC kcc;
    public readonly ActionComponent action;

    public PlayerFSMContext(PlayerFSMController inController, PlayerNetworkedAnimatorController inNetworkedAnimatorController, KCC inKcc, ActionComponent inAction)
    {
        this.controller = inController;
        this.NetworkedAnimatorController = inNetworkedAnimatorController;
        this.kcc = inKcc;
        this.action = inAction;
    }
}
