using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenuGrid : MonoBehaviour
{
    private SubMenuDragItem m_DragItem;

    public void Init(List<string> equs)
    {
        if (m_DragItem == null)
        {
            m_DragItem = transform.GetChild(0).GetComponent<SubMenuDragItem>();
        }

        // 每個工具欄中Item的實例創建，SubMenus就是需要用到的工具信息
        foreach (var item in equs)
        {       
            SubMenuDragItem dragItem = Instantiate(m_DragItem, this.transform);
            // dragItem.transform.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/Tools/{item}");
            Image img = dragItem.transform.Find("Icon").GetComponent<Image>();
            UITools.SetImage(ref img, $"Textures/Tools/{item}");
            dragItem.gameObject.SetActive(true);
            dragItem.Init(item);
        }
    }
}
