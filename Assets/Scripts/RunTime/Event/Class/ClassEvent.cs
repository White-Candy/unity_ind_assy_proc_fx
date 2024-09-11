using UnityEngine;
using Cysharp.Threading.Tasks;
using LitJson;
using System.Collections.Generic;
using sugar;


public class ClassEvent : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        MessPackage mp = args[0] as MessPackage;
        // Debug.Log("GetProjInfo OnEvent");
        GlobalData.classList = JsonMapper.ToObject<List<ClassInfo>>(mp.ret);
        await UniTask.Yield();
    }
}

/// <summary>
///  班级信息包
/// </summary>
public class ClassInfo
{
    public string id;
    public string Class;
    public string RegisterTime;
    public string Faculty;
    public string Major;
    public string Teacher;
    public int Number;
}