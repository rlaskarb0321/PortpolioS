using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class LobbyPlayerManagerView : MonoBehaviour
{
    [SerializeField] private List<LobbyCharacterSlotView> lobbyCharacterSlots;
    [SerializeField] private List<LobbyCharacterReadyView> lobbyCharacterReadyViews;
}
