using Cysharp.Threading.Tasks;
using UnityEngine;

public class MapLoader : SubManagerBase
{
    [SerializeField] private string loadAddressTemplate;
    
    public override async UniTask DoInit()
    {
        var selectedDefinition =
            BootstrapSceneInstance.Instance.
            GetBootstrapInstance<MultiplaySessionContext>
            (EBootstrapInstance.MultiplaySessionContext).
            GetSelectedDefinition();
        string loadMapAddress = loadAddressTemplate + selectedDefinition.multiplayMapTypeIndex.ToString();
        
        await UniTask.CompletedTask;
    }
}
