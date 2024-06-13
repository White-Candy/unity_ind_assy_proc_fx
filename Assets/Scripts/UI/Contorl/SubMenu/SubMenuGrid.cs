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

            foreach(var subMenu in subMenus)
            {
                Debug.Log("subMenuGrid: " + subMenu.subMenuName);
                if (!subMenu.subMenuName.Contains(" æ“‚Õº"))
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
