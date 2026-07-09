using BackEnd;
using UnityEngine;

public class BackendLogin
{
    public void CustomLogin(string id, string pw)
    {
        Debug.Log($"로그인을 요청합니다. id: {id}");
        
#if UNITY_EDITOR
        var bro = Backend.BMember.CustomLogin(id, pw);

        if (bro.IsSuccess() == false)
        {
            Debug.LogError("로그인이 실패했습니다. : " + bro);
        }
#else
        GoogleLoginManager googleLogin = new GoogleLoginManager();
        
        googleLogin.StartGoogleLogin();
#endif
    }
}

public class GoogleLoginManager
{
    public void StartGoogleLogin()
    {
        Debug.Log($"[MenuSceneManager] StartGoogleLogin");
        string webClientId = "919448465881-qg2ukmpoa21leao4hunbftnrjuu1ub9b.apps.googleusercontent.com";
        
        GoogleCredentialManagerBridge.SignIn(webClientId, GoogleLoginCallback);
        Debug.Log($"[MenuSceneManager] GoogleLogin called successfully");
    }

    private void GoogleLoginCallback(bool isSuccess, string errorMessage, string token)
    {
        if (isSuccess == false)
        {
            Debug.Log($"[MenuSceneManager] GoogleLoginCallback");
            Debug.LogError(errorMessage);
            return;
        }

        Debug.Log("구글 토큰 : " + token);
        Backend.BMember.AuthorizeFederation(token, FederationType.Google, "google", callback =>
        {
            if (callback.IsSuccess())
            {
                Debug.Log("구글 로그인 성공");
            }
            else
            {
                Debug.LogError("구글 로그인 실패: " + callback.GetErrorCode() + " " + callback.GetMessage());
            }
        });
    }
}