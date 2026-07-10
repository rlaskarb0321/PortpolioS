using UnityEngine;
using UnityEngine.UI;

public class LobbyCharacterReadyView : MonoBehaviour
{
    [SerializeField] private Text readyStateText;
    
    public void Init(LobbyPlayerModel model)
    {
        string readyText = model.isReady ? "Ready !!" : "-";
        readyStateText.text = readyText;
    }
}
