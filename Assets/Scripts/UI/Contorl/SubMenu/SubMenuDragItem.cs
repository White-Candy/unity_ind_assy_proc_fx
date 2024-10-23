using Cysharp.Threading.Tasks;

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.SocialPlatforms.Impl;

// ���϶���Item
public class SubMenuDragItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private string m_Name;
    private int enumId;

    [HideInInspector]
    public float score;

    [HideInInspector]
    public float currScore;

    private GameObject m_ItemModel = null;

    //private List<string> stepNameList = new List<string>(); // ģ�Ͳ�������

    public void Init(string name, int enumID = 0)
    {
        transform.GetComponentInChildren<TextMeshProUGUI>().text = name;
        m_Name = name;
        enumId = enumID;
    }

    /// <summary>
    /// ��ʼ��ק����Դ�ļ�������ģ�͵�������
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnBeginDrag(PointerEventData eventdata)
    {
        //Debug.Log("OnBeginDrag: " + m_Name);
        LoadModelAsync();
    }

    /// <summary>
    /// ��קʱҪ��ģ��λ��ʼ�ո������
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
    /// ��ק�����������߼�⣬�Ƿ���ק����ָ����Object��
    /// </summary>
    /// <param name="eventdata"></param>
    public void OnEndDrag(PointerEventData eventdata)
    {
       // Debug.Log("OnEndDrag: " + m_Name);
        if (m_ItemModel != null)
        {
            RaycastHit hit;
            
            //�������߼��
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                //Debug.Log("m_itemMode name: " + m_ItemModel.name + " || " + "hit name: " + hit.collider.name);
                if (hit.collider.name == "Arrow")
                {
                    GameMode.Instance.SetToolName(m_ItemModel.name);
                }
            }         
            Destroy(m_ItemModel);
        }           
    }

    /// <summary>
    /// ��ģ���첽���ص�������
    /// </summary>
    public async void LoadModelAsync()
    {
        if (m_ItemModel == null)
        {
            if (Camera.main == null) return;
            var go = await Addressables.LoadAssetAsync<GameObject>(m_Name);
            m_ItemModel = Instantiate(go);
            m_ItemModel.name = m_Name;
            //Debug.Log("m_ItemModel: " + m_ItemModel.name);
        }
    }
}
