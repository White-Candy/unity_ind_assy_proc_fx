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
            m_DragItem = transform.GetChild(1).GetComponent<SubMenuDragItem>();
        }

        // ÿ�����ߙ���Item�Č���������SubMenus������Ҫ�õ��Ĺ�����Ϣ
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
