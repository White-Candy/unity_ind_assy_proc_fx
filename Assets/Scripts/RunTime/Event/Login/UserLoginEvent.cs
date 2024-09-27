using Cysharp.Threading.Tasks;
using LitJson;

using UnityEngine;

public class UserLoginEvent : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        MessPackage mp = args[0] as MessPackage;
        await UniTask.SwitchToMainThread();
        UserInfo info = JsonMapper.ToObject<UserInfo>(mp.ret);
        GlobalData.usrInfo = info;

        if (info.login)
        {
            UITools.Loading("Menu");
        }

        UITools.ShowMessage(info.hint);
    }
}
