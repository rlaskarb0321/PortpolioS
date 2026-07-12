using UnityEngine;
using UnityEngine.UI;

public class LobbyCharacterReadyView : MonoBehaviour
{
    [SerializeField] private Text readyStateText;

    public void SetView(LobbyPlayerRef lobbyPlayerRef)
    {
        string readyText = lobbyPlayerRef.isReady ? "Ready" : "Not Ready";
        
        readyStateText.text = readyText;
    }
}
