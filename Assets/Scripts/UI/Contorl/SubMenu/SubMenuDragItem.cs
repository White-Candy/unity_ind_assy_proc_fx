using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;

// 可拖动的Item
public class SubMenuDragItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private string m_Name;
    private int enumId;

    [HideInInspector]
    public float score;

    [HideInInspector]
    public float currScore;

    private GameObject m_ItemModel = null;

    //private List<string> stepNameList = new List<string>(); // 模型步骤名称

    public void Init(string name, int enumID = 0)
    {
        transform.GetComponentInChildren<TextMeshProUGUI>().text = name;
        m_Name = name;
        enumId = enumID;
    }

    /// <summary>
    /// 开始拖拽从资源文件中生成模型到场景中
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnBeginDrag(PointerEventData eventdata)
    {
        //Debug.Log("OnBeginDrag: " + m_Name);
        if (m_ItemModel == null)
        {
            if (Camera.main == null) return;
            string path = "Prefabs/Model/" + GlobalData.ModelTarget.modelName + "/" + m_Name;
            //Debug.Log(path);
            m_ItemModel = Instantiate(Resources.Load<GameObject>(path));
            m_ItemModel.name = m_Name;
            //Debug.Log("m_ItemModel: " + m_ItemModel.name);
        }
    }

    /// <summary>
    /// 拖拽时要让模型位置始终跟随鼠标
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnDrag(PointerEventData eventdata)
    {
        //Debug.Log("OnDrag: " + m_Name);
        if (m_ItemModel != null)
        {
            m_ItemModel.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1f));
        }
    }

    /// <summary>
    /// 拖拽结束进行射线检测，是否拖拽到了指定的Object上
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnEndDrag(PointerEventData eventdata)
    {
       // Debug.Log("OnEndDrag: " + m_Name);
        if (m_ItemModel != null)
        {
            RaycastHit hit;
            
            //开启射线检测
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                //Debug.Log("m_itemMode name: " + m_ItemModel.name + " || " + "hit name: " + hit.collider.name);
                GameMode.Instance.SetToolName(m_ItemModel.name);
            }         
            Destroy(m_ItemModel);
        }           
    }
}
