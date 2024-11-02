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
        // Debug.Log("����ʵѵ���ˣ�");
        var inf = GlobalData.scoresInfo.Find(x => x.className == GlobalData.usrInfo.UnitName && x.userName == GlobalData.usrInfo.userName 
                                             && x.courseName == GlobalData.currExamsInfo.CourseName && x.registerTime == GlobalData.currExamsInfo.RegisterTime);
        // Debug.Log($"{GlobalData.usrInfo.className} | {GlobalData.usrInfo.userName} | {GlobalData.currExamsInfo.CourseName} | {GlobalData.currExamsInfo.RegisterTime} ");
        if (inf != null && inf.trainingFinished || GlobalData.practSubmit)
        {
            UITools.OpenDialog("�������", "�����ʵѵ���ˡ�", new ButtonData("ȷ��", FPath.DialogBlue, () => { }));
            return;
        }

        //Debug.Log(name);
        await Tools.LoadSceneModel();
        InfoPanel._instance.TrainingModeUIClose();
        MenuPanel._instance.Active(false);
        GlobalData.mode = Mode.Examination;
        TitlePanel._instance.SetTitlePanelActive(false);
    }

    public override void Exit(Action callback)
    {
        base.Exit(callback);
        // Debug.Log("ʵ��ģʽ�˳�");
        isFinsh = true;
        CameraControl.SetMain();
    }
}
