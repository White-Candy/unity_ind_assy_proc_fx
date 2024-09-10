using LitJson;
using sugar;
using System.Collections.Generic;
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
    
    // ��ѡ�������б�
    private List<SingleChoice> m_singleList = new List<SingleChoice>();

    // ��ѡ�������б�
    private List<MulitChoice> m_mulitList = new List<MulitChoice>();

    // �ж��������б�
    private List<TOFChoice> m_tofList = new List<TOFChoice>();

    private ExamineInfo m_info = new ExamineInfo(); // �� ��Ŀ| �γ� ��������Ϣ��

    public void Start()
    {
        m_singlePanel = UIConsole.FindPanel<SinglePanel>();
        m_mulitPanel = UIConsole.FindPanel<MulitPanel>();
        m_tofPanel = UIConsole.FindPanel<TOFPanel>();

        queType.onValueChanged.AddListener((i) => 
        {
            m_singlePanel.Active(i == 0 ? true : false);
            m_mulitPanel.Active(i == 1 ? true : false);
            m_tofPanel.Active(i == 2 ? true : false);
        });

        m_Submit.onClick.AddListener(() => 
        {
            UITools.OpenDialog("", "�Ƿ��ύ�������ۿ��ˣ�", () => 
            {
                float _theoryScore = Submit(m_info); 
                ScoreInfo inf = new ScoreInfo()
                {
                    className = GlobalData.usrInfo.className,
                    columnsName = m_info.ColumnsName,
                    courseName = m_info.CourseName,
                    registerTime = m_info.RegisterTime,
                    userName = GlobalData.usrInfo.userName,
                    Name = GlobalData.usrInfo.Name,
                    theoryScore = _theoryScore.ToString(),
                    theoryFinished = true,
                };
                TCP.SendAsync(JsonMapper.ToJson(inf), EventType.ScoreEvent, OperateType.ADD);
                // Debug.Log($"Submit Finish! User Name: {GlobalData.usrInfo.userName}, | Theory Score: {_theoryScore}, | This Examins Register Time: {m_info.RegisterTime}");
                Active(false);
            });
        });
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

    public void Close() 
    {
        m_singlePanel.Clear();
        m_mulitPanel.Clear();
        m_tofPanel.Clear();
        queType.value = 0;
        Active(false);
    }
}