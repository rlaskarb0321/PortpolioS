using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class AppLaunchModuleBase : MonoBehaviour
{
    public abstract string ModuleName { get; }
    public abstract UniTask Execute(CancellationToken token = default);
}