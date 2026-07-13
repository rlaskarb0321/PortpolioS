using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class LobbyPlayerManagerView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private List<LobbyCharacterSlotView> lobbyCharacterSlots;
    [SerializeField] private List<LobbyCharacterReadyView> lobbyCharacterReadyViews;
    [SerializeField] private CreateSessionView createSessionView;

    public void OnChangedChangedLobbyPlayerView(NetworkArray<LobbyPlayerRef> lobbyPlayerRefs)
    {
        bool isAllPlayerReady = true;
        for (int i = 0; i < lobbyPlayerRefs.Length; i++)
        {
            if (lobbyPlayerRefs[i].isPlayerValid == false)
                continue;
            
            isAllPlayerReady = lobbyPlayerRefs[i].isReady;
            lobbyCharacterSlots[i].SetView(lobbyPlayerRefs[i]);
            lobbyCharacterReadyViews[i].SetView(lobbyPlayerRefs[i]);
        }

        if (BootstrapSceneInstance.Instance.NetworkRunner != null &&
            BootstrapSceneInstance.Instance.NetworkRunner.IsServer)
        {
            ECreateSessionViewState allReadyState = isAllPlayerReady ? 
                ECreateSessionViewState.AllReady : ECreateSessionViewState.NotAllReady;
            
            createSessionView.SetState(allReadyState);
        }
    }
}
