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

        // ① [프록시(늦게 받은 사람)만] 이미 값이 채워진 채 도착한 오브젝트를 UI에 그려준다.
        //    늦게 접속한 사람이 "기존 플레이어들의 닉네임"을 보게 하는 핵심.
        //    (OnChangedRender는 이미 정해진 초기값엔 안 울리니, 여기서 직접 호출해 그려준다.)
        //    ★ 호스트(StateAuthority)는 제외: 모든 변경을 OnChangedRender로 실시간 감지하므로 불필요하고,
        //      오히려 아직 occupiedIndex가 0인(=②가 안 돈) 상태로 호출돼 0번 슬롯을 빈 값으로 덮어버린다.
        if (HasStateAuthority == false)
            HandleLobbyModelChanged();

        // ② [호스트(StateAuthority)만] 이 오브젝트가 쓸 슬롯 번호를 정한다.
        //    Networked 값을 실제로 쓸 수 있는 건 StateAuthority뿐이라 호스트가 담당.
        if (HasStateAuthority)
        {
            LobbyModel = new LobbyPlayerModel
            {
                nickName = LobbyModel.nickName,                  // 닉네임은 아직 비어 있을 수 있음 → ③의 RPC로 채워짐
                occupiedIndex = Runner.ActivePlayers.Count() - 1,
            };
        }

        // ③ [이 오브젝트의 주인(InputAuthority)만] 자기 계정 닉네임을 호스트에게 보고한다.
        //    클라는 Networked 값을 직접 못 쓰므로 RPC로 "부탁"한다. (호스트 자신도 여기 해당됨)
        if (HasInputAuthority)
        {
            RPC_ReportNickname(GetMyNickname());
        }
    }

    // 이 오브젝트 주인(InputAuthority)이 → 호스트(StateAuthority)에게 자기 닉네임을 보고.
    // [Rpc] 특성 덕분에 이 메서드 "본문"은 호스트에서만 실행된다.
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    private void RPC_ReportNickname(string nickname)
    {
        LobbyModel = new LobbyPlayerModel
        {
            nickName = nickname,
            occupiedIndex = LobbyModel.occupiedIndex,  // ②에서 호스트가 정한 슬롯 번호는 유지
        };
    }

    // 뒤끝(Backend)에서 "내 계정"의 닉네임을 읽어온다. (각 클라이언트가 자기 로컬 계정을 읽음)
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
}