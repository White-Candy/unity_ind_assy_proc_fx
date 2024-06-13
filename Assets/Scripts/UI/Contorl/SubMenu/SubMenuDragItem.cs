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

    private List<string> stepNameList = new List<string>(); // 模型步骤名称

    public void Init(string name, int enumID = 0)
    {
        transform.GetComponentInChildren<TextMeshProUGUI>().text = (name.Split('_').Length > 1 ? name.Split('_')[0] : name);
        m_Name = name;
        enumId = enumID;

        if (name.Split('_').Length > 1)
        {
            string[] strs = name.Split('_');
            for (int i = 1; i < strs.Length; i++)
            {
                stepNameList.Add(strs[i]);
            }
        }
    }

    /// <summary>
    /// 开始拖拽从资源文件中生成模型到场景中
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnBeginDrag(PointerEventData eventdata)
    {
        Debug.Log("OnBeginDrag: " + m_Name);
        if (m_ItemModel == null)
        {
            if (Camera.main == null) return;
            m_ItemModel = Instantiate(Resources.Load<GameObject>("Prefabs/Model/" + (m_Name.Split('_').Length > 1 ? m_Name.Split('_')[0] : m_Name)));
            m_ItemModel.name = m_Name;
            Debug.Log("m_ItemModel: " + m_ItemModel.name);
        }
    }

    /// <summary>
    /// 拖拽时要让模型位置始终跟随鼠标
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnDrag(PointerEventData eventdata)
    {
        Debug.Log("OnDrag: " + m_Name);
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
        Debug.Log("OnEndDrag: " + m_Name);
        if (m_ItemModel != null)
        {
            RaycastHit hit;
            
            //开启射线检测
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                Debug.Log("播放动画 = " + m_ItemModel.name + "，步骤列表:" + string.Join(",", stepNameList));
                if (hit.collider.name == m_ItemModel.name || stepNameList.Contains(hit.collider.name))
                {
                    // TODO..
                    if (GlobalData.mode == Mode.Practice)
                    {
                        //RoamView._Instance.PlayAnimation(hit.collider.name);
                    }
                    else
                    {
                        //currScore = score;
                        //ModelAnimControl._Instance.totalScore += currScore;
                        //ModelAnimControl._Instance.PlayAnimation(hit.collider.name, () =>
                        //{
                        //    ModelAnimControl._Instance.SetCurrentCamera();
                        //});
                    }
                    //ModelAnimControl._Instance.SetNextStep();
                }
                else
                {
                    if (GlobalData.mode == Mode.Examination)
                    {
                        currScore = 0;
                    }
                }
            }         
            Destroy(m_ItemModel);
        }           
    }
}
