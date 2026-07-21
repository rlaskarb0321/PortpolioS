using System;
using Fusion;
using UnityEngine;

public class DevelopSceneManager : MonoSingleton<DevelopSceneManager>
{
    // [SerializeField] private NetworkRunner networkRunnerPrefab;
    //
    // private NetworkRunner networkRunnerInstance;
    //
    // public NetworkRunner NetworkRunnerInstance => networkRunnerInstance;
    //
    // private async void OnEnable()
    // {
    //     networkRunnerInstance = Instantiate(networkRunnerPrefab);
    //
    //     var result = await networkRunnerInstance.StartGame(new StartGameArgs()
    //     {
    //         GameMode = GameMode.AutoHostOrClient,
    //         SessionName = "Develop Only Scene",
    //         SceneManager = networkRunnerInstance.GetComponent<NetworkSceneManagerDefault>() 
    //                        ?? networkRunnerInstance.gameObject.AddComponent<NetworkSceneManagerDefault>(),
    //         PlayerCount = 3
    //     });
    //     
    //     if (result.Ok)
    //     {
    //         Debug.Log($"Successfully join session");
    //         return;
    //     }
    //     
    //     Debug.LogError($"Failed to join session: ({result.ShutdownReason})");
    // }
}
