using System;
using Fusion;

public class LobbyStateManager : NetworkBehaviour
{
    public static event Action<NetworkArray<LobbyPlayerRef>> OnLobbyPlayerJoined;
    
    [Networked, Capacity(3), OnChangedRender(nameof(HandleLobbyPlayerChanged))]
    public NetworkArray<LobbyPlayerRef> LobbyPlayer => default;
    
    public override void Spawned()
    {
        base.Spawned();
        
        int index = BootstrapSceneInstance.Instance.NetworkRunner.LocalPlayer.PlayerId - 1;
        LobbyPlayerRef lobbyPlayerRef = new LobbyPlayerRef();
        TheBackendUserInfoGetter infoGetter = new TheBackendUserInfoGetter();
        
        lobbyPlayerRef.nickName = infoGetter.GetUserNickName();
        lobbyPlayerRef.isReady = LobbyPlayer[index].isReady;
        RPC_UpdateLobbyPlayerView(lobbyPlayerRef, index);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_UpdateLobbyPlayerView(LobbyPlayerRef lobbyPlayerRef, int index)
    {
        LobbyPlayer.Set(index, lobbyPlayerRef);
    }

    private void HandleLobbyPlayerChanged()
    {
        OnLobbyPlayerJoined?.Invoke(LobbyPlayer);
    }
}

[System.Serializable]
public struct LobbyPlayerRef : INetworkStruct
{
    public NetworkString<_16> nickName;
    public bool isReady;
}