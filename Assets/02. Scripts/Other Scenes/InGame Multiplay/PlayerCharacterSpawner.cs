using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayerCharacterSpawner : SubManagerBase
{
    public override UniTask DoInit()
    {
        var gameMode = BootstrapSceneInstance.Instance.GetCurrentGameMode<MultiplayerInGameMode>();
        var firstSpawnPoints = gameMode.PlayerRespawnPoints;
        throw new System.NotImplementedException();
    }
}
