using Cysharp.Threading.Tasks;
using sugar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �����ѧ��ť���¼�
public class TeachingEvent : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        base.OnEvent(args);
        //Debug.Log("Teaching Event!");

        TCP.SendAsync("[]", EventType.GetProjInfo, OperateType.NONE);
        SwitchSceneAccName(m_Name);
        await UniTask.Yield();
    }
}
