using Cysharp.Threading.Tasks;
using UnityEngine;

public class MultiplayerSceneGameMode : GameModeBase
{
    public override async UniTask OnSceneActivated()
    {
        BootstrapSceneInstance.Instance.NetworkRunner.ProvideInput = true;
        await UniTask.CompletedTask;
    }

    public override void OnSceneCompletelyLoaded()
    {
        throw new System.NotImplementedException();
    }
}
