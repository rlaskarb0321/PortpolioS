using Cysharp.Threading.Tasks;
using UnityEngine;

public class MenuSceneManager : MonoBehaviour, ISceneLoadCallback
{
    private void Awake()
    {
        var loadingSceneManager =
            BootstrapSceneInstance.Instance
            .GetBootstrapInstance<LoadingSceneManager>(EBootstrapInstance.LoadingSceneManager);

        loadingSceneManager.OnSceneActivated += OnSceneActivated;
        loadingSceneManager.OnCompleteLoad += OnSceneLoaded;
    }

    public async UniTask OnSceneActivated()
    {
        Debug.Log($"[MenuSceneManager] OnSceneActivated");
        await UniTask.CompletedTask;
    }

    public void OnSceneLoaded()
    {
        Debug.Log($"[MenuSceneManager] OnSceneLoaded");
    }
}
