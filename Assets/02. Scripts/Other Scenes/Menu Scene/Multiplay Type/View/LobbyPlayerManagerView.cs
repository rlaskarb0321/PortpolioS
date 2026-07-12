using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class LobbyPlayerManagerView : MonoBehaviour
{
    [SerializeField] private List<LobbyCharacterSlotView> lobbyCharacterSlots;
    [SerializeField] private List<LobbyCharacterReadyView> lobbyCharacterReadyViews;

    public void OnChangedLobbyPlayerView(NetworkArray<LobbyPlayerRef> lobbyPlayerRefs)
    {
        for (int i = 0; i < lobbyPlayerRefs.Length; i++)
        {
            lobbyCharacterSlots[i].SetView(lobbyPlayerRefs[i]);
            lobbyCharacterReadyViews[i].SetView(lobbyPlayerRefs[i]);
        }
    }
}
