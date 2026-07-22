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
    [SerializeField] private PlayerCharacterSpawnFactory characterSpawnFactory;
    [SerializeField] private Transform[] playerSpawnPoses;
    [SerializeField] private string playerModelAddress;

    private NetworkRunner runner;
    private NetworkRunnerController runnerController;
    private AsyncOperationHandle<GameObject> playerModelHandle;
    
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
            await characterSpawnFactory.SpawnPlayerCharacter
            (
                playerModelHandle.Result,
                spawnPos.position,
                player
            );
            // await runner.SpawnAsync
            // (
            //     playerModelHandle.Result,
            //     spawnPos.position,
            //     inputAuthority: player
            // );
        }
        catch (Exception e)
        {
            Debug.LogError($"[OnPlayerJoined] Error: {e.Message}]");
        }
    }

    private void OnDestroy()
    {
        runnerController.PlayerJoined -= OnPlayerJoined;
    }
}
