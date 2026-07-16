using System;
using UnityEngine;

public class GameMap : MonoBehaviour
{
    [SerializeField] private Transform[] playerSpawnPoints;

    private MultiplayerInGameMode gameMode; 
    
    private void Start()
    {
        gameMode = BootstrapSceneInstance.Instance.GetCurrentGameMode<MultiplayerInGameMode>();

        SetRespawnPoints(playerSpawnPoints);
    }

    public void SetRespawnPoints(Transform[] inPlayerSpawnPoints)
    {
        gameMode.SetRespawnPoints(inPlayerSpawnPoints);
    }
}
