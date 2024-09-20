using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using LitJson;


public class GetProjInfo : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        MessPackage mp = args[0] as MessPackage;
        // Debug.Log("GetProjInfo OnEvent");
        GlobalData.Projs = JsonMapper.ToObject<List<Proj>>(mp.ret);
        await UniTask.Yield();
    }
}
