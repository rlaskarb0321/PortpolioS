using Cysharp.Threading.Tasks;
using UnityEngine;

public class MultiplayerSceneGameMode : GameModeBase
{
    public override async UniTask OnSceneActivated()
    {
        Debug.Log($"[MultiplayerSceneGameMode] OnSceneActivated");
        
        // BootstrapSceneInstance.Instance.NetworkRunner.ProvideInput = true;
        await UniTask.WhenAll(GameTypeManagers.Select(m => m.DoInit()));
        
        // Addressable 로 UI-Image 에셋과 BGM, 3d Model 등등 불러오기
        
        await UniTask.CompletedTask;
    }

    public override void OnSceneCompletelyLoaded()
    {
        
    }
}
