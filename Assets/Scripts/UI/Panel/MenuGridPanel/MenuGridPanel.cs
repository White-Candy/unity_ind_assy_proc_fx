using sugar;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class MenuGridPanel : Singleton<MenuGridPanel>
{
    public GameObject m_Item;
    public Transform m_Parent;

    private List<Menu> m_MenuList = new List<Menu>();
    private List<GameObject> m_Items = new List<GameObject>();

    private BaseAction m_currAction;
    private string m_currActName = "";

    void Start()
    {
        BuildItem();
        gameObject.SetActive(false);
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
        if (m_currActName == name)
        {
            return;
        }

        if (m_currAction != null)
        {
            m_currAction.Exit();
        }

        BaseAction action = Tools.CreateObject<BaseAction>(Tools.Escaping(name));
        m_currActName = name;
        m_currAction = action;
        await action.AsyncShow(name);
    }
}
