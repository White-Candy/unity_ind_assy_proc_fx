using Cysharp.Threading.Tasks;
using LitJson;
using sugar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserLoginEvent : BaseEvent
{
    public override async void OnPrepare(params object[] args) { await UniTask.Yield(); }

    public override async void OnEvent(params object[] args)
    {
        await UniTask.RunOnThreadPool(async () =>
        {
            MessPackage mp = args[0] as MessPackage;
            UserInfo info = JsonMapper.ToObject<UserInfo>(mp.ret);
            if (info.login)
            {
                UITools.Loading("Menu");
            }

            await UniTask.SwitchToMainThread();
            UITools.ShowMessage(info.hint);
        });
    }
}
