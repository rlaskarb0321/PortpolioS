using UnityEngine;
using UnityEngine.UI;

public class LobbyCharacterSlotView : MonoBehaviour
{
    [SerializeField] private Text nickNameText;
    
    public void Init(LobbyPlayerModel model)
    {
        nickNameText.text = model.nickName.ToString();
    }
}
