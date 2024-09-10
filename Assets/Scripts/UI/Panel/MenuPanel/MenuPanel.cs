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
    public string method; // ����[��ק or ���]
    public List<string> tools; // ����
    public string stepName; // ��������
    public List<string> animLimite; // �ò���Ķ���֡����Χ
    public Transform arrowTrans; // ��ͷԤ����ÿ�������λ��
    public AudioClip clip; // ÿ�������������ʾ
}


// �ˆ����
public class MenuPanel : BasePanel
{
    public GameObject m_MenuItem; 
    public GameObject m_Item; // �˵��б��е�Item

    public Transform menuItemParent;

    //private List<Menu> m_MenuList = new List<Menu>(); // �Y������Ҫ���ù��ߵ���Ϣ
    private string currTaskName; // Ŀǰ��Task����
    //private BaseTask currTask; // Ŀǰ������ʵ��


    [HideInInspector] public List<GameObject> m_Menus = new List<GameObject>(); // ���� �˵���ť�б�
    private List<GameObject> m_Menulist = new List<GameObject>(); // ���� �˵��б�

    private GameObject currMeunList; //Ŀǰ�򿪵Ĳ˵��б�

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
        if (GlobalData.currModuleName != "����") NormalBuildMenu();
        else ExamineBuildMenu();
#elif UNITY_WEBGL
        var proj = GlobalData.Projs[0];
        BuildMenuItem(proj.targets);
#endif
    }

    /// <summary>
    /// �ǿ���ģʽMenu�Ĵ���
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
    /// ����ģʽ�˵�����
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
        if (GlobalData.currModuleName != "����") NormalBuildItem(courses, list: list);
        else ExamsBuildItem(examinees, colName, list: list);
#elif UNITY_WEBGL
        // ��ģ��ģʽ
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
                // �����ģ��˵�
                Active(true);                
                Debug.Log("GlobalData.currModuleCode: " + GlobalData.currModuleCode);
            }
        }
#endif
    }


    /// <summary>
    /// �ǿ���ģʽ�Ĺ��� Menu Item�Ĵ���
    /// </summary>
    /// <param name="courses"></param>
    /// <param name="list"></param>
    public void NormalBuildItem(List<string> courses, GameObject list = null)
    {
        // ��ģ��ģʽ
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
    /// ����ģʽMenu��Item�Ĵ���
    /// </summary>
    /// <param name="examinees"></param>
    public void ExamsBuildItem(List<ExamineInfo> examinees, string parentCol, GameObject list = null)
    {
        // ��ģ��ģʽ
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
    /// ѵ��ģʽ���첽����ģ�ͳ����л�
    /// ����ģʽ����ʾ�˵�
    /// </summary>
    /// <param name="course"> �γ��� </param>
    /// <param name="obj"> �˵����ڵ�ʵ�� </param>
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
            // �����ģ��˵�
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
