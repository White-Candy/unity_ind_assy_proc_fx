using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenuGrid : MonoBehaviour
{
    private SubMenuItem m_Item;
    private SubMenuDragItem m_DragItem;

    public Action<string, int> OnSubBtnClicked = (a, b) => { };

    public void Init(List<SubMenu> subMenus, bool drag = false)
    {
        if (!drag)
        {

        }
        else
        {
            if (m_DragItem == null)
            {
                m_DragItem = transform.GetChild(1).GetComponent<SubMenuDragItem>();
            }

            // 每個工具欄中Item的實例創建，SubMenus就是需要用到的工具信息
            foreach(var subMenu in subMenus)
            {
                //Debug.Log("subMenuGrid: " + subMenu.subMenuName);
                if (!subMenu.subMenuName.Contains("示意图"))
                {
                    SubMenuDragItem drag_item = Instantiate(m_DragItem, this.transform);
                    drag_item.gameObject.GetComponent<Image>().sprite = 
                        Resources.Load<Sprite>("Textures/Tools/" + 
                        (subMenu.subMenuName.Split('_').Length > 1 ? subMenu.subMenuName.Split('_')[0] : subMenu.subMenuName));
                    drag_item.gameObject.SetActive(true);
                    drag_item.Init(subMenu.subMenuName, subMenu.enumID);
                }
            }
        }
    }
}
