using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MultiplayerScenePresenter : SubManagerBase
{
    [Header("Definition Asset Address")]
    [SerializeField] private string loadAddress;

    [Header("Init UIs")]
    [SerializeField] private List<MultiplayStageElementView> elementViews;
    [SerializeField] private CreateSessionView createSessionView;
    
    private AsyncOperationHandle<MultiplayStageDataSO> loadHandle;
    
    public override async UniTask DoInit()
    {
        Debug.Log($"[MultiplayerSceneGameMode] DoInit");
        loadHandle = Addressables.LoadAssetAsync<MultiplayStageDataSO>(loadAddress);
        await loadHandle;
        
        InitUIs(loadHandle.Result.MultiplayDefinitions);
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
    }

    private void OnClickElementView(in MultiplayStageDefinition definition)
    {
        createSessionView.SetState(ECreateSessionViewState.MapSelected, definition);
    }

    private void OnClickCreateSessionView()
    {
        if (createSessionView.CurrentState != ECreateSessionViewState.MapSelected)
            return;
        
        
    }

    private void OnDestroy()
    {
        if (loadHandle.IsValid())
            loadHandle.Release();
        
        for (int i = 0; i < elementViews.Count; i++)
        {
            elementViews[i].ElementButton.onClick.RemoveAllListeners();
        }
    }
}
