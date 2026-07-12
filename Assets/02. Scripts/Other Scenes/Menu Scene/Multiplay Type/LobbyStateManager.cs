using System;
using System.Collections.Generic;
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
        SubscribeOnPlayerJoined();
    }

    private void SubscribeOnPlayerJoined()
    {
        // 현재 코드의 문제점
        // Host만 LobbyPlayer 의 값을 수정할 수 있음. 뒤끝에서 입장한 유저의 닉네임대신 Host 걸 불러옴
        // 즉 LobbyPlayer 에 Test 1 만 담기고 복제됨.
        
        // 그래서 어쨌든 클라가 내 로컬 Nickname 을 RPC 로 보내야함
        
        if (HasStateAuthority == false)
            return;
        
        var controller = BootstrapSceneInstance.Instance.NetworkRunner.GetComponent<NetworkRunnerController>();

        controller.PlayerJoined -= OnPlayerJoined;
        controller.PlayerJoined += OnPlayerJoined;
        Debug.Log($"Player: {BootstrapSceneInstance.Instance.NetworkRunner.LocalPlayer.PlayerId}," +
                  $"Subscribe OnPlayerJoined");
    }

    private void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        int index = player.PlayerId - 1;
        LobbyPlayerRef lobbyPlayerRef = new LobbyPlayerRef();
        TheBackendUserInfoGetter infoGetter = new TheBackendUserInfoGetter();

        lobbyPlayerRef.nickName = infoGetter.GetUserNickName();
        lobbyPlayerRef.isReady = false;
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