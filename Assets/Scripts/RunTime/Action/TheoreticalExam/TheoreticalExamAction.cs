using Cysharp.Threading.Tasks;
using sugar;
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
        // if (GlobalData.isFinishTheoreticalExam)
        // {
        //     UITools.OpenDialog("", "已完成理论考核，请在退出考核模式后前往后台查询成绩。", () => { }, true);
        //     return;
        // }
        // Debug.Log($"user inf: {GlobalData.usrInfo.userName} | {GlobalData.usrInfo.Name} | {GlobalData.usrInfo.className}");
        var inf = GlobalData.scoresInfo.Find(x => x.className == GlobalData.usrInfo.className && x.userName == GlobalData.usrInfo.userName 
                                             && x.courseName == GlobalData.currExamsInfo.CourseName && x.registerTime == GlobalData.currExamsInfo.RegisterTime);
        Debug.Log($"{GlobalData.usrInfo.className} | {GlobalData.usrInfo.userName} | {GlobalData.currExamsInfo.CourseName} | {GlobalData.currExamsInfo.RegisterTime} ");
        if (inf != null && inf.theoryFinished)
        {
            UITools.OpenDialog("", "已完成理论考核。", () => { }, true);
            return;
        }

        if (GlobalData.mode == Mode.Examination)
        {
            m_Panel = UIConsole.FindAssetPanel<TheoryExaminePanel>();
            m_Panel.Init(GlobalData.currExamsInfo);
            m_Panel.Active(true);
            try
            {
                await UniTask.WaitUntil(() => m_Panel?.m_Content.activeSelf == false, PlayerLoopTiming.Update, m_panelToken.Token);
            }
            catch { }
        }
    }

    // /// <summary>
    // /// 处理服务器的body信息到内存中
    // /// </summary>
    // /// <param name="items"></param>
    // /// <returns></returns>
    // public List<QuestionData> ConvertExam(List<SoftwareQuestionVosItem> items)
    // {
    //     List<QuestionData> qds = new List<QuestionData>();
    //     int idx = 1;
    //     float score = GlobalData.theoreticalExamscore / items.Count; // 每道题的分数
    //     foreach (var item in items) // 把每道题的信息存储到内存当中去 
    //     {
    //         QuestionData q_data = new QuestionData
    //         {
    //             number = idx++,
    //             ID = item.questionId,
    //             type = (QuestionType)item.type - 1,
    //             text = item.body,
    //             answer = item.answer
    //         };
    //         string opt = string.Format($"{item.choiceA}_{item.choiceB}_{item.choiceC}_{item.choiceD}_{item.choiceE}_{item.choliceF}");
    //         q_data.options = QuestionData.GetOptions(opt);
    //         q_data.score = score;
    //         qds.Add(q_data);
    //     }
    //     return qds;
    // }

    /// <summary>
    /// 退出
    /// </summary>
    public override void Exit(Action callback)
    {
        base.Exit(callback);
        m_Panel.Close();
    }
}
