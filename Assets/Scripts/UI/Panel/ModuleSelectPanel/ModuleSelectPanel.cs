using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleSelectPanel : BasePanel
{
    public GameObject m_Item; // 模式選擇的Item按鈕

    public Button m_Exit; // 退出按鈕

    public GameObject m_TaskListPanel; // 考核任務列表窗口
    public Button m_CloseTask; // 考核窗口關閉按鈕
    public Transform m_ParentTrans; // 考核列表中的按鈕依附的Parent
    public Button m_Task; // 考核列表中的按鈕

    private ConfigModuleList m_ModuleList;

    public override void Awake()
    {
        base.Awake();

        m_Exit.onClick.AddListener(UITools.Quit);
        m_CloseTask.onClick.AddListener(CloseTaskPanel);
    }

    private void Start()
    {
        m_ModuleList = ConfigConsole.Instance.FindConfig<ConfigModuleList>();
    }

    public void CloseTaskPanel()
    {
        for (int i = 0; i < m_ParentTrans.childCount; i++)
        {
            m_ParentTrans.GetChild(i).gameObject.SetActive(false);
        }
        m_TaskListPanel.SetActive(false);
    }
}
