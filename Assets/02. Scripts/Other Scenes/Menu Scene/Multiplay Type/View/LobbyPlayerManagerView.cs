using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class LobbyPlayerManagerView : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private List<LobbyCharacterSlotView> lobbyCharacterSlots;
    [SerializeField] private List<LobbyCharacterReadyView> lobbyCharacterReadyViews;

    public void Render(NetworkArray<LobbyPlayerRef> lobbyPlayerRefs)
    {
        for (int i = 0; i < lobbyPlayerRefs.Length; i++)
        {
            if (lobbyPlayerRefs[i].isPlayerValid == false)
                continue;
            
            lobbyCharacterSlots[i].SetView(lobbyPlayerRefs[i]);
            lobbyCharacterReadyViews[i].SetView(lobbyPlayerRefs[i]);
        }
    }
}
