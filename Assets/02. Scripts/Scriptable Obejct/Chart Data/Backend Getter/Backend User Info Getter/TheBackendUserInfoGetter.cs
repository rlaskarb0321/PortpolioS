using BackEnd;
using UnityEngine;

public class TheBackendUserInfoGetter
{
    public string GetUserNickName()
    {
        BackendReturnObject bro = Backend.BMember.GetUserInfo();
        return bro.GetReturnValuetoJSON()["row"]["nickname"].ToString();
    }
}
