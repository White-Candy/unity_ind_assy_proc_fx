using sugar;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MenuGridPanel : BasePanel
{
    public GameObject m_Item;
    public Transform m_Parent;

    private List<Menu> m_MenuList = new List<Menu>();
    private List<GameObject> m_Items = new List<GameObject>();
    void Start()
    {
        BuildItem();
        if (GlobalData.currModuleName == "ÑµÁ·")
        {
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
        }
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
                        Debug.Log(item.subMenuName);
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
}
