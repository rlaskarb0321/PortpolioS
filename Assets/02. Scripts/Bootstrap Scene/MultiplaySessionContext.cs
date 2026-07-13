using UnityEngine;

public class MultiplaySessionContext : MonoBehaviour, IBootstrapInstance
{
    private MultiplayStageDefinition selectedDefinition;
    
    public void UpdateSelectedDefinition(MultiplayStageDefinition inSelectedDefinition)
    {
        selectedDefinition = inSelectedDefinition;
    }

    public MultiplayStageDefinition GetSelectedDefinition()
    {
        return selectedDefinition;
    }
    
    public void Start()
    {
        AllocateToBootstrapInstance();
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