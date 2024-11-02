using Cysharp.Threading.Tasks;
using LitJson;

using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TheoryExaminePanel : BasePanel
{
    // 提交按钮
    public Button m_Submit;

    // 关闭按钮
    public Button m_Close;

    // 题目类型 (单选, 多选, 判断)
    public TMP_Dropdown queType;

    // 模板
    public GameObject m_Template;

    // 模板的父控件
    public GameObject m_Parent;
    // 单选窗口
    public SinglePanel m_singlePanel;

    // 多选窗口
    public MulitPanel m_mulitPanel;

    // 判断窗口
    public TOFPanel m_tofPanel;
    public TextMeshProUGUI m_CountDown; // 考核模式倒计时

    // 单选题问题列表
    private List<SingleChoice> m_singleList = new List<SingleChoice>();

    // 多选题问题列表
    private List<MulitChoice> m_mulitList = new List<MulitChoice>();

    // 判断题问题列表
    private List<TOFChoice> m_tofList = new List<TOFChoice>();

    private ExamineInfo m_info = new ExamineInfo(); // 该 栏目| 课程 的所有信息。

    private CancellationTokenSource m_cts = new CancellationTokenSource();

    public override void Start()
    {
        // m_singlePanel = UIConsole.FindPanel<SinglePanel>();
        // m_mulitPanel = UIConsole.FindPanel<MulitPanel>();
        // m_tofPanel = UIConsole.FindPanel<TOFPanel>();

        queType.onValueChanged.AddListener((i) => 
        {
            Debug.Log("queType value changed!");
            m_singlePanel.Active(i == 0 ? true : false);
            m_mulitPanel.Active(i == 1 ? true : false);
            m_tofPanel.Active(i == 2 ? true : false);
        });

        m_Submit.onClick.AddListener(FinishAction);

        m_Close.onClick.AddListener(Close);

        // 默认是单选界面
        m_singlePanel.Active(true);
        m_mulitPanel.Active(false);
        m_tofPanel.Active(false);
    }

    /// <summary>
    /// 初始化问题面板
    /// </summary>
    /// <param name="data"></param>
    public void Init(ExamineInfo inf)
    {
        m_info = inf.Clone();
        m_singleList = new List<SingleChoice>(m_info.SingleChoices);
        m_mulitList = new List<MulitChoice>(m_info.MulitChoices);
        m_tofList = new List<TOFChoice>(m_info.TOFChoices);

        m_singlePanel.Init(m_singleList);
        m_mulitPanel.Init(m_mulitList);
        m_tofPanel.Init(m_tofList);
    }

    public float Submit(ExamineInfo inf)
    {
        // Debug.Log($"Submit This Proj: Columns Name: {m_info.ColumnsName} | Course Name: {m_info.CourseName} | Register Time: {m_info.RegisterTime}");
        float theoryScore = 0.0f;
        List<string> usrSingle = m_singlePanel.Submit();
        List<string> usrMulit = m_mulitPanel.Submit();
        List<string> usrTof = m_tofPanel.Submit();

        for (int i = 0; i < usrSingle.Count; ++i)
            if (inf.SingleChoices[i].Answer == usrSingle[i]) { theoryScore += float.Parse(inf.SingleChoices[i].Score); }

        for (int i = 0; i < usrMulit.Count; ++i)
            if (inf.MulitChoices[i].Answer == usrMulit[i]) { theoryScore += float.Parse(inf.MulitChoices[i].Score); }

        for (int i = 0; i < usrTof.Count; ++i)
            if (inf.TOFChoices[i].Answer == usrTof[i]) { theoryScore += float.Parse(inf.TOFChoices[i].Score); }

        //Debug.Log("Total Theory: " + theoryScore);
        return theoryScore;
    }

    /// <summary>
    /// 完成操作
    /// </summary>
    public void FinishAction()
    {
        UITools.OpenDialog("成绩提交", "是否提交本次理论考核？", new ButtonData("取消", FPath.DialogWhite, () => { }), new ButtonData("确定", FPath.DialogBlue, () => 
        { 
            ExamineSubmit();
            GlobalData.theorSubmit = true;
        }));
    }

    public void ExamineSubmit()
    {
        float _theoryScore = Submit(m_info); 
        GlobalData.currScoreInfo.theoryScore = _theoryScore.ToString();
        GlobalData.currScoreInfo.theoryFinished = true;
        HTTPConsole.SendAsyncPost(JsonMapper.ToJson(GlobalData.currScoreInfo), EventType.ScoreEvent, OperateType.ADD);
        Debug.Log($"Submit Finish! User Name: {GlobalData.currScoreInfo.userName}, | Theory Score: {_theoryScore}, | This Examins Register Time: {GlobalData.currScoreInfo.registerTime}");
        Close();
    }

    // 开始倒计时
    public async UniTask StartCountDown()
    {
        int time = int.Parse(GlobalData.currExamsInfo.TheoryTime) * 60;
        await UniTask.WaitUntil(() => m_Visible == true);

        while (time > 0 && m_Visible)
        { 
            time--;
            UpdateTimeOnUI(time); 
            await UniTask.Delay(1000, cancellationToken: m_cts.Token);
            // Debug.Log(time);
        }

        if (time <= 0)
            UITools.OpenDialog("时间超时", "时间到，已自动交卷。", 
                new ButtonData("取消", FPath.DialogWhite, () => { }), 
                new ButtonData("确定", FPath.DialogBlue, () => { ExamineSubmit(); }));
    }
    
    // 修改UI的时间
    private void UpdateTimeOnUI(int time)
    {
        int hour = time / 3600;
        int min = (time - hour * 3600) / 60;
        int second = time - hour * 3600 - min * 60;

        string str_time = $"{Tools.FillingForTime(hour.ToString()) + ":" + Tools.FillingForTime(min.ToString()) + ":" + Tools.FillingForTime(second.ToString())}";
        m_CountDown.SetText(str_time);
    }

    // 停止CountDown的线程
    public void CancelCountDownToken()
    {
        m_cts.Cancel();
        m_cts.Dispose();
        m_cts = new CancellationTokenSource();
    }

    public void Close() 
    {
        CancelCountDownToken();
        m_singlePanel.Clear();
        m_mulitPanel.Clear();
        m_tofPanel.Clear();
        queType.value = 0;
        Active(false);
    }
    
}