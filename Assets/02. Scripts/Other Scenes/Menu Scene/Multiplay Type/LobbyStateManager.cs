using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

public class LobbyStateManager : NetworkBehaviour
{
    public static event Action<NetworkArray<LobbyPlayerRef>> OnLobbyPlayerJoined;
    
    [Networked, Capacity(3), OnChangedRender(nameof(HandleLobbyPlayerChanged))]
    public NetworkArray<LobbyPlayerRef> LobbyPlayer => default;
    
    public override void Spawned()
    {
        base.Spawned();
        
        // 아래 코드는 Host 에서 1번 그리고 복제됐으니까 클라에서도 1번 호출됨
        // 그래서 그냥, RPC 를 Spawned 에서 하는 간단한 방법이 맞는듯?
        // Debug.Log($"Player: {BootstrapSceneInstance.Instance.NetworkRunner.LocalPlayer.PlayerId}," +
        //           $"Subscribe OnPlayerJoined");
        
        // SubscribeOnPlayerJoined();

        // RPC_UpdateLobbyPlayerView();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_UpdateLobbyPlayerView()
    {
        int index = BootstrapSceneInstance.Instance.NetworkRunner.LocalPlayer.PlayerId - 1;
        LobbyPlayerRef lobbyPlayerRef = new LobbyPlayerRef();
        TheBackendUserInfoGetter infoGetter = new TheBackendUserInfoGetter();

        lobbyPlayerRef.nickName = infoGetter.GetUserNickName();
        lobbyPlayerRef.isReady = false;
        LobbyPlayer.Set(index, lobbyPlayerRef);
    }

    private void SubscribeOnPlayerJoined()
    {
        // 현재 코드의 문제점
        // Host만 LobbyPlayer 의 값을 수정할 수 있음. GetUserInfo 하면 Host 걸 불러옴
        // 즉 LobbyPlayer 에 Test 1 만 담기고 복제됨.
        
        // 그래서 어쨌든 클라가 내 로컬 Nickname 을 RPC 로 보내야함
        
        if (HasStateAuthority == false)
            return;
        
        var controller = BootstrapSceneInstance.Instance.NetworkRunner.GetComponent<NetworkRunnerController>();

        controller.PlayerJoined -= OnPlayerJoined;
        controller.PlayerJoined += OnPlayerJoined;
    }

    private void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // int index = player.PlayerId - 1;
        // LobbyPlayerRef lobbyPlayerRef = new LobbyPlayerRef();
        // TheBackendUserInfoGetter infoGetter = new TheBackendUserInfoGetter();
        //
        // lobbyPlayerRef.nickName = infoGetter.GetUserNickName();
        // lobbyPlayerRef.isReady = false;
        // LobbyPlayer.Set(index, lobbyPlayerRef);
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