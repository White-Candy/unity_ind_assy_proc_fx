using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenuGrid : MonoBehaviour
{
    private SubMenuItem m_Item;
    private SubMenuDragItem m_DragItem;
    private List<string> m_Tools = new List<string>(); // 保存已经生成过的工具名字

    public Action<string, int> OnSubBtnClicked = (a, b) => { };

    public void Init(List<StepStruct> subMenus, bool drag = false)
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
                foreach (var tool in subMenu.tools)
                {
                    if (m_Tools.Contains(tool))
                    {
                        continue; // 如果工具栏已经生成过了这个道具 那么不在生成了
                    }
                    SubMenuDragItem drag_item = Instantiate(m_DragItem, this.transform);
                    drag_item.gameObject.GetComponent<Image>().sprite =
                        Resources.Load<Sprite>("Textures/Tools/" + tool);
                    drag_item.gameObject.SetActive(true);
                    drag_item.Init(tool);
                    m_Tools.Add(tool);
                }
            }
        }
    }
}
