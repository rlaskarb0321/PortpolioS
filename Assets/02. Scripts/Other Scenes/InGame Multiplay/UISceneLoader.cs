using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class UISceneLoader : SubManagerBase
{
    [SerializeField] private AssetReference targetScene;
    
    public override async UniTask DoInit()
    {
        await targetScene.LoadSceneAsync(LoadSceneMode.Additive);
        await UniTask.Yield();
    }
}
