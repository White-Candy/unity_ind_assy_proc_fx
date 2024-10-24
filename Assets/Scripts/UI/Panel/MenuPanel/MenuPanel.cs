using LitJson;

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using System.Linq;

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
    public GameObject SearchObj;


    //private List<Menu> m_MenuList = new List<Menu>(); // 裏面有需要配置工具的信息
    private string currTaskName; // 目前的Task名字
    //private BaseTask currTask; // 目前的任务实例


    [HideInInspector] public List<GameObject> m_Menus = new List<GameObject>(); // 保存 菜单按钮列表
    private List<GameObject> m_Menulist = new List<GameObject>(); // 保存 菜单列表

    private GameObject currMeunList; //目前打开的菜单列表

    private TMP_InputField m_SearchInputField;
    private Button m_SearchButton;

    public static MenuPanel _instance;

    private MenuGridPanel m_MenuGridPanel;

    public override void Awake()
    {
        base.Awake();
        _instance = this;
    }

    private void Start()
    {
        m_SearchInputField = SearchObj.GetComponentInChildren<TMP_InputField>();
        m_SearchButton = SearchObj.GetComponentInChildren<Button>();
        m_MenuGridPanel = UIConsole.FindPanel<MenuGridPanel>();

        BuildMenuList();

        m_SearchButton.onClick.AddListener(() =>
        {
            string course = m_SearchInputField.text;
            if (string.IsNullOrEmpty(course)) return;

            foreach (var pair in GlobalData.CurrModeMenuList)
            {
                if (pair.Value.FindIndex(x => x == course) != -1)
                {
                    GameObject targetMenu = pair.Key;
                    MeunClick(ref targetMenu);
                }
            }
        });

#if UNITY_WEBGL
        m_SearchInputField.gameObject.SetActive(false);
        m_SearchButton.gameObject.SetActive(false);
#endif
        //Init();
    }

    private async void BuildMenuList()
    {
#if UNITY_STANDALONE_WIN
        await UniTask.WaitUntil(() => { return GlobalData.Projs.Count != 0; });
        if (GlobalData.currModuleName != "考核") NormalBuildMenu();
        else ExamineBuildMenu();
#elif UNITY_WEBGL
        // Debug.Log("BuildMenuList");
        await Utilly.DownLoadTextFromServer(Application.streamingAssetsPath + "\\Config\\WebGLTargetMode.txt", async (text) => 
        {
            string[] split = text.Split("|");
            Debug.Log($"BuildMenuList: {split[0]} | {split[1]}");
            Proj project = new Proj()
            {
                Columns = split[0],
                Courses = new List<string>{split[1]}
            };
            GlobalData.columnsName = split[0];
            GlobalData.courseName = split[1];
            if (GlobalData.currModuleName == "教学") 
            {
                m_MenuGridPanel.Active(true); 
            }
            else if (GlobalData.currModuleName == "考核")
            {
                await UniTask.WaitUntil(() => GlobalData.ExamineesInfo.Count > 0);
                List<ExamineInfo> examinesInfo = GlobalData.ExamineesInfo.FindAll(x => x.ColumnsName == GlobalData.columnsName && x.CourseName == GlobalData.courseName);
                examinesInfo.Sort((ExamineInfo a, ExamineInfo b) => 
                {
                    string[] aSplit = a.RegisterTime.Split("/");
                    string[] bSplit = b.RegisterTime.Split("/");
                    if (aSplit[0] != bSplit[0]) return int.Parse(aSplit[0]) > int.Parse(bSplit[0]) ? -1 : 1;
                    else if (aSplit[1] != bSplit[1]) return int.Parse(aSplit[1]) > int.Parse(bSplit[1]) ? -1 : 1;
                    else return int.Parse(aSplit[2]) > int.Parse(bSplit[2]) ? -1 : 1;
                });
                
                if (examinesInfo.Count > 0) 
                {
                    GlobalData.currExamsInfo = examinesInfo[0];
                    m_MenuGridPanel.Active(true); 
                }
                else UITools.OpenDialog("", "该模块没有考题。", () => { }, true);
            }
            else
            {
                Active(false);
                SetActiveMenuList(false);
                await Tools.LoadSceneModel();
                TitlePanel._instance.SetTitle(split[1]);
            }
        });
#endif
    }

    /// <summary>
    /// 非考核模式Menu的创建
    /// </summary>
    public void NormalBuildMenu()
    {
        GlobalData.CurrModeMenuList.Clear();
        foreach (var proj in GlobalData.Projs)
        {
            GameObject menuItem = Instantiate(m_MenuItem, menuItemParent);
            GameObject list = menuItem.transform.Find("SubMenuGrid").gameObject;

            BuildMenuItem(courses: proj.Courses, list: list);
            list.gameObject.SetActive(false);
            menuItem.gameObject.SetActive(true);

            Button menuBtn = menuItem.transform.GetChild(0).GetComponent<Button>();
            menuBtn.GetComponentInChildren<TextMeshProUGUI>().text = proj.Columns;
            menuBtn.onClick.AddListener(() => { MeunClick(ref menuItem); });

            if (GlobalData.CurrModeMenuList.ContainsKey(menuItem)) GlobalData.CurrModeMenuList[menuItem] = new List<string>(proj.Courses);
            else GlobalData.CurrModeMenuList.Add(menuItem, new List<string>(proj.Courses));
            m_Menus.Add(menuItem);
            m_Menulist.Add(list);
        }
    }

    /// <summary>
    /// 考核模式菜单构建
    /// </summary>
    public void ExamineBuildMenu()
    {
        GlobalData.CurrModeMenuList.Clear();
        foreach (var inf in GlobalData.Projs)
        {
            GameObject menuItem = Instantiate(m_MenuItem, menuItemParent);
            GameObject list = menuItem.transform.Find("SubMenuGrid").gameObject;

            BuildMenuItem(examinees: GlobalData.ExamineesInfo, inf.Columns, list: list);
            list.gameObject.SetActive(false);
            menuItem.gameObject.SetActive(true);

            Button menuBtn = menuItem.transform.GetChild(0).GetComponent<Button>();
            menuBtn.GetComponentInChildren<TextMeshProUGUI>().text = inf.Columns;
            menuBtn.onClick.AddListener(() => { MeunClick(ref menuItem); });

            if (GlobalData.CurrModeMenuList.ContainsKey(menuItem)) GlobalData.CurrModeMenuList[menuItem] = new List<string>(inf.Courses);
            else GlobalData.CurrModeMenuList.Add(menuItem, new List<string>(inf.Courses));
            m_Menus.Add(menuItem);
            m_Menulist.Add(list);
        }           
    }

    /// <summary>
    /// 点击菜单，弹出子菜单列表
    /// </summary>
    /// <param name="menu"></param>
    public void MeunClick(ref GameObject menu)
    {
        GameObject list = menu.transform.Find("SubMenuGrid").gameObject;
        Button menuBtn = menu.transform.GetChild(0).GetComponent<Button>();

        bool b = list.activeSelf;
        GlobalData.columnsName = menuBtn.GetComponentInChildren<TextMeshProUGUI>().text;
        SetActiveMenuItem(list, !b);
    }

    private void BuildMenuItem(List<ExamineInfo> examinees = null, string colName = "", List<string> courses = null, GameObject list = null)
    {
#if UNITY_STANDALONE_WIN
        if (GlobalData.currModuleName != "考核") NormalBuildItem(courses, list: list);
        else ExamsBuildItem(examinees, colName, list: list);
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
                // GlobalData.currModuleName = split[0];
                GlobalData.currExamsInfo = GlobalData.ExamineesInfo.Find(x => x.RegisterTime == split[1] && x.CourseName == split[0]).Clone();
                int scoreIdx = GlobalData.scoresInfo.FindIndex(x => x.className == GlobalData.usrInfo.UnitName && x.userName == GlobalData.usrInfo.userName
                            && x.registerTime == GlobalData.currExamsInfo.RegisterTime && x.columnsName == GlobalData.currExamsInfo.ColumnsName 
                            && x.courseName == GlobalData.currExamsInfo.CourseName);
                if (scoreIdx == -1)
                {
                    ScoreInfo inf = new ScoreInfo()
                    {
                        className = GlobalData.usrInfo.UnitName,
                        columnsName = GlobalData.currExamsInfo.ColumnsName,
                        courseName = GlobalData.currExamsInfo.CourseName,
                        registerTime = GlobalData.currExamsInfo.RegisterTime,
                        userName = GlobalData.usrInfo.userName,
                        Name = GlobalData.usrInfo.Name,
                    };
                    GlobalData.currScoreInfo = inf.Clone();
                }
                else GlobalData.currScoreInfo = GlobalData.scoresInfo[scoreIdx].Clone();              
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
            m_MenuGridPanel.Active(true);
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
