using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MultiplayerScenePresenter : SubManagerBase
{
    [Header("Lobby Player")]
    [SerializeField] private LobbyPlayer lobbyPlayer;
    
    [Header("Definition Asset Address")]
    [SerializeField] private string loadAddress;

    [Header("Init UIs")]
    [SerializeField] private List<MultiplayStageElementView> elementViews;
    [SerializeField] private CreateSessionView createSessionView;
    [SerializeField] private LobbyPlayerManagerView lobbyPlayerViewManager;
    
    private AsyncOperationHandle<MultiplayStageDataSO> loadHandle;
    private MultiplayStageDefinition selectedDefinition;
    private MatchMakingManager matchMakingManager = new();
    
    public override async UniTask DoInit()
    {
        Debug.Log($"[MultiplayerSceneGameMode] DoInit");
        loadHandle = Addressables.LoadAssetAsync<MultiplayStageDataSO>(loadAddress);
        await loadHandle;
        
        InitUIs(loadHandle.Result.MultiplayDefinitions);
        LobbyPlayer.OnLobbyModelChanged += lobbyPlayerViewManager.ChangeJoinedPlayerView;
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
        if (createSessionView.CurrentState != ECreateSessionViewState.MapSelected)
            return;

        CreateSessionAsync().Forget();
    }

    private async UniTask CreateSessionAsync()
    {
        createSessionView.SetState(ECreateSessionViewState.MatchMaking);

        // Create Runner
        BootstrapSceneInstance.Instance.CreateNetworkRunner();
        
        // Subscribe Event Method
        var controller = BootstrapSceneInstance.Instance.RunnerController;
        
        controller.PlayerJoined -= CreateLobbyPlayer;
        controller.PlayerJoined += CreateLobbyPlayer;
        controller.PlayerLeft -= DestroyLobbyPlayer;
        controller.PlayerLeft += DestroyLobbyPlayer;
        
        // Match Making
        bool isSuccess = await matchMakingManager.JoinOrCreateSession(selectedDefinition.sessionName);
        
        if (isSuccess)
        {
        }
        else
        {
            createSessionView.SetState(ECreateSessionViewState.MapSelected, selectedDefinition);
        }
    }

    private void CreateLobbyPlayer(NetworkRunner runner, PlayerRef playerRef)
    {
        if (runner.IsServer)
        {
            runner.Spawn(lobbyPlayer, inputAuthority: playerRef);
        }
    }

    private void DestroyLobbyPlayer(NetworkRunner runner, PlayerRef playerRef)
    {
        
    }

    private void OnClickCancelSearchingButton()
    {
        if (createSessionView.CurrentState != ECreateSessionViewState.MatchMaking)
            return;
        
        createSessionView.SetState(ECreateSessionViewState.MapSelected, selectedDefinition);
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
    }
}
