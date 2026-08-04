using System;
using Fusion;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

public class DevelopSceneManager : MonoBehaviour
{
    [Header("Initializer")]
    [SerializeField] private DevelopSceneInitializer initializer;

    [Header("Player")]
    [SerializeField] private Transform[] playerSpawnPoses;
    [SerializeField] private string playerModelAddress;
    [SerializeField] private string characterName;

    [Header("Character Config")]
    [SerializeField] private string combatConfigForm = "Data/Config/Combat/{0}";
    [SerializeField] private string statConfigForm = "Data/Config/Stat/{0}";

    private NetworkRunner runner;
    private NetworkRunnerController runnerController;
    private AsyncOperationHandle<GameObject> playerModelHandle;
    private AsyncOperationHandle<CharacterCombatConfig> combatConfigHandle;
    private AsyncOperationHandle<CharacterStatConfig> statConfigHandle;
    
    private void OnEnable()
    {
        Init();
    }

    private async void Init()
    {
        await UniTask.WaitUntil(() => BootstrapSceneInstance.Instance != null);

        BootstrapSceneInstance.Instance.CreateNetworkRunner();
        
        runner = BootstrapSceneInstance.Instance.NetworkRunner;
        runnerController = BootstrapSceneInstance.Instance.RunnerController;

        Destroy(GameObject.Find("Login Canvas"));

        await PreloadCharacterConfig();

        runnerController.PlayerJoined -= OnPlayerJoined;
        runnerController.PlayerJoined += OnPlayerJoined;
        
        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = "Develop Only Scene",
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>() 
                           ?? runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 3
        });

        if (result.Ok)
        {
            Debug.Log($"Successfully join session");
            initializer.InitDevelopScene();
            return;
        }
        
        Debug.LogError($"Failed to join session: ({result.ShutdownReason})");
    }

    /// <summary>
    /// 테스트 씬은 로비/세션(SessionContext)을 거치지 않으므로 CharacterConfigPreloader 를 쓸 수 없다.
    /// 인스펙터에 지정된 캐릭터 하나만 직접 로드해 레지스트리에 넣는다.
    ///
    /// PlayerJoined 는 StartGame 도중에 발화하므로, 이 로드는 반드시 StartGame 이전에 끝나야 한다.
    /// (스폰된 PlayerFSMController / ActionComponent 가 Spawned 에서 레지스트리를 동기 조회한다)
    /// </summary>
    private async UniTask PreloadCharacterConfig()
    {
        string combatAddress = string.Format(combatConfigForm, characterName);
        string statAddress = string.Format(statConfigForm, characterName);

        combatConfigHandle = Addressables.LoadAssetAsync<CharacterCombatConfig>(combatAddress);
        statConfigHandle = Addressables.LoadAssetAsync<CharacterStatConfig>(statAddress);

        await combatConfigHandle.Task;
        await statConfigHandle.Task;

        if (combatConfigHandle.Status != AsyncOperationStatus.Succeeded || combatConfigHandle.Result == null)
            Debug.LogError($"[DevelopSceneManager] CombatConfig 로드 실패: {combatAddress}");

        if (statConfigHandle.Status != AsyncOperationStatus.Succeeded || statConfigHandle.Result == null)
            Debug.LogError($"[DevelopSceneManager] StatConfig 로드 실패: {statAddress}");

        CharacterConfigRegistry.Register(characterName, combatConfigHandle.Result, statConfigHandle.Result);
    }

    private async void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        try
        {
            if (runner.IsServer == false)
                return;
            
            int index = runner.LocalPlayer.PlayerId - 1;
            Transform spawnPos = playerSpawnPoses[index];

            playerModelHandle = Addressables.LoadAssetAsync<GameObject>(playerModelAddress);
        
            await playerModelHandle.Task;
            // await characterSpawnFactory.SpawnPlayerCharacter
            // (
            //     playerModelHandle.Result,
            //     spawnPos.position,
            //     player
            // );
            await runner.SpawnAsync
            (
                playerModelHandle.Result,
                spawnPos.position,
                inputAuthority: player,
                onBeforeSpawned: (spawnedRunner, obj) =>
                {
                    var fsmController = obj.GetComponent<PlayerFSMController>();
                    if (fsmController != null)
                        fsmController.SetCharacterName(characterName);
                    else
                        Debug.LogError($"[DevelopSceneManager] PlayerFSMController를 찾을 수 없습니다: {obj.name}");
                }
            );
        }
        catch (Exception e)
        {
            Debug.LogError($"[OnPlayerJoined] Error: {e.Message}]");
        }
    }

    private void OnDestroy()
    {
        // Init 이 끝나기 전에 씬을 빠져나가면 runnerController 가 아직 null 이다.
        if (runnerController != null)
            runnerController.PlayerJoined -= OnPlayerJoined;

        // 레지스트리는 참조만 들고 있으므로 핸들 해제 전에 먼저 비운다.
        CharacterConfigRegistry.Clear();

        if (combatConfigHandle.IsValid())
            Addressables.Release(combatConfigHandle);
        if (statConfigHandle.IsValid())
            Addressables.Release(statConfigHandle);
    }
}
