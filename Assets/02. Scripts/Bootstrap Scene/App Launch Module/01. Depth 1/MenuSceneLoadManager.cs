using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class MenuSceneLoadManager : AppLaunchModuleBase
{
    public override string ModuleName { get => "Menu Scene Load Manager"; }
    
    public override async UniTask Execute(CancellationToken token = default)
    {
        await SceneManager.LoadSceneAsync("Menu Scene", LoadSceneMode.Additive)
            .ToUniTask(cancellationToken: token);
    }
}