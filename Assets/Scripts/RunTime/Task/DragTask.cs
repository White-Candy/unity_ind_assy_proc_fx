using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class DragTask
{
    private BaseAction currAction;
    private string currTaskName;
    private Dictionary<string, BaseAction> m_StateDic = new Dictionary<string, BaseAction>();
 
    // private List<StepStruct> m_SubMenuList = new List<StepStruct>();

    private SubMenuGrid m_ToolSubMenuGrid; // 显示工具的Layout
    private SubMenuGrid m_MaterialSubMenuGrid; // 显示工具的Layout

    private Transform m_SubMenuGridParent;

    private List<string> m_Tools;
    private List<string> m_Materials;

    private SubMenuDragGrid m_SubMenuDragGrid;

    public bool IsInit { get; protected set; }

    /// <summary>
    /// item1: tools
    /// item2: materials
    /// item3: parents
    /// </summary>
    /// <param name="args"></param>
    public virtual void Init(params object[] args)
    {
        // Debug.Log("DragTask init");
        m_Tools = (List<string>)args[0];
        m_Materials = (List<string>)args[1];
        m_SubMenuGridParent = (Transform)args[2];
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
        m_SubMenuDragGrid = Object.Instantiate(Resources.Load<SubMenuDragGrid>("Prefabs/UI/Control/SubMenuDragGrid"), m_SubMenuGridParent);
        SubMenuGridInit("ToolScrolView", m_Tools, out m_ToolSubMenuGrid, m_SubMenuDragGrid);
        SubMenuGridInit("MaterialScrolView", m_Materials, out m_MaterialSubMenuGrid, m_SubMenuDragGrid);
    }

    private void SubMenuGridInit(string name, List<string> list, out SubMenuGrid grid, SubMenuDragGrid subMenuDragGrid)
    {
        if (name == "ToolScrolView")
            grid = subMenuDragGrid.toolSubMenu;
        else
            grid = subMenuDragGrid.materialSubMenu;
        grid.transform.parent.SetSiblingIndex(1);
        grid.Init(list);
    }

    public virtual void Exit()
    {
        if (m_SubMenuDragGrid) m_SubMenuDragGrid.gameObject.SetActive(false);
        Object.Destroy(m_SubMenuDragGrid);
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
        if (m_ToolSubMenuGrid != null)
        {
            GameObject.DestroyImmediate(m_ToolSubMenuGrid.gameObject);
        }

        if (m_MaterialSubMenuGrid != null)
        {
            GameObject.DestroyImmediate(m_MaterialSubMenuGrid.gameObject);
        }
    }
}
