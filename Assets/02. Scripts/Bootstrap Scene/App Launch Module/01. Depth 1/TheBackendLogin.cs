using BackEnd;
using UnityEngine;

public class BackendLogin
{
    private static BackendLogin instance = null;

    public static BackendLogin Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new BackendLogin();
            }

            return instance;
        }
    }

    public void CustomSignUp(string id, string pw)
    {
        Debug.Log("회원가입을 요청합니다.");
        var bro = Backend.BMember.CustomSignUp(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("회원가입에 성공했습니다. : " + bro);
        }
        else
        {
            Debug.LogError("회원가입에 실패했습니다. : " + bro);
        }
    }

    public void CustomLogin(string id, string pw)
    {
        Debug.Log("로그인을 요청합니다.");
        var bro = Backend.BMember.CustomLogin(id, pw);

        if (bro.IsSuccess())
        {
            Debug.Log("로그인이 성공했습니다. : " + bro);
        }
        else
        {
            Debug.LogError("로그인이 실패했습니다. : " + bro);
        }
    }

    public void UpdateNickname(string nickname)
    {
        // Step 4. 닉네임 변경 구현하기 로직
    }
}