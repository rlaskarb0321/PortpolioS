using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MultiplayerScenePresenter : SubManagerBase
{
    [SerializeField] private List<AssetLabelReference> definitionLabel;
    
    private AsyncOperationHandle<IList<MultiplayStageDataSO>> loadHandle;
    private MultiplayStageDataSO loadedObject;
    
    public override UniTask DoInit()
    {
        Debug.Log($"[MultiplayerSceneGameMode] DoInit");
        List<string> labelString = new List<string>();
        
        for (int i = 0; i < definitionLabel.Count; i++)
        {
            labelString.Add(definitionLabel[i].labelString);
        }

        loadHandle = Addressables.LoadAssetsAsync<MultiplayStageDataSO>
        (
            labelString,
            OnAssetEachLoaded,
            Addressables.MergeMode.Intersection
        );
        
        return UniTask.CompletedTask;
    }

    private void OnAssetEachLoaded(MultiplayStageDataSO obj)
    {
        if (obj == null)
            return;
        
        loadedObject = obj;
    }
}
