using LitJson;
using sugar;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public struct StepStruct
{
    public string method; // 方法[拖拽 or 点击]
    public List<string> tools; // 工具
    public string stepName; // 步骤名称
    public List<string> animLimite; // 该步骤的动画帧数范围
}


// 菜單面板
public class MenuPanel : BasePanel
{
    public GameObject m_MenuItem;
    public Transform menuItemParent;

    private List<Menu> m_MenuList = new List<Menu>(); // 裏面有需要配置工具的信息
    private string currTaskName; // 目前的Task名字
    private BaseTask currTask; // 目前的任务实例

    private Dictionary<string, BaseTask> m_TaskDic = new Dictionary<string, BaseTask>(); // 任务字典

    private List<Button> m_Menus = new List<Button>(); // 保存菜单按钮列表

    private void Start()
    {
        ConfigMenuList menulist = ConfigConsole.Instance.FindConfig<ConfigMenuList>();
        m_MenuList = menulist.m_MenuList;
        BuildMenuList();
        //Init();
    }

    private void BuildMenuList()
    {
        foreach (var item in GlobalData.Targets)
        {
            GameObject menuItem = Instantiate(m_MenuItem, menuItemParent);
            menuItem.gameObject.SetActive(true);
            Button menuBtn = menuItem.transform.GetChild(0).GetComponent<Button>();
            menuBtn.GetComponentInChildren<TextMeshProUGUI>().text = item.menuName;
            menuBtn.onClick.AddListener((() => { ChooseThisItem(item); }));
            m_Menus.Add(menuBtn);
        }
    }

    private void ChooseThisItem(Target target)
    {
        GlobalData.ModelTarget = target;
        GlobalData.currModuleCode = target.modelCode.ToString();
        //GlobalData.currModuleName = target.modelName;

        if (GlobalData.currModuleName == "训练")
        {
            LoadSceneModel();
        }
        else
        {
            
        }
    }

    private void Init(string name)
    {
        if (m_MenuItem == null)
        {
            Debug.Log("菜單實例不存在！");
            return;
        }

        if (name == "训练")
        {
            NetworkManager._Instance.DownLoadTextFromServer((Application.streamingAssetsPath + "/ModelExplain/" + GlobalData.ModelTarget.modelName + "Step.txt"), (dataStr) =>
            {
                //Debug.Log(dataStr);
                List<StepStruct> list = new List<StepStruct>();
                JsonData js_data = JsonMapper.ToObject(dataStr);
                JsonData step = js_data["child"];
                for (int i = 0; i < step.Count; i++)
                {
                    StepStruct step_st = new StepStruct();
                    string[] field = step[i].ToString().Split("_");
                    if (field.Length == 4)
                    {
                        step_st.method = field[0];
                        step_st.tools = new List<string>(field[1].Split("|"));
                        step_st.stepName = field[2];
                        step_st.animLimite = new List<string>(field[3].Split("~"));
                    }
                    else
                    {
                        step_st.tools = new List<string>(field[0].Split("|"));
                        step_st.stepName = field[1];
                        step_st.animLimite = new List<string>(field[2].Split("~"));
                    }
                    list.Add(step_st);
                    //subMenu.subMenuName = step[i].ToString();
                    //subMenu.enumID = item.enumID;
                    //list.Add(subMenu);
                }
                SpawnTask(GlobalData.currModuleName, list); // 不同的模式分发不同的事件
                SetActiveMenuList(false);
            });
        }
        else
        {

        }
    }

    private void SpawnTask(string menuName, List<StepStruct> list)
    {
        if (currTaskName == menuName)
        {
            Debug.Log($"重复进入 ： {menuName}");
            return;
        }

        if (currTask != null)
            currTask.Exit();

        BaseTask task;
        m_TaskDic.TryGetValue(menuName, out task);
        if (task == null)  // 生成
        {
            switch (menuName)
            {
                case "教学": //认知学习
                    task = new RenZhiXueXi();
                    break;
                case "训练":
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
                InfoPanel._instance.gameObject.SetActive(true);
            }
            else
            {
                InfoPanel._instance.gameObject.SetActive(false);
            }
        }
        currTaskName = menuName;
        currTask = task;
        if (!currTask.IsInit)
        {
            currTask.Init(list, transform.Find("Content/BG"));
        }

        // 动画信息载入
        GlobalData.stepStructs.Clear();
        GlobalData.stepStructs = list;
        GlobalData.canClone = true;

        currTask.Show();
        GameMode.Instance.Prepare(); // Step录入完成后，游戏准备
    }

    public void LoadSceneModel()
    {
        StartCoroutine(LoadModel());
    }

    private IEnumerator LoadModel()
    {
        // 模型场景异步加载
        GameObject obj;
        AsyncOperationHandle<GameObject> model_async = Addressables.LoadAssetAsync<GameObject>(GlobalData.ModelTarget.modelName);
        while (!model_async.IsDone)
        {
            //Debug.Log("proess: " + model_async.PercentComplete.ToString("f6"));
            yield return null;
        }
        obj = Instantiate(model_async.Result);
        obj.name = GlobalData.ModelTarget.modelName;
        Init(GlobalData.currModuleName);
    }

    private void SetActiveMenuList(bool b)
    {
        foreach (var menu in m_Menus)
        {
            menu.gameObject.SetActive(b);
        }
    }
}
