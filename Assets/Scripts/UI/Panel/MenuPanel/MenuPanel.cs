using LitJson;
using sugar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

// 菜單面板
public class MenuPanel : BasePanel
{
    public GameObject m_MenuItem;
    public Transform menuItemParent;

    private List<Menu> m_MenuList = new List<Menu>(); // 裏面有需要配置工具的信息
    private string currTaskName; // 目前的Task名字
    private BaseTask currTask; // 目前的任务实例

    private Dictionary<string, BaseTask> m_TaskDic = new Dictionary<string, BaseTask>(); // 任务字典

    private void Start()
    {
        ConfigMenuList menulist = ConfigConsole.Instance.FindConfig<ConfigMenuList>();
        m_MenuList = menulist.m_MenuList;

        Init();
    }

    private void Init()
    {
        if (m_MenuItem == null)
        {
            Debug.Log("菜單實例不存在！");
            return;
        }

        foreach (var menu in m_MenuList)
        {
            string[] menuName = menu.menuName.Split('_');
            //Debug.Log("GlobalData.currModuleName: " + GlobalData.currModuleName + "| menuName[1]: " + menuName[1]);
            if (GlobalData.currModuleName == menuName[1])
            {
                if (menuName[0] != "拆装仿真")
                {
                    // TODO..
                }
                else
                {
                    Debug.Log("menuName: " + menuName[0]);
                    // 借用 SubMenu 结构存储属于subMenuName的流程步骤
                    List<SubMenu> list = new List<SubMenu>();
                    foreach (var item in menu.subMenuList)
                    {
                        //Debug.Log("当前code为 = " + GlobalData.currModuleCode + " | 列表ID为 = " + item.enumID.ToString());
                        if (int.Parse(GlobalData.currModuleCode) == item.enumID)
                        {
                            Debug.Log("curr Name: " + item.subMenuName);
                            NetworkManager._Instance.DownLoadTextFromServer((Application.streamingAssetsPath + "/ModelExplain/" + item.subMenuName + "Step.txt"), (dataStr) => 
                            {
                                Debug.Log(dataStr);
                                JsonData js_data = JsonMapper.ToObject(dataStr);
                                JsonData step = js_data["child"];
                                for(int i = 0; i < step.Count; i++)
                                {
                                    SubMenu subMenu = new SubMenu();
                                    subMenu.subMenuName = step[i].ToString();
                                    subMenu.enumID = item.enumID;
                                    list.Add(subMenu);
                                }
                                SpawnTask(menuName[0], list); // 不同的模式分发不同的事件
                            });
                        }
                    }
                }
            }
        }
    }

    private void SpawnTask(string menuName, List<SubMenu> list)
    {
        if (currTaskName == menuName)
        {
            Debug.Log($"重复进入 ： {menuName}");
            return;
        }

        if (currTask != null)
            currTask.Exit();

        BaseTask task = null;
        m_TaskDic.TryGetValue(menuName, out task);
        if (task == null)  // 生成
        {
            switch (menuName)
            {
                case "教学资源": //认知学习
                    task = new RenZhiXueXi();
                    break;
                case "施工准备": //构造原理
                    task = new GouZaoYuanLi();
                    break;
                case "拆装仿真":
                    task = new ChaiZhuangFangZhen();
                    task.m_Drag = true;
                    break;
                case "考核":
                    task = new ShiXunKaoHe();
                    break;
            }
            m_TaskDic.Add(menuName, task);

            // TODO..
            if (task.m_Drag)
            {
                // 显示提示窗口控件
            }
            else
            {

            }
        }
        currTaskName = menuName;
        currTask = task;
        if (!currTask.IsInit)
        {
            currTask.Init(list, transform.Find("Content/BG"));
        }
        currTask.Show();
    }
}
