using Fusion;
using UnityEngine;

public class MultiplaySessionContext : NetworkBehaviour
{
    // ──── Multiplay Setting ────
    [Networked] private MultiplayStageDefinition SelectedDefinition { get; set; }
    [Networked] public ELoadingSceneType LoadingSceneType { get; set; }
    [Networked] public SceneRef SceneRef { get; set; }
    
    // ─── User Setting ───
    [Networked, Capacity(3)] public NetworkArray<UserData> UserData => default;

    public override void Spawned()
    {
        base.Spawned();

        SetUserDatas();
        RegisterToRunnerController();
    }

    private void SetUserDatas()
    {
        var userDataLoader = BootstrapSceneInstance.Instance
            .GetBootstrapInstance<TheBackendUserDataLoader>(EBootstrapInstance.TheBackendUserDataLoader);
        int index = Runner.LocalPlayer.PlayerId - 1;
        UserData userData = userDataLoader.UserData;

        RPC_UpdateUserDatas(userData, index);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_UpdateUserDatas(UserData userData, int index)
    {
        UserData.Set(index, userData);
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

    public void UpdateSelectedDefinition(MultiplayStageDefinition inSelectedDefinition)
    {
        SelectedDefinition = inSelectedDefinition;
    }

    public MultiplayStageDefinition GetSelectedDefinition()
    {
        return SelectedDefinition;
    }
}