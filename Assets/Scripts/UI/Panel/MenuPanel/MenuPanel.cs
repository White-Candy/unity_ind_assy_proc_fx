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
using Unity.VisualScripting;

public struct StepStruct
{
    public string method; // 方法[拖拽 or 点击]
    public List<string> tools; // 工具
    public string stepName; // 步骤名称
    public List<string> animLimite; // 该步骤的动画帧数范围
    public Transform arrowTrans; // 箭头预制体每个步骤的位置
    public AudioClip clip; // 每个步骤的声音提示
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


    [HideInInspector] public List<GameObject> m_Menus = new List<GameObject>(); // 保存 菜单按钮列表
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

    private async void BuildMenuList()
    {
        await UniTask.WaitUntil(() => 
        {
            // Debug.Log($"wait at a time: {GlobalData.Projs.Count}");
            return GlobalData.Projs.Count != 0;
        });
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        if (GlobalData.currModuleName != "考核") NormalBuildMenu();
        else ExamineBuildMenu();
#elif UNITY_WEBGL
        var proj = GlobalData.Projs[0];
        BuildMenuItem(proj.targets);
#endif
    }

    /// <summary>
    /// 非考核模式Menu的创建
    /// </summary>
    public void NormalBuildMenu()
    {
        foreach (var proj in GlobalData.Projs)
        {
            GameObject menuItem = Instantiate(m_MenuItem, menuItemParent);
            GameObject list = menuItem.transform.Find("SubMenuGrid").gameObject;

            BuildMenuItem(courses: proj.Courses, list: list);
            list.gameObject.SetActive(false);
            menuItem.gameObject.SetActive(true);

            Button menuBtn = menuItem.transform.GetChild(0).GetComponent<Button>();
            menuBtn.GetComponentInChildren<TextMeshProUGUI>().text = proj.Columns;
            menuBtn.onClick.AddListener(() => 
            {
                bool b = list.activeSelf;
                GlobalData.columnsName = menuBtn.GetComponentInChildren<TextMeshProUGUI>().text;
                SetActiveMenuItem(list, !b);
            });

            m_Menus.Add(menuItem);
            m_Menulist.Add(list);
        }        
    }

    /// <summary>
    /// 考核模式菜单构建
    /// </summary>
    public void ExamineBuildMenu()
    {
        foreach (var inf in GlobalData.ExamineesInfo)
        {
            if (inf.Status == false) continue;
            GameObject menuItem = Instantiate(m_MenuItem, menuItemParent);
            GameObject list = menuItem.transform.Find("SubMenuGrid").gameObject;

            BuildMenuItem(examinees: GlobalData.ExamineesInfo, inf.ColumnsName, list: list);
            list.gameObject.SetActive(false);
            menuItem.gameObject.SetActive(true);

            Button menuBtn = menuItem.transform.GetChild(0).GetComponent<Button>();
            menuBtn.GetComponentInChildren<TextMeshProUGUI>().text = inf.ColumnsName;
            menuBtn.onClick.AddListener(() => 
            {
                bool b = list.activeSelf;
                GlobalData.columnsName = menuBtn.GetComponentInChildren<TextMeshProUGUI>().text;
                SetActiveMenuItem(list, !b);
            });

            m_Menus.Add(menuItem);
            m_Menulist.Add(list);
        }           
    }

    private void BuildMenuItem(List<ExamineInfo> examinees = null, string colName = "", List<string> courses = null, GameObject list = null)
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        if (GlobalData.currModuleName != "考核") NormalBuildItem(courses, list: list);
        else ExamsBuildItem(examinees, colName, list: list);
#elif UNITY_WEBGL
        // 单模块模式
        if (targets.Count > 0)
        {
            var tar = targets[0];
            GlobalData.ModelTarget = tar;
            GlobalData.currModuleCode = tar.modelCode.ToString();
            if (GlobalData.isLoadModel)
            {
                await Tools.LoadSceneModel();
                SetActiveMenuList(false);
                TitlePanel._instance.SetTitle(tar.menuName);
                Active(false);
            }
            else
            {
                // 左侧子模块菜单
                Active(true);                
                Debug.Log("GlobalData.currModuleCode: " + GlobalData.currModuleCode);
            }
        }
#endif
    }


    /// <summary>
    /// 非考核模式的关于 Menu Item的创建
    /// </summary>
    /// <param name="courses"></param>
    /// <param name="list"></param>
    public void NormalBuildItem(List<string> courses, GameObject list = null)
    {
        // 多模块模式
        foreach (var course in courses)
        {
            GameObject item = Instantiate(m_Item, list.transform);
            item.gameObject.SetActive(true);
            Button itemBtn = item.transform.GetChild(0).GetComponent<Button>();
            //itemBtn.GetComponentInChildren<TextMeshProUGUI>().text = course;
            itemBtn.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = course;
            itemBtn.onClick.AddListener(() => { ChooseThisItem(course, list); });
        }
    }
    
    /// <summary>
    /// 考核模式Menu的Item的创建
    /// </summary>
    /// <param name="examinees"></param>
    public void ExamsBuildItem(List<ExamineInfo> examinees, string parentCol, GameObject list = null)
    {
        // 多模块模式
        foreach (var exams in examinees)
        {
            if (exams.Status == false || parentCol != exams.ColumnsName) continue;
            GameObject item = Instantiate(m_Item, list.transform);
            item.gameObject.SetActive(true);
            Button itemBtn = item.transform.GetChild(0).GetComponent<Button>();
            //itemBtn.GetComponentInChildren<TextMeshProUGUI>().text = course;
            string name = $"{exams.CourseName}\n{exams.RegisterTime}";
            itemBtn.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = name;
            itemBtn.onClick.AddListener(() => 
            {
                string name = itemBtn.transform.Find("Name").GetComponent<TextMeshProUGUI>().text;
                string[] split = name.Split("\n");
                GlobalData.currModuleName = split[0];
                GlobalData.currExamsTime = split[1];
                GlobalData.currExamsInfo = GlobalData.ExamineesInfo.Find(x => x.RegisterTime == GlobalData.currExamsTime && x.CourseName == GlobalData.currModuleName).Clone();
                ChooseThisItem(exams.CourseName, list); 
            });
        }        
    }

    /// <summary>
    /// 训练模式：异步加载模型场景切换
    /// 其他模式：显示菜单
    /// </summary>
    /// <param name="course"> 课程名 </param>
    /// <param name="obj"> 菜单窗口的实例 </param>
    private async void ChooseThisItem(string course, GameObject obj)
    {
        GlobalData.courseName = course;

        if (GlobalData.isLoadModel)
        {
            await Tools.LoadSceneModel();
            SetActiveMenuList(false);
            TitlePanel._instance.SetTitle(course);
            Active(false);
        }
        else
        {
            // 左侧子模块菜单
            Active(true);
            MenuGridPanel._instance.Active(true);
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

    public void SetActiveMenuList(bool b)
    {
        foreach (var menu in m_Menus)
        {
            if (menu == null) continue;
            menu.gameObject.SetActive(b);
        }
    }
}
