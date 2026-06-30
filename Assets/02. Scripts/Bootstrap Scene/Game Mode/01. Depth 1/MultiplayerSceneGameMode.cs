using Cysharp.Threading.Tasks;
using UnityEngine;

public class MultiplayerSceneGameMode : GameModeBase, ISceneLoadCallback
{
    protected override void Awake()
    {
        base.Awake();
    }

    public UniTask OnSceneActivated()
    {
        throw new System.NotImplementedException();
    }

    public void OnSceneCompletelyLoaded()
    {
        throw new System.NotImplementedException();
    }
}
