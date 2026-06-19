using Cysharp.Threading.Tasks;

public interface ISceneLoadCallback
{
    public UniTask OnSceneActivated();
    public void OnSceneLoaded();
}
