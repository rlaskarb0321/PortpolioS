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
        bool initIsReady = HasStateAuthority ? true : false;
        
        lobbyPlayerRef.nickName = infoGetter.GetUserNickName();
        lobbyPlayerRef.isReady = initIsReady;
        lobbyPlayerRef.isPlayerValid = true;
        lobbyPlayerRef.isHost = HasStateAuthority;
        RPC_UpdateLobbyPlayerView(lobbyPlayerRef, index);
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_UpdateLobbyPlayerView(LobbyPlayerRef lobbyPlayerRef, int index)
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
    public bool isPlayerValid;
    public bool isReady;
    public bool isHost;
}