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

    //private Dictionary<string, BaseTask> m_TaskDic = new Dictionary<string, BaseTask>(); // �����ֵ�

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

    private void BuildMenuList()
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
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
#elif UNITY_WEBGL
        var proj = GlobalData.Projs[0];
        BuildMenuItem(proj.targets);
#endif
    }

    private void BuildMenuItem(List<Target> targets, GameObject list = null)
    {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        // ��ģ��ģʽ
        foreach (var target in targets)
        {
            GameObject item = Instantiate(m_Item, list.transform);
            item.gameObject.SetActive(true);
            Button itemBtn = item.transform.GetChild(0).GetComponent<Button>();
            itemBtn.GetComponentInChildren<TextMeshProUGUI>().text = target.menuName;
            itemBtn.onClick.AddListener(() => { ChooseThisItem(target, list); });
        }
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
    /// ѵ��ģʽ���첽����ģ�ͳ����л�
    /// ����ģʽ����ʾ�˵�
    /// </summary>
    /// <param name="target"> ����Ŀ��info </param>
    /// <param name="obj"> �˵����ڵ�ʵ�� </param>
    private async void ChooseThisItem(Target target, GameObject obj)
    {
        GlobalData.ModelTarget = target;
        GlobalData.currModuleCode = target.modelCode.ToString();
        //GlobalData.currModuleName = target.modelName;

        if (GlobalData.isLoadModel)
        {
            await Tools.LoadSceneModel();
            SetActiveMenuList(false);
            TitlePanel._instance.SetTitle(target.menuName);
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
