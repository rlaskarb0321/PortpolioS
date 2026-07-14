using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TheBackendLoginManager : AppLaunchModuleBase
{
    [SerializeField] private GameObject loginCanvas;
    [SerializeField] private Button loginButton;
    
    public override string ModuleName { get => "The Backend Login Manager"; }

    public override async UniTask ExecuteAsync(CancellationToken token = default)
    {
        CustomLogin(ResolveCredential(), "1234");
        await UniTask.CompletedTask;
        
        Destroy(loginCanvas);
    }

    private string ResolveCredential()
    {
#if UNITY_EDITOR
        string project = new DirectoryInfo(Application.dataPath).Parent.Name;
        Debug.Log($"Project Name: {project}");

        if (project.EndsWith("_clone_0")) return "user2";
        if (project.EndsWith("_clone_1")) return "user3";
        return "user1";
#endif
        return "user1";
    }

    private void Start()
    {
        loginButton.onClick.AddListener(() => CustomLogin("user1", "1234"));
    }

    private void CustomLogin(string username, string password)
    {
        BackendLogin backendLogin = new BackendLogin();
        
        backendLogin.CustomLogin(username, password);
    }
}
