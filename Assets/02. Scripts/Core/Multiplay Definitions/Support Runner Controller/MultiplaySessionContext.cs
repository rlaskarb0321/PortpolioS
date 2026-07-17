using Fusion;
using UnityEngine;

public class MultiplaySessionContext : NetworkBehaviour
{
    // ──── Multiplay Setting ────
    [Networked] private MultiplayStageDefinition SelectedDefinition { get; set; }
    [Networked] public ELoadingSceneType LoadingSceneType { get; set; }
    [Networked] public SceneRef SceneRef { get; set; }
    
    // ─── User Setting ───
    [Networked] public UserData UserData { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        
        RegisterToRunnerController();
    }

    public void UpdateSelectedDefinition(MultiplayStageDefinition inSelectedDefinition)
    {
        SelectedDefinition = inSelectedDefinition;
    }

    public MultiplayStageDefinition GetSelectedDefinition()
    {
        return SelectedDefinition;
    }

    private void RegisterToRunnerController()
    {
        var runnerController = Runner.GetComponent<NetworkRunnerController>();
        if (runnerController == null)
        {
            Debug.LogError("[MultiplaySessionContext] NetworkRunnerController not found on the runner");
            return;
        }

        runnerController.SessionContext = this;
    }
}