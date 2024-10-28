using Cysharp.Threading.Tasks;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class TheoreticalExamAction : BaseAction
{
    private TheoryExaminePanel m_Panel;

    private bool m_Init;

    public TheoreticalExamAction()
    {
        m_Token = new CancellationTokenSource();
        m_panelToken = new CancellationTokenSource();
    }

    public override async UniTask AsyncShow(string name)
    {
        var inf = GlobalData.scoresInfo.Find(x => x.className == GlobalData.usrInfo.UnitName && x.userName == GlobalData.usrInfo.userName 
                                             && x.courseName == GlobalData.currExamsInfo.CourseName && x.registerTime == GlobalData.currExamsInfo.RegisterTime);
                                             
        //Debug.Log($"{GlobalData.usrInfo.UnitName} | {GlobalData.usrInfo.userName} | {GlobalData.currExamsInfo.CourseName} | {GlobalData.currExamsInfo.RegisterTime} ");
        if (inf != null && inf.theoryFinished)
        {
            UITools.OpenDialog("考核完成", "已完成理论考核。", new ButtonData("确定", FPath.DialogBlue, () => { }));
            return;
        }

        GlobalData.currExamsInfo = GlobalData.ExamineesInfo.Find(x => x.RegisterTime == GlobalData.currExamsInfo.RegisterTime && x.CourseName == GlobalData.currExamsInfo.CourseName).Clone();
        int scoreIdx = GlobalData.scoresInfo.FindIndex(x => x.className == GlobalData.usrInfo.UnitName && x.userName == GlobalData.usrInfo.userName
                    && x.registerTime == GlobalData.currExamsInfo.RegisterTime && x.columnsName == GlobalData.currExamsInfo.ColumnsName 
                    && x.courseName == GlobalData.currExamsInfo.CourseName);
        if (scoreIdx == -1)
        {
            ScoreInfo inf0 = new ScoreInfo()
            {
                className = GlobalData.usrInfo.UnitName,
                columnsName = GlobalData.currExamsInfo.ColumnsName,
                courseName = GlobalData.currExamsInfo.CourseName,
                registerTime = GlobalData.currExamsInfo.RegisterTime,
                userName = GlobalData.usrInfo.userName,
                Name = GlobalData.usrInfo.Name,
            };
            GlobalData.currScoreInfo = inf0.Clone();
        }
        else GlobalData.currScoreInfo = GlobalData.scoresInfo[scoreIdx].Clone(); 

        if (GlobalData.mode == Mode.Examination)
        {
            m_Panel = UIConsole.FindAssetPanel<TheoryExaminePanel>();
            m_Panel.Init(GlobalData.currExamsInfo);
            m_Panel.Active(true);
            await m_Panel.StartCountDown().SuppressCancellationThrow();
            try
            {
                await UniTask.WaitUntil(() => m_Panel?.m_Content.activeSelf == false, PlayerLoopTiming.Update, m_panelToken.Token);
            }
            catch { }
        }
    }

    /// <summary>
    /// 退出
    /// </summary>
    public override void Exit(Action callback)
    {
        base.Exit(callback);
        m_Panel.Close();
    }
}
