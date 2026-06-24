using System.Threading;
using BackEnd;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class TheBackendLoginManager : AppLaunchModuleBase
{
    public override string ModuleName { get => "The Backend Login Manager"; }

    public override async UniTask ExecuteAsync(CancellationToken token = default)
    {
        var uniTask = new UniTaskCompletionSource();

        BackendLogin.Instance.CustomLogin("user1", "1234");
        await uniTask.Task;
    }
}
