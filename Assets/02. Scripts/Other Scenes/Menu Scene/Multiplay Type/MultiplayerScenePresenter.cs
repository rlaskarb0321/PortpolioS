using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MultiplayerScenePresenter : SubManagerBase
{
    [Header("LobbyStateManager")]
    [SerializeField] private LobbyStateManager lobbyStateManagerPrefab;
    
    [Header("Definition Asset Address")]
    [SerializeField] private string loadAddress;

    [Header("Init UIs")]
    [SerializeField] private List<MultiplayStageElementView> elementViews;
    [SerializeField] private CreateSessionView createSessionView;
    [SerializeField] private LobbyPlayerManagerView lobbyPlayerViewManager;
    
    private AsyncOperationHandle<MultiplayStageDataSO> loadHandle;
    private MultiplayStageDefinition selectedDefinition;
    private LobbyStateManager lobbyStateManager;
    private bool isMatched;
    
    public override async UniTask DoInit()
    {
        Debug.Log($"[MultiplayerSceneGameMode] DoInit");
        loadHandle = Addressables.LoadAssetAsync<MultiplayStageDataSO>(loadAddress);
        await loadHandle;
        
        InitUIs(loadHandle.Result.MultiplayDefinitions);
        LobbyStateManager.OnSpawned += OnLobbyStateManagerSpawned;
    }

    private void InitUIs(List<MultiplayStageDefinition> definitions)
    {
        if (definitions.Count != elementViews.Count)
        {
            Debug.LogError("[MultiplayerSceneGameMode] Invalid number of element views");
            return;
        }

        // Init Element Views
        for (int i = 0; i < elementViews.Count; i++)
        {
            MultiplayStageDefinition definition = definitions[i];
            
            elementViews[i].InitElementView(definition);
            elementViews[i].ElementButton.onClick.AddListener(() => OnClickElementView(definition));
        }
        
        // Init Create Session View
        createSessionView.SetState(ECreateSessionViewState.None);
        createSessionView.ElementButton.onClick.AddListener(OnClickCreateSessionView);
        createSessionView.CancelSearchButton.onClick.AddListener(OnClickCancelSearchingButton);
    }

    private void OnClickElementView(in MultiplayStageDefinition definition)
    {
        selectedDefinition = definition;
        createSessionView.SetState(ECreateSessionViewState.MapSelected, definition);
    }

    private void OnClickCreateSessionView()
    {
        if (isMatched == false)
        {
            if (createSessionView.CurrentState != ECreateSessionViewState.MapSelected)
                return;
            
            // Matching Session
            CreateSessionAsync().Forget();
            return;
        }

        if (lobbyStateManager == null)
        {
            Debug.LogError($"[MultiplayerScenePresenter] lobbyStateManager instance is Null");
            return;
        }

        int playerIndex = BootstrapSceneInstance.Instance.NetworkRunner.LocalPlayer.PlayerId - 1;
        
        switch (createSessionView.CurrentState)
        {
            case ECreateSessionViewState.UnReady:
                createSessionView.SetState(ECreateSessionViewState.Ready);
                lobbyStateManager.RPC_UpdatePlayerReadyState(playerIndex, true);
                break;
 
            case ECreateSessionViewState.Ready:
                createSessionView.SetState(ECreateSessionViewState.UnReady);
                lobbyStateManager.RPC_UpdatePlayerReadyState(playerIndex, false);
                break;
        }
    }

    private async UniTask CreateSessionAsync()
    {
        MatchMakingManager matchMakingManager = new MatchMakingManager();
        
        createSessionView.SetState(ECreateSessionViewState.MatchMaking);
        BootstrapSceneInstance.Instance.CreateNetworkRunner();
        isMatched = await matchMakingManager.JoinOrCreateSession(selectedDefinition.sessionName);

        if (isMatched == false)
        {
            createSessionView.SetState(ECreateSessionViewState.MapSelected, selectedDefinition);
            return;
        }

        createSessionView.SetState(ECreateSessionViewState.UnReady);
        if (BootstrapSceneInstance.Instance.NetworkRunner.IsServer)
        {
            BootstrapSceneInstance.Instance.NetworkRunner.Spawn(lobbyStateManagerPrefab);
        }
    }

    private void OnClickCancelSearchingButton()
    {
        if (createSessionView.CurrentState != ECreateSessionViewState.MatchMaking)
            return;
        
        createSessionView.SetState(ECreateSessionViewState.MapSelected, selectedDefinition);
    }

    private void OnLobbyStateManagerSpawned(LobbyStateManager inLobbyStateManager)
    {
        inLobbyStateManager.OnChangedLobbyPlayer += lobbyPlayerViewManager.OnChangedChangedLobbyPlayerView;
        lobbyStateManager = inLobbyStateManager;
    }

    private void OnDestroy()
    {
        if (loadHandle.IsValid())
            loadHandle.Release();
        
        for (int i = 0; i < elementViews.Count; i++)
        {
            elementViews[i].ElementButton.onClick.RemoveAllListeners();
        }
        
        createSessionView.ElementButton.onClick.RemoveAllListeners();
        lobbyStateManager.OnChangedLobbyPlayer -= lobbyPlayerViewManager.OnChangedChangedLobbyPlayerView;
        LobbyStateManager.OnSpawned -= OnLobbyStateManagerSpawned;
    }
}
