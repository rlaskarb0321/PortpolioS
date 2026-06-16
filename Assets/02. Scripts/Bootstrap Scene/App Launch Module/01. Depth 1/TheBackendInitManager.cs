using System.Threading;
using UnityEngine;
using BackEnd;
using Cysharp.Threading.Tasks;

public class TheBackendInitManager : AppLaunchModuleBase
{
    public override string ModuleName { get => "The Backend Init Manager"; }
    
    public override async UniTask ExecuteAsync(CancellationToken token = default)
    {
        var uniTask = new UniTaskCompletionSource();

        Backend.InitializeAsync(bro =>
        {
            if (bro.IsSuccess())
            {
                uniTask.TrySetResult();
            }
            else
            {
                uniTask.TrySetException(new System.Exception("[AppLaunchModuleBase] Failed to Init Backend: " + bro));
            }
        });

        await uniTask.Task;
    }
}
