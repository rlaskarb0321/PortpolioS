using UnityEngine;
using UnityEngine.UI;

public class LobbyCharacterSlotView : MonoBehaviour
{
    [SerializeField] private Text nickNameText;
    
    public void SetView(LobbyPlayerRef lobbyPlayerRef)
    {
        nickNameText.text = lobbyPlayerRef.nickName.ToString();
    }
}
