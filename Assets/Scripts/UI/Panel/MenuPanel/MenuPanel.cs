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
    //private BaseTask currTask; // 目前的任务实例

    //private Dictionary<string, BaseTask> m_TaskDic = new Dictionary<string, BaseTask>(); // 任务字典

    private List<GameObject> m_Menus = new List<GameObject>(); // 保存 菜单按钮列表
    private List<GameObject> m_Menulist = new List<GameObject>(); // 保存 菜单列表

    private GameObject currMeunList; //目前打开的菜单列表

    public static MenuPanel _instance;

    public override void Awake()
    {
        base.Awake();
        _instance = this;
    }

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

        if (GlobalData.isLoadModel)
        {
            await Tools.LoadSceneModel();
            SetActiveMenuList(false);
        }
        else
        {
            // 左侧子模块菜单
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

    private void SetActiveMenuList(bool b)
    {
        foreach (var menu in m_Menus)
        {
            menu.gameObject.SetActive(b);
        }
    }
}
