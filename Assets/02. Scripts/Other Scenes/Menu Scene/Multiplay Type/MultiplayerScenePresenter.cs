using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
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
        // Before Match Making
        if (isMatched == false)
        {
            if (createSessionView.CurrentState != ECreateSessionViewState.MapSelected)
                return;
            
            // Matching Session
            CreateSessionAsync().Forget();
            return;
        }

        // After Match Making
        if (lobbyStateManager == null)
        {
            Debug.LogError($"[MultiplayerScenePresenter] lobbyStateManager instance is Null");
            return;
        }

        int playerIndex = BootstrapSceneInstance.Instance.NetworkRunner.LocalPlayer.PlayerId - 1;
        LobbyPlayerRef lobbyPlayerRef = lobbyStateManager.LobbyPlayer[playerIndex];

        if (lobbyPlayerRef.isHost == false)
        {
            switch (createSessionView.CurrentState)
            {
                case ECreateSessionViewState.UnReady:
                    lobbyPlayerRef.isReady = true;
                    createSessionView.SetState(ECreateSessionViewState.Ready);
                    break;
     
                case ECreateSessionViewState.Ready:
                    lobbyPlayerRef.isReady = false;
                    createSessionView.SetState(ECreateSessionViewState.UnReady);
                    break;
            }
            
            lobbyStateManager.RPC_UpdateLobbyPlayerView(lobbyPlayerRef, playerIndex);
        }
        else
        {
            switch (createSessionView.CurrentState)
            {
                case ECreateSessionViewState.AllReady:
                    // Debug.Log($"Enter Game !!");
                    // 여기서 Definition 을 통해 Map 과 BGM 과 몬스터 뭐 기타 등등 불러올 Addressable 의 정보가 담긴
                    // selectedDefinition 을 누군가에게 전달해줘야함
                    break;
                
                case ECreateSessionViewState.NotAllReady:
                    Debug.Log($"All Players must be ready");
                    break;
            }
        }
    }

    private async UniTask CreateSessionAsync()
    {
        MatchMakingManager matchMakingManager = new MatchMakingManager();
        
        createSessionView.SetState(ECreateSessionViewState.MatchMaking);
        BootstrapSceneInstance.Instance.CreateNetworkRunner();
        isMatched = await matchMakingManager.JoinOrCreateSession(selectedDefinition.sessionName);

        // Failed to Matching
        if (isMatched == false)
        {
            createSessionView.SetState(ECreateSessionViewState.MapSelected, selectedDefinition);
            return;
        }

        createSessionView.SetState(ECreateSessionViewState.UnReady);
        if (BootstrapSceneInstance.Instance.NetworkRunner.IsServer)
        {
            BootstrapSceneInstance.Instance.NetworkRunner.Spawn(lobbyStateManagerPrefab);
            createSessionView.SetState(ECreateSessionViewState.NotAllReady);
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
        lobbyStateManager = inLobbyStateManager;
        lobbyStateManager.OnChangedLobbyPlayer += OnChangedLobbyPlayer;
    }

    private void OnChangedLobbyPlayer(NetworkArray<LobbyPlayerRef> lobbyPlayerRefs)
    {
        lobbyPlayerViewManager.Render(lobbyPlayerRefs);

        if (BootstrapSceneInstance.Instance.NetworkRunner != null &&
            BootstrapSceneInstance.Instance.NetworkRunner.IsServer)
        {
            createSessionView.SetState(IsAllPlayerReady(lobbyPlayerRefs)
                ? ECreateSessionViewState.AllReady
                : ECreateSessionViewState.NotAllReady);
        }
    }

    private bool IsAllPlayerReady(NetworkArray<LobbyPlayerRef> players)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].isPlayerValid == false) continue;
            if (players[i].isReady == false) return false;
        }
        
        return true;
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
        if (lobbyStateManager != null)
            lobbyStateManager.OnChangedLobbyPlayer -= OnChangedLobbyPlayer;

        LobbyStateManager.OnSpawned -= OnLobbyStateManagerSpawned;
    }
}
