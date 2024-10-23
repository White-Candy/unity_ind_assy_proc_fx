using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitJson;

using UnityEngine;

public class ScoreEvent : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        MessPackage mp = args[0] as MessPackage;
        GlobalData.scoresInfo = JsonMapper.ToObject<List<ScoreInfo>>(mp.ret);
        await UniTask.Yield();
    }
}
