using Cysharp.Threading.Tasks;
using sugar;
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

        SwitchSceneAccName(m_Name);
        await UniTask.Yield();
    }
}
