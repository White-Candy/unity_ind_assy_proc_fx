using Cysharp.Threading.Tasks;
using sugar;
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
        SwitchSceneAccName(base.m_Name);
        await UniTask.Yield();
    }
}
