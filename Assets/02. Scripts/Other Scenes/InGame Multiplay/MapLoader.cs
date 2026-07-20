using System;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MapLoader : SubManagerBase
{
    [SerializeField] private string loadAddressTemplate;

    private AsyncOperationHandle<GameObject> mapLoadHandle;
    
    public override async UniTask DoInit()
    {
        Debug.Log($"[MapLoader] MultiplaySessionContext is Null: " +
                  $"{BootstrapSceneInstance.Instance.RunnerController.SessionContext == null}");
        
        var selectedDefinition =
            BootstrapSceneInstance.Instance.RunnerController.SessionContext
            .GetSelectedDefinition();
        string loadMapAddress = loadAddressTemplate + selectedDefinition.multiplayMapTypeIndex;

        mapLoadHandle = Addressables.LoadAssetAsync<GameObject>(loadMapAddress);
        await mapLoadHandle.Task;
        
        GameMap loadedMap = mapLoadHandle.Result.GetComponent<GameMap>();
        Instantiate(loadedMap, Vector3.zero, Quaternion.identity, transform);
    }

    private void OnDestroy()
    {
        if (mapLoadHandle.IsValid())
            mapLoadHandle.Release();
    }
}
