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

        // ÿ�����ߙ���Item�Č���������SubMenus������Ҫ�õ��Ĺ�����Ϣ
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
