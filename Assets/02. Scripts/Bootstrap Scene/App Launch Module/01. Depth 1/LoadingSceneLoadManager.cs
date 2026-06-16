using UnityEngine.SceneManagement;

public class LoadingSceneLoadManager : AppLaunchModuleBase
{
    public override string ModuleName { get => "Loading Scene Load Manager"; }

    public override void ExecuteSync()
    {
        SceneManager.LoadScene("Loading Scene", LoadSceneMode.Additive);
    }
}
