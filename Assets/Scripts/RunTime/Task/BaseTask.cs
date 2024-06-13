using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public abstract class BaseTask
{
    protected BaseAction currAction;
    protected string currTaskName;
    protected Dictionary<string, BaseAction> m_StateDic = new Dictionary<string, BaseAction>();

    protected List<SubMenu> m_SubMenuList = new List<SubMenu>();
    protected SubMenuGrid m_SubMenuGridObj; // 显示工具的Layout
    protected Transform m_SubMenuGridParent;

    public bool m_Drag = false;

    public bool IsInit { get; protected set; }

    protected abstract void OnSubMenuBtnClicked(string name, int id);

    public virtual void Init(params object[] args)
    {
        m_SubMenuList = (List<SubMenu>)args[0];
        m_SubMenuGridParent = (Transform)args[1];
        IsInit = true;
    }

    public virtual void Show()
    {
        SpawnSubMenus();
    }

    /// <summary>
    /// 将存放ToolsItem的面板控件，从资源中生成到场景中
    /// </summary>
    protected virtual void SpawnSubMenus()
    {
        if (!m_Drag)
        {
            // TODO..
        }
        else
        {
            m_SubMenuGridObj = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/UI/Control/SubMenuDragGrid"),
                m_SubMenuGridParent).gameObject.transform.Find("Scroll View/Viewport/SubMenuDragGrid").GetComponent<SubMenuGrid>();
            m_SubMenuGridObj.gameObject.transform.parent.SetSiblingIndex(1);
            m_SubMenuGridObj.OnSubBtnClicked = OnSubMenuBtnClicked;
            m_SubMenuGridObj.Init(m_SubMenuList, m_Drag);
        }
    }

    public virtual void Exit()
    {
        ClearData();
        DestoryGridObj();
    }

    protected virtual void ClearData()
    {
        if (currAction != null)
        {
            currAction.Exit();
        }
        currTaskName = "";
        currAction = null;
    }


    /// <summary>
    /// 销毁 工具Item的Layout
    /// </summary>
    protected virtual void DestoryGridObj()
    {
        if (m_SubMenuGridObj != null)
        {
            GameObject.DestroyImmediate(m_SubMenuGridObj.gameObject);
        }
    }
}
