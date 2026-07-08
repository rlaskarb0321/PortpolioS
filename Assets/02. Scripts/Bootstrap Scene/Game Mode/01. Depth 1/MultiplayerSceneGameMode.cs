using Cysharp.Threading.Tasks;
using UnityEngine;

public class MultiplayerSceneGameMode : GameModeBase
{
    public override async UniTask OnSceneActivated()
    {
        Debug.Log($"[MultiplayerSceneGameMode] OnSceneActivated");

        // Addressable 로 UI-Image 에셋과 BGM, 3d Model 등등 불러오기를 SubManager 에서 구현
        await UniTask.WhenAll(SubManagers.Select(m => m.DoInit()));
    }

    public override void OnSceneCompletelyLoaded()
    {
        Debug.Log($"[MultiplayerSceneGameMode] OnSceneCompletelyLoaded");
        
    }
}
