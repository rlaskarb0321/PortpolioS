using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

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

public class BootstrapSceneInstance : MonoSingleton<BootstrapSceneInstance>
{
    private Dictionary<EBootstrapInstance, IBootstrapInstance> bootstrapInstances;
    
    public IReadOnlyDictionary<EBootstrapInstance, IBootstrapInstance> BootstrapInstances => bootstrapInstances;

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