using Cysharp.Threading.Tasks;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 点击训练按钮触发的事件
public class TrainEvent : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        base.OnEvent(args);
        //Debug.Log("Train Event!");

#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        TCP.SendAsync("[]", EventType.GetProjInfo, OperateType.NONE);
#endif
        SwitchSceneAccName(m_Name);
        await UniTask.Yield();
    }
}
