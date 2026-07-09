using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class LobbyPlayerManagerView : MonoBehaviour
{
    [SerializeField] private List<LobbyCharacterSlotView> lobbyCharacterSlots;

    public void ChangeJoinedPlayerView(LobbyPlayerModel lobbyPlayerModel)
    {
        lobbyCharacterSlots[lobbyPlayerModel.occupiedIndex].Init(lobbyPlayerModel);
    }
}
