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
    // �ύ��ť
    public Button m_Submit;

    // �رհ�ť
    public Button m_Close;

    // ��Ŀ���� (��ѡ, ��ѡ, �ж�)
    public TMP_Dropdown queType;

    // ģ��
    public GameObject m_Template;

    // ģ��ĸ��ؼ�
    public GameObject m_Parent;
    // ��ѡ����
    public SinglePanel m_singlePanel;

    // ��ѡ����
    public MulitPanel m_mulitPanel;

    // �жϴ���
    public TOFPanel m_tofPanel;
    public TextMeshProUGUI m_CountDown; // ����ģʽ����ʱ

    // ��ѡ�������б�
    private List<SingleChoice> m_singleList = new List<SingleChoice>();

    // ��ѡ�������б�
    private List<MulitChoice> m_mulitList = new List<MulitChoice>();

    // �ж��������б�
    private List<TOFChoice> m_tofList = new List<TOFChoice>();

    private ExamineInfo m_info = new ExamineInfo(); // �� ��Ŀ| �γ� ��������Ϣ��

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

        // Ĭ���ǵ�ѡ����
        m_singlePanel.Active(true);
        m_mulitPanel.Active(false);
        m_tofPanel.Active(false);
    }

    /// <summary>
    /// ��ʼ���������
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
    /// ��ɲ���
    /// </summary>
    public void FinishAction()
    {
        UITools.OpenDialog("�ɼ��ύ", "�Ƿ��ύ�������ۿ��ˣ�", new ButtonData("ȡ��", FPath.DialogWhite, () => { }), new ButtonData("ȷ��", FPath.DialogBlue, () => 
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

    // ��ʼ����ʱ
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
            UITools.OpenDialog("ʱ�䳬ʱ", "ʱ�䵽�����Զ�����", 
                new ButtonData("ȡ��", FPath.DialogWhite, () => { }), 
                new ButtonData("ȷ��", FPath.DialogBlue, () => { ExamineSubmit(); }));
    }
    
    // �޸�UI��ʱ��
    private void UpdateTimeOnUI(int time)
    {
        int hour = time / 3600;
        int min = (time - hour * 3600) / 60;
        int second = time - hour * 3600 - min * 60;

        string str_time = $"{Tools.FillingForTime(hour.ToString()) + ":" + Tools.FillingForTime(min.ToString()) + ":" + Tools.FillingForTime(second.ToString())}";
        m_CountDown.SetText(str_time);
    }

    // ֹͣCountDown���߳�
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