using Cysharp.Threading.Tasks;
using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static QuestionData;

public class TheoreticalExamAction : BaseAction
{
    private TheoreticalExamPanel m_Panel;

    private bool m_Init;

    public TheoreticalExamAction()
    {
        m_Panel = UIConsole.Instance.FindAssetPanel<TheoreticalExamPanel>();

        m_Token = new CancellationTokenSource();
        m_panelToken = new CancellationTokenSource();
    }

    public override async UniTask AsyncShow(string name) 
    {
        if (GlobalData.isFinishTheoreticalExam)
        {
            UITools.OpenDialog("", "已完成理论考核，请在退出考核模式后前往后台查询成绩。", () => { });
            return;
        }

        if (GlobalData.mode == Mode.Examination)
        {
            ExamJsonData ex_js_data = GlobalData.examData; // 可以理解为里面是理论试题
            foreach (var child in ex_js_data.data.softwareInfoVo.child)
            {
                if (child.code.Equals(GlobalData.currModuleCode))
                {
                    m_Panel.Init(ConvertExam(child.softwareQuestionVos));
                    m_Init = true;
                    await UniTask.WaitUntil(() => m_Init == true, PlayerLoopTiming.Update, m_Token.Token);
                    m_Panel.Active(true);
                    break;
                }
            }
            try
            {
                await UniTask.WaitUntil(() => m_Panel?.m_Content.activeSelf == false, PlayerLoopTiming.Update, m_panelToken.Token);
            }
            catch { }
        }
    }

    /// <summary>
    /// 处理服务器的body信息到内存中
    /// </summary>
    /// <param name="items"></param>
    /// <returns></returns>
    public List<QuestionData> ConvertExam(List<SoftwareQuestionVosItem> items)
    {
        List<QuestionData> qds = new List<QuestionData>();
        int idx = 1;
        float score = GlobalData.theoreticalExamscore / items.Count; // 每道题的分数
        foreach (var item in items) // 把每道题的信息存储到内存当中去 
        {
            QuestionData q_data = new QuestionData();
            q_data.number = idx++;
            q_data.ID = item.questionId;
            q_data.type = (QuestionType)item.type - 1;
            q_data.text = item.body;
            q_data.answer = item.answer;
            string opt = string.Format($"{item.choiceA}_{item.choiceB}_{item.choiceC}_{item.choiceD}_{item.choiceE}_{item.choliceF}");
            q_data.options = QuestionData.GetOptions(opt);
            q_data.score = score;
            qds.Add(q_data);
        }
        return qds;
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
