using System;
using System.Threading;
using BackEnd;
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
        CustomLogin("user1", "1234");
        await UniTask.CompletedTask;
        
        Destroy(loginCanvas);
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
