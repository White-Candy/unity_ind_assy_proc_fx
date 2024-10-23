using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using LitJson;

using UnityEngine;

public class ExamineEvent : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        MessPackage mp = args[0] as MessPackage;
        GlobalData.ExamineesInfo = JsonMapper.ToObject<List<ExamineInfo>>(mp.ret);
        await UniTask.Yield();
    }
}
