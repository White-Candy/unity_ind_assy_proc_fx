using Cysharp.Threading.Tasks;
using LitJson;
using sugar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegisterEvent : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        await UniTask.RunOnThreadPool(async () =>
        {
            MessPackage mp = args[0] as MessPackage;
            UserInfo info = JsonMapper.ToObject<UserInfo>(mp.ret);
            
            await UniTask.SwitchToMainThread();

            if (!string.IsNullOrEmpty(info.userName))
            {
                LoginPanel._instance.Active(true);
                RegisterPanel._instance.Active(false);
            }

            UITools.ShowMessage(info.hint);
        });
    }

}
