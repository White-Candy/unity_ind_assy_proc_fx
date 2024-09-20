
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ModuleSelectPanel : BasePanel
{
    public GameObject m_Item; // 模式選擇的Item按鈕

    public Button m_Exit; // 退出按鈕

    // public GameObject m_TaskListPanel; // 考核任務列表窗口
    public Button m_CloseTask; // 考核窗口關閉按鈕
    public Transform m_ParentTrans; // 考核列表中的按鈕依附的Parent
    // public Button m_Task; // 考核列表中的按鈕

    private ConfigModuleList m_configModuleList; // 選擇界面中有那些模式都在這個裏面保存

    public override void Awake()
    {
        base.Awake();

        m_Exit.onClick.AddListener(UITools.Quit);
        m_CloseTask.onClick.AddListener(CloseTaskPanel);

        // GlobalData.TaskListPanel = m_TaskListPanel;
        // GlobalData.Task = m_Task;
        GlobalData.TaskParent = m_ParentTrans;
    }

    private void Start()
    {
        m_configModuleList = ConfigConsole.Instance.FindConfig<ConfigModuleList>();

        foreach (var item in m_configModuleList.m_ModuleList)
        {
            //Debug.Log(item.moduleName);
        }

        // 用Module_List来创建模式按钮
        CreateModuleList(m_configModuleList.m_ModuleList);
    }

    public void CloseTaskPanel()
    {
        for (int i = 0; i < m_ParentTrans.childCount; i++)
        {
            m_ParentTrans.GetChild(i).gameObject.SetActive(false);
        }
        // m_TaskListPanel.SetActive(false);
    }

    private void CreateModuleList(List<ModuleData> modules)
    {
        if (modules == null || modules.Count <= 0) return;

        foreach (var item in modules)
        {
            // 利用UI中已经有的Item来clone多个不同名字的Item
            GameObject item_clone = Instantiate(m_Item, m_Item.transform.parent);
            item_clone.gameObject.SetActive(true);
            item_clone.transform.Find("Title").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/UI/Title/{item.moduleName}");

            Button button = item_clone.GetComponentInChildren<Button>();
            button.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/UI/{item.moduleName}");

            button.onClick.AddListener(() =>
            {
                // 不同模式执行不同的 OnEvent.
                BaseEvent @event = ModuleEventSpawn.Spawn<BaseEvent>(Tools.Escaping(item.moduleName), item.moduleCode, this);
                @event.OnEvent();
            });
        }
    }
}