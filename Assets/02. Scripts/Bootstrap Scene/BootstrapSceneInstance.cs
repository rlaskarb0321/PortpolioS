using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

public class BootstrapSceneInstance : MonoSingleton<BootstrapSceneInstance>
{
    [Space(10.0f)]
    [SerializeField] private NetworkRunner networkRunner;
    [SerializeField] private ESceneMode sceneMode = ESceneMode.None;
    
    private Dictionary<EBootstrapInstance, IBootstrapInstance> bootstrapInstances;
    
    public NetworkRunner NetworkRunner => networkRunner;
    public ESceneMode SceneMode => sceneMode;

    public void TrySetNetworkRunner()
    {
        if (networkRunner != null)
            return;
        
        GameObject runner = new GameObject("NetworkRunner");
        
        runner.AddComponent<NetworkRunner>();
        runner.AddComponent<NetworkRunnerController>();
        networkRunner = runner.GetComponent<NetworkRunner>();
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

public enum ESceneMode
{
    None,
    Menu,
    Loading,
    StageMode,
    MultiplayMode,
    Count
}

public interface IBootstrapInstance
{
    public void Start();
    public EBootstrapInstance GetInstanceType();
    public void AllocateToBootstrapInstance();
    public void Execute();
}