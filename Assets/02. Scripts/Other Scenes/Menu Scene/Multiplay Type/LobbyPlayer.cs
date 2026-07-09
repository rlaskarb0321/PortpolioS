using System;
using System.Linq;
using BackEnd;
using Fusion;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour
{
    // ───── public event ────
    public static event Action<LobbyPlayerModel> OnLobbyModelChanged;
    
    // ───── Network Property ────
    [Networked, OnChangedRender(nameof(HandleLobbyModelChanged))]
    public LobbyPlayerModel LobbyModel { get; set; }

    private void HandleLobbyModelChanged()
    {
        OnLobbyModelChanged?.Invoke(LobbyModel);
    }
    
    public override void Spawned()
    {
        base.Spawned();
        Debug.Log("Spawned LobbyPlayer");

        if (HasStateAuthority == false)
            return;

        var bro = Backend.BMember.GetUserInfo();

        if(bro.IsSuccess() == false)
        {
            Debug.LogError("Failed to BRO: " + bro);
            return;
        }

        LitJson.JsonData userInfoJson = bro.GetReturnValuetoJSON()["row"];

        // struct 전체를 새로 만들어 통째로 대입해야 네트워크 동기화가 일어난다
        LobbyModel = new LobbyPlayerModel
        {
            nickName = userInfoJson["nickname"].ToString(),
            occupiedIndex = Runner.ActivePlayers.Count() - 1,
        };
    }
}

public struct LobbyPlayerModel : INetworkStruct
{
    public NetworkString<_16> nickName;
    public int occupiedIndex;
}