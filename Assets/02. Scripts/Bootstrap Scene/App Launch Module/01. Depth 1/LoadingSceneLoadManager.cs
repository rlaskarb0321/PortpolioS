using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneLoadManager : AppLaunchModuleBase
{
    [SerializeField] private SceneField loadingScene;
    
    public override string ModuleName { get => "Loading Scene Additive Load Manager"; }

    public override void ExecuteSync()
    {
        SceneManager.LoadScene(loadingScene.SceneName, LoadSceneMode.Additive);
    }
}
