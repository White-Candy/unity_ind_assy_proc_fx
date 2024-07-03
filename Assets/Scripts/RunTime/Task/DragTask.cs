using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DragTask
{
    protected BaseAction currAction;
    protected string currTaskName;
    protected Dictionary<string, BaseAction> m_StateDic = new Dictionary<string, BaseAction>();

    protected List<StepStruct> m_SubMenuList = new List<StepStruct>();
    protected SubMenuGrid m_SubMenuGridObj; // 显示工具的Layout
    protected Transform m_SubMenuGridParent;

    public bool IsInit { get; protected set; }

    public virtual void Init(params object[] args)
    {
        m_SubMenuList = (List<StepStruct>)args[0];
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
        m_SubMenuGridObj = Object.Instantiate(Resources.Load<GameObject>("Prefabs/UI/Control/SubMenuDragGrid"),
                m_SubMenuGridParent).gameObject.transform.Find("Scroll View/Viewport/SubMenuDragGrid").GetComponent<SubMenuGrid>();
        m_SubMenuGridObj.gameObject.transform.parent.SetSiblingIndex(1);
        m_SubMenuGridObj.Init(m_SubMenuList);
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
            currAction.Exit(() => { });
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
