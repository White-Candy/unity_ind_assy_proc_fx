using sugar;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TheoryExaminePanel : BasePanel
{
    // 提交按钮
    public Button m_Commit;

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
    
    // 单选题问题列表
    private List<SingleChoice> m_singleList = new List<SingleChoice>();

    // 多选题问题列表
    private List<MulitChoice> m_mulitList = new List<MulitChoice>();

    // 判断题问题列表
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