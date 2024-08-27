using Cysharp.Threading.Tasks;
using LitJson;
using sugar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserLoginEvent : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        await UniTask.RunOnThreadPool(async () =>
        {
            MessPackage mp = args[0] as MessPackage;
            UserInfo info = JsonMapper.ToObject<UserInfo>(mp.ret);

            await UniTask.SwitchToMainThread();
            if (info.login)
            {
                UITools.Loading("Menu");
            }

            UITools.ShowMessage(info.hint);
        });
    }
}
