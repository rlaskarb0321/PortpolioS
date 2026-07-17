using System.Threading;
using BackEnd;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LitJson;

public class TheBackendUserDataLoader : AppLaunchModuleBase, IBootstrapInstance
{
    private UserData userData;
    private string gameDataRowInDate;
    
    public override string ModuleName { get => "The Backend User Data Loader"; }
    public UserData UserData { get => userData; }
    public string GameDataRowInDate { get => gameDataRowInDate; }

    public override async UniTask ExecuteAsync(CancellationToken token = default)
    {
        var uniTask = new UniTaskCompletionSource<BackendReturnObject>();
        
        Backend.GameData.GetMyData("USER_DATA", new Where(), bro =>
        {
            if(bro.IsSuccess() == false)
            {
                uniTask.TrySetException(new System.Exception("[TheBackendUserDataLoader] Failed to Init Backend: " + bro));
                return;
            }

            JsonData gameDataJson = bro.FlattenRows(); 
            
            userData = new UserData();
            if(gameDataJson.Count <= 0)
            {
                // uniTask.TrySetException(new System.Exception("[TheBackendUserDataLoader] There is no USER_DATA"));
                Param param = new Param();
                
                param.Add("mainCharacterIndex", userData.mainCharacterIndex);
                param.Add("mainCharacterSkinIndex", userData.mainCharacterSkinIndex);
                
                var insertBRO = Backend.GameData.Insert("USER_DATA", param);
                
                gameDataRowInDate = bro.GetInDate();
                return;
            }
            
            gameDataRowInDate = gameDataJson[0]["inDate"].ToString();
            userData.mainCharacterIndex = int.Parse(gameDataJson[0]["mainCharacterIndex"].ToJson());
            userData.mainCharacterSkinIndex = int.Parse(gameDataJson[0]["mainCharacterSkinIndex"].ToJson());
            AllocateToBootstrapInstance();
            uniTask.TrySetResult(bro);
        });

        await uniTask.Task;
    }

    public EBootstrapInstance GetInstanceType()
    {
        return EBootstrapInstance.TheBackendUserDataLoader;
    }

    public void AllocateToBootstrapInstance()
    {
        if (BootstrapSceneInstance.Instance == null)
        {
            Debug.LogError("[LoadingSceneManager] Failed to assign to BootstrapSceneInstance");
            return;
        }
        
        BootstrapSceneInstance.Instance.AllocateToBootstrapInstance(GetInstanceType(), this);
    }
}