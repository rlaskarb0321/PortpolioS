using Cysharp.Threading.Tasks;
using UnityEngine;

public class DevelopSceneInitializer : MonoBehaviour
{
    [SerializeField] private UISceneLoader uiSceneLoader;
    
    public void InitDevelopScene()
    {
        InitAsync().Forget();
    }

    private async UniTask InitAsync()
    {
        await uiSceneLoader.DoInit();
    }
}
