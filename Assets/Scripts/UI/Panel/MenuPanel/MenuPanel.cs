using LitJson;
using sugar;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

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
    public GameObject m_Item; // 菜单列表中的Item

    public Transform menuItemParent;

    //private List<Menu> m_MenuList = new List<Menu>(); // 裏面有需要配置工具的信息
    private string currTaskName; // 目前的Task名字
    private BaseTask currTask; // 目前的任务实例

    private Dictionary<string, BaseTask> m_TaskDic = new Dictionary<string, BaseTask>(); // 任务字典

    private List<GameObject> m_Menus = new List<GameObject>(); // 保存菜单按钮列表
    private List<GameObject> m_Menulist = new List<GameObject>(); // 保存菜单列表

    private GameObject currMeunList; //目前打开的菜单列表

    private void Start()
    {
        BuildMenuList();
        //Init();
    }

    private void BuildMenuList()
    {
        foreach (var proj in GlobalData.Projs)
        {
            GameObject menuItem = Instantiate(m_MenuItem, menuItemParent);
            GameObject list = menuItem.transform.Find("SubMenuGrid").gameObject;

            BuildMenuItem(proj.targets, list);
            list.gameObject.SetActive(false);
            menuItem.gameObject.SetActive(true);

            Button menuBtn = menuItem.transform.GetChild(0).GetComponent<Button>();
            menuBtn.GetComponentInChildren<TextMeshProUGUI>().text = proj.ProjName;
            menuBtn.onClick.AddListener(() => 
            {
                bool b = list.activeSelf;
                SetActiveMenuItem(list, !b);
            });

            m_Menus.Add(menuItem);
            m_Menulist.Add(list);
        }
    }

    private void BuildMenuItem(List<Target> targets, GameObject list)
    {
        foreach (var target in targets)
        {
            GameObject item = Instantiate(m_Item, list.transform);
            item.gameObject.SetActive(true);
            Button itemBtn = item.transform.GetChild(0).GetComponent<Button>();
            itemBtn.GetComponentInChildren<TextMeshProUGUI>().text = target.menuName;
            itemBtn.onClick.AddListener(() => { ChooseThisItem(target, list); });
        }
    }

    /// <summary>
    /// 训练模式：异步加载模型场景切换
    /// 其他模式：显示菜单
    /// </summary>
    /// <param name="target"> 子项目的info </param>
    /// <param name="obj"> 菜单窗口的实例 </param>
    private async void ChooseThisItem(Target target, GameObject obj)
    {
        GlobalData.ModelTarget = target;
        GlobalData.currModuleCode = target.modelCode.ToString();
        //GlobalData.currModuleName = target.modelName;

        if (GlobalData.currModuleName == "训练")
        {
            await LoadSceneModel();
        }
        else
        {
            MenuGridPanel.Instance.gameObject.SetActive(true);
        }
        SetActiveMenuItem(obj, false);
    }

    private void SetActiveMenuItem(GameObject menu, bool b)
    {
        if (currMeunList != null)
        {
            currMeunList.SetActive(false);
        }
        currMeunList = menu;
        currMeunList.SetActive(b);
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

    // TODO.. 修改这个函数
    private void SpawnTask(string menuName, List<StepStruct> list)
    {
        if (currTaskName == menuName)
        {
            Debug.Log($"重复进入 ： {menuName}");
            return;
        }

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

                    // 动画信息载入
                    GlobalData.stepStructs.Clear();
                    GlobalData.stepStructs = list;
                    GlobalData.canClone = true;
                    GameMode.Instance.Prepare(); // Step录入完成后，游戏准备
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
        currTask.Show();
    }

 
    public async UniTask LoadSceneModel()
    {
        GlobalData.DestroyModel = false;
        await LoadModel();
    }

    private async UniTask LoadModel()
    {
        // 模型场景异步加载
        GameObject obj;        
        AsyncOperationHandle<GameObject> model_async = Addressables.LoadAssetAsync<GameObject>(GlobalData.ModelTarget.modelName);
        await UniTask.WaitUntil(() => model_async.IsDone == true);

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
