using System;
using System.Linq;
using BackEnd;
using Fusion;
using UnityEngine;

public class LobbyPlayer : NetworkBehaviour
{
    // ───── public event ────
    public static event Action<LobbyPlayerModel> OnLobbyModelChanged;

    // 로컬(내) LobbyPlayer 인스턴스.
    // SerializeField로 물린 "프리팹"이 아니라, 이 클라이언트가 소유(InputAuthority)한 "실제 스폰된 오브젝트".
    public static LobbyPlayer Local { get; private set; }

    // ───── Network Property ────
    [Networked, OnChangedRender(nameof(HandleLobbyModelChanged))]
    public LobbyPlayerModel LobbyModel { get; set; }
    
    public override void Spawned()
    {
        base.Spawned();
        if (HasStateAuthority == false)
            HandleLobbyModelChanged();

        if (HasStateAuthority)
        {
            var currentModel = LobbyModel;
            
            currentModel.occupiedIndex = Runner.ActivePlayers.Count() - 1;
            currentModel.isReady = LobbyModel.isReady;
            LobbyModel = currentModel;
        }

        if (HasInputAuthority)
        {
            Local = this;                       
            RPC_RequestUpdateModel(GetMyNickname(), LobbyModel.isReady);
        }
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (Local == this)
            Local = null;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_RequestUpdateModel(string nickname, bool isReady)
    {
        var currentModel = LobbyModel;
        
        currentModel.nickName = nickname;
        currentModel.occupiedIndex = LobbyModel.occupiedIndex;
        currentModel.isReady = isReady;
        LobbyModel = currentModel;
    }

    private void HandleLobbyModelChanged()
    {
        OnLobbyModelChanged?.Invoke(LobbyModel);
    }

    private string GetMyNickname()
    {
        var bro = Backend.BMember.GetUserInfo();
        if (bro.IsSuccess() == false)
        {
            Debug.LogError("Failed to get user info: " + bro);
            return string.Empty;
        }

        LitJson.JsonData userInfoJson = bro.GetReturnValuetoJSON()["row"];
        return userInfoJson["nickname"].ToString();
    }
}

public struct LobbyPlayerModel : INetworkStruct
{
    public NetworkString<_16> nickName;
    public int occupiedIndex;
    public bool isReady;
}