using Cysharp.Threading.Tasks;
using sugar;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 左侧子模块菜单
public class MenuGridPanel : BasePanel
{
    public GameObject m_Item;
    public Transform m_Parent;

    private List<Menu> m_MenuList = new List<Menu>();
    private List<GameObject> m_Items = new List<GameObject>();

    private BaseAction m_currAction;
    private string m_currActName = "";

    // 为了避免重复申请一个子模块的baseaction实例
    private Dictionary<string, BaseAction> m_Actions = new Dictionary<string, BaseAction>();

    public override void Awake()
    {
        base.Awake();
    }

    void Start()
    {
        BuildItem();
        Active(false);
    }

    private void OnDestroy()
    {
        CleaerMenuListItem();
    }

    void BuildItem()
    {
        CleaerMenuListItem();

        ConfigMenuList menulist = ConfigConsole.Instance.FindConfig<ConfigMenuList>();
        m_MenuList = menulist.m_MenuList;
        foreach (Menu menu in m_MenuList)
        {
            if (menu.menuName == GlobalData.currModuleName)
            {
                foreach (var item in menu.subMenuList)
                {
                    GameObject go = Instantiate(m_Item, m_Parent);

                    Button menuBtn = go.transform.GetChild(0).GetComponent<Button>();
                    menuBtn.GetComponentInChildren<TextMeshProUGUI>().text = item.subMenuName;
                    menuBtn.onClick.AddListener(() =>
                    {                       
                        MenuItemClick(item.subMenuName);
                    });
                    m_Items.Add(go);
                    go.gameObject.SetActive(true);
                }
            }
        }
    }

    private void CleaerMenuListItem()
    {
        foreach (var item in m_Items)
        {
            item.gameObject.SetActive(false);
            Destroy(item.gameObject);
        }
        m_Items.Clear();
    }

    public async void MenuItemClick(string name)
    {
        if (m_currAction != null)
        {
            //Debug.Log("m_currAction不是空");
            m_currAction.Exit(() => 
            {
                m_currActName = "";
                m_currAction = null;
            });
        }

        await UniTask.WaitUntil(() => m_currActName == "" && m_currAction == null);
        //Debug.Log("All Clear!");

        m_currActName = name;
        //BaseAction action;// = Tools.CreateObject<BaseAction>(Tools.Escaping(name));
        if (m_Actions.ContainsKey(name))
        {
            m_currAction = m_Actions[name];
        }
        else
        {
            m_currAction = Tools.CreateObject<BaseAction>(Tools.Escaping(name));
            m_Actions.Add(name, m_currAction);
        }
        GlobalData.currItemMode = name;
        //Debug.Log("curraction init finish");
        await m_currAction.AsyncShow(name);
    }
}
