using Cysharp.Threading.Tasks;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 点击训练按钮触发的事件
public class TrainEvent : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        base.OnEvent(args);
        //Debug.Log("Train Event!");

#if UNITY_STANDALONE_WIN
        TCP.SendAsync("[]", EventType.GetProjInfo, OperateType.NONE);
#endif
        GlobalData.mode = Mode.Practice;
        // SwitchSceneAccName(m_Name);
        GlobalData.currModuleName = m_Name;
        await SceneManager.LoadSceneAsync("Main");
        TitlePanel._instance.SetTitlePanelActive(false);

        await UniTask.Yield();
    }
}
