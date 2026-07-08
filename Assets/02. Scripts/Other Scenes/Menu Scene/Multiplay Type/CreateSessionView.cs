using System;
using UnityEngine;
using UnityEngine.UI;

public class CreateSessionView : MonoBehaviour
{
    [Header("Searching State UIs")]
    [SerializeField] private GameObject searchingMatchGroup;
    [SerializeField] private Button cancelSearchButton;
    
    [Header("Other State UIs")]
    [SerializeField] private Text selectedStageNameText;
    [SerializeField] private Text startMatchText;
    
    private Button elementButton;
    private ECreateSessionViewState currentState;

    public Button ElementButton { get => elementButton; }
    public ECreateSessionViewState CurrentState { get => currentState; }
    public Button CancelSearchButton { get => cancelSearchButton; }

    public void SetState(ECreateSessionViewState state, in MultiplayStageDefinition definition = default)
    {
        currentState = state;
        
        switch (state)
        {
            case ECreateSessionViewState.None:
                searchingMatchGroup.gameObject.SetActive(false);
                selectedStageNameText.gameObject.SetActive(false);
                startMatchText.gameObject.SetActive(false);
                break;
            
            case ECreateSessionViewState.MapSelected:
                searchingMatchGroup.gameObject.SetActive(false);
                selectedStageNameText.gameObject.SetActive(true);
                selectedStageNameText.text = definition.sessionName;
                startMatchText.gameObject.SetActive(true);
                break;
            
            case ECreateSessionViewState.MatchMaking:
                searchingMatchGroup.gameObject.SetActive(true);
                startMatchText.gameObject.SetActive(false);
                break;
        }
    }
    
    private void Awake()
    {
        elementButton = GetComponent<Button>();
    }
}

public enum ECreateSessionViewState
{
    None,
    MapSelected,
    MatchMaking,
    Count
}