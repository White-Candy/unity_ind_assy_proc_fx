using EPOOutline;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Outlinable))]
public class ModelPartHighlightCol : MonoBehaviour
{
    private Outlinable m_Outlinable;
    Color m_OutlinableInitColor;

    
    private void Awake()
    {

    }

    public void Start()
    {
        m_Outlinable = GetComponent<Outlinable>();
        m_Outlinable.enabled = false;

        m_OutlinableInitColor = m_Outlinable.OutlineParameters.Color;
    }

    /// <summary>
    /// 开始高亮
    /// </summary>
    public void OnHighlight()
    { 
        m_Outlinable.enabled = true;
        //m_Outlinable.OutlineParameters.DOColor(Color.red, .5f).SetLoops(3, LoopType.Yoyo).onComplete += () =>
        //{
        //    m_Outlinable.enabled = false;
        //    m_Outlinable.OutlineParameters.Color = m_OutlinableInitColor;
        //};
    }

    /// <summary>
    /// 停止高亮
    /// </summary>
    public void OffHighlight()
    { 
        m_Outlinable.enabled = false;
       // /*int a = */m_Outlinable.OutlineParameters.DOKill(true);
        //Debug.Log(a);
        m_Outlinable.OutlineParameters.Color = m_OutlinableInitColor;
    }

    private void OnDestroy()
    {
      // m_Outlinable.OutlineParameters.DOKill(true);
    }
}
