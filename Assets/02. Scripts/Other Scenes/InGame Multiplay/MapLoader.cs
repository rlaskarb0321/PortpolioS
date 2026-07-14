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
        var selectedDefinition =
            BootstrapSceneInstance.Instance.
            GetBootstrapInstance<MultiplaySessionContext>
            (EBootstrapInstance.MultiplaySessionContext).
            GetSelectedDefinition();
        string loadMapAddress = loadAddressTemplate + selectedDefinition.multiplayMapTypeIndex;

        mapLoadHandle = Addressables.LoadAssetAsync<GameObject>(loadMapAddress);
        await mapLoadHandle.Task;
        
        GameObject loadedMap = mapLoadHandle.Result;
        Instantiate(loadedMap, Vector3.zero, Quaternion.identity, transform);
    }
}
