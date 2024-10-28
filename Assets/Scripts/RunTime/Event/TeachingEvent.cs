using Cysharp.Threading.Tasks;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 点击教学按钮的事件
public class TeachingEvent : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        base.OnEvent(args);
        //Debug.Log("Teaching Event!");
#if UNITY_STANDALONE_WIN
        TCP.SendAsync("[]", EventType.GetProjInfo, OperateType.NONE);
#endif
        SwitchSceneAccName(m_Name);
        await UniTask.Yield();
    }
}
