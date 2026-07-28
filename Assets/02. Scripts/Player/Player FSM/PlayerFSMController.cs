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
    private bool isInitialized;

    public CharacterCombatConfig Config => combatConfig;

#if UNITY_EDITOR
    [SerializeField] private EPlayerStateType currentState;
#endif

    // ─── Networked Properties ────────
    [Networked] private NetworkButtons PreviousButtons { get; set; }
    [Networked] private EPlayerStateType CurrentState { get; set; }
    [Networked] private NetworkString<_32> CharacterName { get; set; }

    public void SetCharacterName(string inName) => CharacterName = inName;

    public override void FixedUpdateNetwork()
    {
        if (isInitialized == false)
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

    public override void Spawned()
    {
        base.Spawned();
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
        
        var animator = GetComponent<PlayerNetworkedAnimatorController>();
        var kcc = GetComponent<KCC>();

        context = new PlayerFSMContext(this, animator, kcc);
        stateDict = new Dictionary<EPlayerStateType, PlayerStateBase>();
        stateDict.Add(EPlayerStateType.Idle, new IdleState(context));
        stateDict.Add(EPlayerStateType.Move, new MoveState(context));
        stateDict.Add(EPlayerStateType.NormalAttack, new NormalAttackState(context));
        context.NetworkedAnimatorController.SetAnimatorState(CurrentState);
        
        isInitialized = true;
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
