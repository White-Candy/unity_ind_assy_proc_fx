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
        //     UITools.OpenDialog("", "��������ۿ��ˣ������˳�����ģʽ��ǰ����̨��ѯ�ɼ���", () => { }, true);
        //     return;
        // }
        // Debug.Log($"user inf: {GlobalData.usrInfo.userName} | {GlobalData.usrInfo.Name} | {GlobalData.usrInfo.className}");
        var inf = GlobalData.scoresInfo.Find(x => x.className == GlobalData.usrInfo.className && x.userName == GlobalData.usrInfo.userName 
                                             && x.courseName == GlobalData.currExamsInfo.CourseName && x.registerTime == GlobalData.currExamsInfo.RegisterTime);
        Debug.Log($"{GlobalData.usrInfo.className} | {GlobalData.usrInfo.userName} | {GlobalData.currExamsInfo.CourseName} | {GlobalData.currExamsInfo.RegisterTime} ");
        if (inf != null && inf.theoryFinished)
        {
            UITools.OpenDialog("", "��������ۿ��ˡ�", () => { }, true);
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
    // /// �����������body��Ϣ���ڴ���
    // /// </summary>
    // /// <param name="items"></param>
    // /// <returns></returns>
    // public List<QuestionData> ConvertExam(List<SoftwareQuestionVosItem> items)
    // {
    //     List<QuestionData> qds = new List<QuestionData>();
    //     int idx = 1;
    //     float score = GlobalData.theoreticalExamscore / items.Count; // ÿ����ķ���
    //     foreach (var item in items) // ��ÿ�������Ϣ�洢���ڴ浱��ȥ 
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
    /// �˳�
    /// </summary>
    public override void Exit(Action callback)
    {
        base.Exit(callback);
        m_Panel.Close();
    }
}
