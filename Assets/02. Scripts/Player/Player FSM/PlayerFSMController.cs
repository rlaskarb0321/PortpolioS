using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using Fusion.Addons.KCC;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PlayerFSMController : NetworkBehaviour
{
    [Header("Combat Config")]
    [SerializeField] private string combatConfigForm = "Data/Config/Combat/{0}";

    private PlayerFSMContext context;
    private Dictionary<EPlayerStateType, PlayerStateBase> stateDict;
    private CharacterCombatConfig combatConfig;
    private AsyncOperationHandle<CharacterCombatConfig> combatConfigHandle;

    public CharacterCombatConfig Config => combatConfig;

#if UNITY_EDITOR
    [SerializeField] private EPlayerStateType currentState;
#endif

    // ─── Networked Properties ────────
    [Networked] private NetworkButtons PreviousButtons { get; set; }
    [Networked] private EPlayerStateType CurrentState { get; set; }
    // config 에셋은 복제 불가 → 캐릭터 이름만 복제하고 각 피어가 스스로 로드.
    // 스폰 시 한 번만 세팅/전송되므로 지속 대역폭 비용은 사실상 0.
    [Networked] private NetworkString<_32> CharacterName { get; set; }

    // onBeforeSpawned(StateAuthority) 시점에 스포너가 호출 → 모든 피어로 복제됨
    public void SetCharacterName(string inName) => CharacterName = inName;

    public override void FixedUpdateNetwork()
    {
        if (combatConfig == null)
            return;

        if (Runner.TryGetInputForPlayer(Object.InputAuthority, out PlayerInput input) == true)
        {
            var pressed = input.buttons.GetPressed(PreviousButtons);
            PreviousButtons = input.buttons;

            EPlayerStateType desired = ResolveDesiredState(input, pressed);
            ConvertState(desired);

            stateDict[CurrentState].OnUpdateState(input);
        }
    }

    public override void Spawned()
    {
        base.Spawned();

        if (HasStateAuthority)
            ConvertState(EPlayerStateType.Idle);

        stateDict[EPlayerStateType.Idle].OnEnterState();

        // config 에셋은 복제 불가 → 각 피어가 프리팹에 박힌 정보로 스스로 로드
        LoadCombatConfig().Forget();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (combatConfigHandle.IsValid())
            Addressables.Release(combatConfigHandle);
    }

    private async UniTaskVoid LoadCombatConfig()
    {
        string address = string.Format(combatConfigForm, CharacterName.ToString());

        combatConfigHandle = Addressables.LoadAssetAsync<CharacterCombatConfig>(address);
        await combatConfigHandle.Task;

        if (combatConfigHandle.Status != AsyncOperationStatus.Succeeded || combatConfigHandle.Result == null)
        {
            Debug.LogError($"[PlayerFSMController] CombatConfig 로드 실패: {address}");
            return;
        }

        combatConfig = combatConfigHandle.Result;
        GetComponent<EnvironmentProcessor>().KinematicSpeed = combatConfig.MaxMoveSpeed;
    }

    public void ResetToIdleState()
    {
        ConvertState(EPlayerStateType.Idle);
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
        context.NetworkedAnimatorController.SetAnimatorState(CurrentState);
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

    private void Awake()
    {
        var animator = GetComponent<PlayerNetworkedAnimatorController>();
        var kcc = GetComponent<KCC>();

        context = new PlayerFSMContext(this, animator, kcc);
        stateDict = new Dictionary<EPlayerStateType, PlayerStateBase>();
        stateDict.Add(EPlayerStateType.Idle, new IdleState(context));
        stateDict.Add(EPlayerStateType.Move, new MoveState(context));
        stateDict.Add(EPlayerStateType.NormalAttack, new NormalAttackState(context));
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

    public PlayerFSMContext(PlayerFSMController inController, PlayerNetworkedAnimatorController inNetworkedAnimatorController, KCC inKcc)
    {
        this.controller = inController;
        this.NetworkedAnimatorController = inNetworkedAnimatorController;
        this.kcc = inKcc;
    }
}
