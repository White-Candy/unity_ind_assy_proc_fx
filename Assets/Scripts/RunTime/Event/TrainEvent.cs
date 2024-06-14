using sugar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 点击训练按钮触发的事件
public class TrainEvent : ModuleEvent
{
    public override void OnEvent(params object[] args)
    {
        base.OnEvent(args);
        //Debug.Log("Train Event!");

        foreach (var t in GlobalData.moduleContent)
        {
            SwitchSceneAccName(base.m_Name, t[1]);
        }
    }
}
