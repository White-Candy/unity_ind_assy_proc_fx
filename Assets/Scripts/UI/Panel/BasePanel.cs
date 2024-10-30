using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour, IBasePanel
{
    //Panel Name
    [HideInInspector]
    public string m_NameP;

    // Panel 强引用
    public GameObject m_Content;

    protected bool m_Visible;

    public virtual void Awake()
    {
        m_NameP = this.GetType().ToString();
        UIConsole.AddPanel(m_NameP, this);
        m_Visible = m_Content == null ? false : m_Content.activeSelf;
    }

    public void Start()
    {

    }

    /// <summary>
    /// 面板的显示控制
    /// </summary>
    /// <param name="b"></param>
    public virtual void Active(bool b)
    {
        // m_Visible = b;
        if (m_Content != null)
        {
            m_Visible = b;
            m_Content.SetActive(b);
        }
    }
}

/// <summary>
/// 全局的panel都要去用這個顯示。
/// </summary>
public interface IGlobalPanel
{

}
