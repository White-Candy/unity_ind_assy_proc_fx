using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenuGrid : MonoBehaviour
{
    private SubMenuItem m_Item;
    private SubMenuDragItem m_DragItem;

    public void Init(List<string> equs)
    {
        if (m_DragItem == null)
        {
            m_DragItem = transform.GetChild(1).GetComponent<SubMenuDragItem>();
        }

        // 每個工具欄中Item的實例創建，SubMenus就是需要用到的工具信息
        foreach (var item in equs)
        {       
            SubMenuDragItem drag_item = Instantiate(m_DragItem, this.transform);
            drag_item.gameObject.GetComponent<Image>().sprite =
                Resources.Load<Sprite>("Textures/Tools/" + item);
            drag_item.gameObject.SetActive(true);
            drag_item.Init(item);
        }
    }
}
