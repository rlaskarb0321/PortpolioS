using Cysharp.Threading.Tasks;
using UnityEngine;

public class StageTypeManager : GameTypeManagerBase
{
    public override async UniTask DoInit()
    {
        await UniTask.CompletedTask;
    }
}
