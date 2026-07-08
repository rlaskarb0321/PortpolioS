using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class BootstrapSceneInstance : MonoSingleton<BootstrapSceneInstance>
{
    [Header("Network Runner")]
    [SerializeField] private NetworkRunner networkRunner;

    [Header("Game Mode Instance")]
    [SerializeField] private GameModeBase currentGameMode;

    private Dictionary<EBootstrapInstance, IBootstrapInstance> bootstrapInstances;
    
    public NetworkRunner NetworkRunner => networkRunner;

    public void SetNetworkRunner()
    {
        Instantiate(networkRunner);
        
        // if (networkRunner != null)
        //     return;
        //
        // GameObject runner = new GameObject("NetworkRunner");
        //
        // runner.AddComponent<NetworkRunner>();
        // runner.AddComponent<NetworkRunnerController>();
        // networkRunner = runner.GetComponent<NetworkRunner>();
    }

    public void SetGameModeInstance(GameModeBase gameMode)
    {
        if (gameMode == null)
        {
            Debug.LogError($"{gameMode.GetType().Name} does not exist");
            return;
        }
        
        currentGameMode = gameMode;
    }
    
    public T GetBootstrapInstance<T>(EBootstrapInstance type) where T : class, IBootstrapInstance
    {
        if (bootstrapInstances.TryGetValue(type, out var instance) == false)
        {
            Debug.LogError($"[BootstrapSceneInstance] {type} is not registered");
            return null;
        }

        if (instance is not T casted)
        {
            Debug.LogError($"[BootstrapSceneInstance] {type} cannot be cast to {typeof(T).Name}");
            return null;
        }

        return casted;
    }

    public T GetCurrentGameMode<T>() where T : class
    {
        if (currentGameMode == null)
        {
            Debug.LogError($"[BootstrapSceneInstance] GameMode is not set");
            return null;
        }

        if (currentGameMode is not T casted)
        {
            Debug.LogError($"[BootstrapSceneInstance] GameMode cannot be cast to {typeof(T).Name}");
            return null;
        }

        return casted;
    }

    public void AllocateToBootstrapInstance(EBootstrapInstance instanceType, IBootstrapInstance bootstrapInstance)
    {
        bootstrapInstances.Add(instanceType, bootstrapInstance);
    }
    
    protected override void Awake()
    {
        base.Awake();
        bootstrapInstances = new Dictionary<EBootstrapInstance, IBootstrapInstance>();
    }
}

public enum EBootstrapInstance
{
    AppLaunchManager,
    LoadingSceneManager,
    Count
}

public interface IBootstrapInstance
{
    public void Start();
    public EBootstrapInstance GetInstanceType();
    public void AllocateToBootstrapInstance();
    public void Execute();
}