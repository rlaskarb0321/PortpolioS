using System;
using Fusion;

public class LobbyStateManager : NetworkBehaviour
{
    public static event Action<LobbyStateManager> OnSpawned;
    public event Action<NetworkArray<LobbyPlayerRef>> OnChangedLobbyPlayer;
    
    [Networked, Capacity(3), OnChangedRender(nameof(HandleLobbyPlayerChanged))]
    public NetworkArray<LobbyPlayerRef> LobbyPlayer => default;
    
    public override void Spawned()
    {
        base.Spawned();
        
        OnSpawned?.Invoke(this);
        
        int index = BootstrapSceneInstance.Instance.NetworkRunner.LocalPlayer.PlayerId - 1;
        LobbyPlayerRef lobbyPlayerRef = new LobbyPlayerRef();
        TheBackendUserInfoGetter infoGetter = new TheBackendUserInfoGetter();
        
        lobbyPlayerRef.nickName = infoGetter.GetUserNickName();
        lobbyPlayerRef.isReady = false;
        RPC_UpdateLobbyPlayerView(lobbyPlayerRef, index);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_UpdatePlayerReadyState(int inIndex, bool inIsReady)
    {
        LobbyPlayerRef lobbyPlayerRef = LobbyPlayer[inIndex];

        lobbyPlayerRef.isReady = inIsReady;
        LobbyPlayer.Set(inIndex, lobbyPlayerRef);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_UpdateLobbyPlayerView(LobbyPlayerRef lobbyPlayerRef, int index)
    {
        LobbyPlayer.Set(index, lobbyPlayerRef);
    }

    private void HandleLobbyPlayerChanged()
    {
        OnChangedLobbyPlayer?.Invoke(LobbyPlayer);
    }
}

[System.Serializable]
public struct LobbyPlayerRef : INetworkStruct
{
    public NetworkString<_16> nickName;
    public bool isReady;
}