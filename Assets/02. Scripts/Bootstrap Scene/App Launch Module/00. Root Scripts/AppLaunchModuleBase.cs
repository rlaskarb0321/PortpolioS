using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class AppLaunchModuleBase : MonoBehaviour
{
    public abstract string ModuleName { get; }
    
    public virtual UniTask ExecuteAsync(CancellationToken token = default) { return UniTask.CompletedTask; }
    public virtual void ExecuteSync() { }
}