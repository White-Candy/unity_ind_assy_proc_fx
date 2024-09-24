using Cysharp.Threading.Tasks;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TrainingAction : BaseAction
{
    public bool isFinsh = false;

    public TrainingAction()
    {
        m_Token = new CancellationTokenSource();
    }

    public override async UniTask AsyncShow(string name)
    {
        // Debug.Log("进入实训考核！");
        var inf = GlobalData.scoresInfo.Find(x => x.className == GlobalData.usrInfo.UnitName && x.userName == GlobalData.usrInfo.userName 
                                             && x.courseName == GlobalData.currExamsInfo.CourseName && x.registerTime == GlobalData.currExamsInfo.RegisterTime);
        // Debug.Log($"{GlobalData.usrInfo.className} | {GlobalData.usrInfo.userName} | {GlobalData.currExamsInfo.CourseName} | {GlobalData.currExamsInfo.RegisterTime} ");
        if (inf != null && inf.trainingFinished)
        {
            UITools.OpenDialog("", "已完成实训考核。", () => { }, true);
            return;
        }

        //Debug.Log(name);
        await Tools.LoadSceneModel();
        InfoPanel._instance.TrainingModeUIClose();
        MenuPanel._instance.Active(false);
        GlobalData.mode = Mode.Examination;

        // TODO...因为服务器的变更，这一部分的代码都要重新写 TAT..
        /*GlobalData.codeVSidDic.Clear();
        foreach (var child in GlobalData.examData.data.softwareInfoVo.child)
        {
            // Debug.Log("当前的CODE = " + child.code + "       当前资源ID为 = " + child.softwareId + "       当前的资源名称为 = " + child.softwareName);
            GlobalData.codeVSidDic.Add(child.code, child.softwareId);
        }
        
        try
        {
            await UniTask.WaitUntil(() => isFinsh == true);
            //Debug.Log("await finish");
        }
        catch { }*/
    }

    public override void Exit(Action callback)
    {
        base.Exit(callback);
        // Debug.Log("实操模式退出");
        isFinsh = true;
        CameraControl.SetMain();
    }
}
