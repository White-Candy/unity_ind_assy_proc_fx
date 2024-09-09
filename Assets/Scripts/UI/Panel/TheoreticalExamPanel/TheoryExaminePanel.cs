using sugar;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TheoryExaminePanel : BasePanel
{
    // �ύ��ť
    public Button m_Commit;

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

        m_Commit.onClick.AddListener(Submit);
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
        m_singleList = new List<SingleChoice>(inf.SingleChoices);
        m_mulitList = new List<MulitChoice>(inf.MulitChoices);
        m_tofList = new List<TOFChoice>(inf.TOFChoices);

        m_singlePanel.Init(m_singleList);
    }

    public void Submit()
    {

    }

    public void Close() 
    {

    }
}