using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
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
    public GameModeBase CurrentGameMode { get => currentGameMode; }

    public void TrySetNetworkRunner()
    {
        if (networkRunner != null)
            return;
        
        GameObject runner = new GameObject("NetworkRunner");
        
        runner.AddComponent<NetworkRunner>();
        runner.AddComponent<NetworkRunnerController>();
        networkRunner = runner.GetComponent<NetworkRunner>();
    }

    public void TrySetGameModeInstance(GameModeBase gameMode)
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
        return bootstrapInstances[type] as T;
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