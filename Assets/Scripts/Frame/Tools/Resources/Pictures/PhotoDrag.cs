using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PhotoDrag : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    private Transform m_ImageTransform;
    private float m_MaxScale = 3;
    private float m_MinScale = 1;

    private RectTransform rt;
    private Vector3 offset = Vector3.zero;

    private float minX, maxX, minY, maxY;

    private void Awake()
    {
        m_ImageTransform = this.transform;
    }

    public void Start()
    {
        rt = this.transform.GetComponent<RectTransform>();
        
        //初始化当前图片大小
        m_ImageTransform.localScale = Vector3.one;
    }

    private void OnEnable()
    {
        //初始化当前图片大小
        m_ImageTransform.localScale = Vector3.one;
    }

    // Update is called once per frame
    void Update()
    {

        if ( Input.GetAxis("Mouse ScrollWheel") > 0 )
            m_ImageTransform.transform.localScale += ( m_ImageTransform.localScale.x >= m_MaxScale ? Vector3.zero : Vector3.one * 0.1f );
        else if ( Input.GetAxis("Mouse ScrollWheel") < 0 )
            m_ImageTransform.transform.localScale += ( m_ImageTransform.localScale.x <= m_MinScale ? Vector3.zero : Vector3.one * -0.1f );

        SetDragRange();
        rt.position = DragRangeLimit(rt.position);


    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if ( RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.enterEventCamera, out Vector3 globalMousePos))
        {
            offset = rt.position - globalMousePos;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if ( RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out Vector3 globalMousePos))
        {
            //Debug.LogFormat("globalMousePos : {0} , offset ：{1}", globalMousePos, offset);
            SetDragRange();
            rt.position = DragRangeLimit(globalMousePos + offset);
        }
    }


    public void SetDragRange()
    {
        float width_half = Screen.width / 2; //960 
        float height_half = Screen.height / 2;//720
        maxX = width_half * rt.transform.localScale.x;
        minX = width_half - (( width_half * rt.transform.localScale.x) - width_half);
        maxY = height_half * rt.transform.localScale.x;
        minY = height_half - (( height_half * rt.transform.localScale.x) - height_half);
    }


    public Vector3 DragRangeLimit(Vector3 pos)
    {
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        // Debug.LogFormat("POS.X : {0} , POS.Y : {1} ", pos.x,pos.y);
        return pos;
    }
    /// <summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        //throw new System.NotImplementedException();
    }

}

