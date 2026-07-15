using Fusion;
using UnityEngine;

public class MultiplaySessionContext : NetworkBehaviour, IBootstrapInstance
{
    [Networked] private MultiplayStageDefinition SelectedDefinition { get; set; }
    [Networked] public ELoadingSceneType LoadingSceneType { get; set; }
    [Networked] public SceneRef SceneRef { get; set; }
    
    public override void Spawned()
    {
        base.Spawned();
        AllocateToBootstrapInstance();
    }

    public void UpdateSelectedDefinition(MultiplayStageDefinition inSelectedDefinition)
    {
        SelectedDefinition = inSelectedDefinition;
    }

    public MultiplayStageDefinition GetSelectedDefinition()
    {
        return SelectedDefinition;
    }
    
    public EBootstrapInstance GetInstanceType()
    {
        return EBootstrapInstance.MultiplaySessionContext;
    }

    public void AllocateToBootstrapInstance()
    {
        if (BootstrapSceneInstance.Instance == null)
        {
            Debug.LogError("[AppLaunchManager] Failed to assign to BootstrapSceneInstance");
            return;
        }
        
        BootstrapSceneInstance.Instance.AllocateToBootstrapInstance(GetInstanceType(), this);
    }
}