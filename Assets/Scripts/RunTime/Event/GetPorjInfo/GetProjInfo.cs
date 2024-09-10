using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LitJson;
using sugar;

public class GetProjInfo : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        await UniTask.RunOnThreadPool(() =>
        {
            MessPackage mp = args[0] as MessPackage;
            // Debug.Log("GetProjInfo OnEvent");
            GlobalData.Projs = JsonMapper.ToObject<List<Proj>>(mp.ret);
        });
    }
}
