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

    [Header("Ready State UIs")]
    [SerializeField] private Text readyStateText;
    [SerializeField] private Text cancelReadyStateText;

    [Header("All Ready State UIs")]
    [SerializeField] private Text allReadyText;
    [SerializeField] private Text notAllReadyText;
    
    private Button elementButton;
    private ECreateSessionViewState currentState;

    public Button ElementButton { get => elementButton; }
    public ECreateSessionViewState CurrentState { get => currentState; }
    public Button CancelSearchButton { get => cancelSearchButton; }

    public void SetState(ECreateSessionViewState state, in MultiplayStageDefinition definition = default)
    {
        currentState = state;
        allReadyText.gameObject.SetActive(false);
        notAllReadyText.gameObject.SetActive(false);
        
        switch (state)
        {
            case ECreateSessionViewState.None:
                searchingMatchGroup.gameObject.SetActive(false);
                selectedStageNameText.gameObject.SetActive(false);
                startMatchText.gameObject.SetActive(false);
                cancelReadyStateText.gameObject.SetActive(false);
                readyStateText.gameObject.SetActive(false);
                break;
            
            case ECreateSessionViewState.MapSelected:
                searchingMatchGroup.gameObject.SetActive(false);
                selectedStageNameText.gameObject.SetActive(true);
                selectedStageNameText.text = definition.sessionName;
                startMatchText.gameObject.SetActive(true);
                cancelReadyStateText.gameObject.SetActive(false);
                readyStateText.gameObject.SetActive(false);
                break;
            
            case ECreateSessionViewState.MatchMaking:
                searchingMatchGroup.gameObject.SetActive(true);
                startMatchText.gameObject.SetActive(false);
                cancelReadyStateText.gameObject.SetActive(false);
                readyStateText.gameObject.SetActive(false);
                break;
            
            case ECreateSessionViewState.Ready:
                searchingMatchGroup.gameObject.SetActive(false);
                startMatchText.gameObject.SetActive(false);
                
                cancelReadyStateText.gameObject.SetActive(true);
                readyStateText.gameObject.SetActive(false);
                break;
            
            case ECreateSessionViewState.UnReady:
                searchingMatchGroup.gameObject.SetActive(false);
                startMatchText.gameObject.SetActive(false);
                
                cancelReadyStateText.gameObject.SetActive(false);
                readyStateText.gameObject.SetActive(true);
                break;
            
            case ECreateSessionViewState.NotAllReady:
                searchingMatchGroup.gameObject.SetActive(false);
                startMatchText.gameObject.SetActive(false);
                cancelReadyStateText.gameObject.SetActive(false);
                readyStateText.gameObject.SetActive(false);
                allReadyText.gameObject.SetActive(false);
                notAllReadyText.gameObject.SetActive(true);
                break;
            
            case ECreateSessionViewState.AllReady:
                searchingMatchGroup.gameObject.SetActive(false);
                startMatchText.gameObject.SetActive(false);
                cancelReadyStateText.gameObject.SetActive(false);
                readyStateText.gameObject.SetActive(false);
                allReadyText.gameObject.SetActive(true);
                notAllReadyText.gameObject.SetActive(false);
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
    Ready,
    UnReady,
    AllReady,
    NotAllReady,
    Count
}