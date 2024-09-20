
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ModuleSelectPanel : BasePanel
{
    public GameObject m_Item; // ģʽ�x���Item���o

    public Button m_Exit; // �˳����o

    // public GameObject m_TaskListPanel; // �����΄��б���
    public Button m_CloseTask; // ���˴����P�]���o
    public Transform m_ParentTrans; // �����б��еİ��o������Parent
    // public Button m_Task; // �����б��еİ��o

    private ConfigModuleList m_configModuleList; // �x�����������Щģʽ�����@���Y�汣��

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

        // ��Module_List������ģʽ��ť
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
            // ����UI���Ѿ��е�Item��clone�����ͬ���ֵ�Item
            GameObject item_clone = Instantiate(m_Item, m_Item.transform.parent);
            item_clone.gameObject.SetActive(true);
            item_clone.transform.Find("Title").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/UI/Title/{item.moduleName}");

            Button button = item_clone.GetComponentInChildren<Button>();
            button.GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/UI/{item.moduleName}");

            button.onClick.AddListener(() =>
            {
                // ��ͬģʽִ�в�ͬ�� OnEvent.
                BaseEvent @event = ModuleEventSpawn.Spawn<BaseEvent>(Tools.Escaping(item.moduleName), item.moduleCode, this);
                @event.OnEvent();
            });
        }
    }
}