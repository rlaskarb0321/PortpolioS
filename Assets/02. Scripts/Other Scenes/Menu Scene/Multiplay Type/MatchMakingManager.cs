using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

public class MatchMakingManager
{
    public async UniTask<bool> JoinOrCreateSession(string sessionName)
    {
        return await StartGame(GameMode.AutoHostOrClient, sessionName);
    }

    private async UniTask<bool> StartGame(GameMode mode, string sessionName)
    {
        NetworkRunner runner = BootstrapSceneInstance.Instance.NetworkRunner;
        if (runner == null)
        {
            runner = BootstrapSceneInstance.Instance.CreateNetworkRunner();
        }

        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = sessionName,
            SceneManager = runner.GetComponent<NetworkSceneManagerDefault>() 
                           ?? runner.gameObject.AddComponent<NetworkSceneManagerDefault>(),
            PlayerCount = 3
        });

        if (result.Ok)
        {
            Debug.Log($"Successfully join session: {sessionName}");
            return true;
        }
        
        Debug.LogError($"Failed to join session: {sessionName} ({result.ShutdownReason})");
        return false;
    }
}
