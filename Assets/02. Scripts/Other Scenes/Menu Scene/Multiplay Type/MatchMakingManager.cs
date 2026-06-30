using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

public class MatchMakingManager
{
    public async UniTask JoinOrCreateSession(string sessionName)
    {
        await StartGame(GameMode.AutoHostOrClient, sessionName);
    }

    private async UniTask StartGame(GameMode mode, string sessionName)
    {
        NetworkRunner runner = BootstrapSceneInstance.Instance.NetworkRunner;
        if (runner == null)
        {
            Debug.LogError("NetworkRunner not found");
            return;
        }
        
        var result = await runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = sessionName
        });

        if (result.Ok)
        {
            Debug.Log($"Successfully join session: {sessionName}");
        }
        else
        {
            Debug.LogError($"Failed to join session: {sessionName}");
        }
    }
}
