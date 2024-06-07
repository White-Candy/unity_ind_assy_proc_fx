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


    public virtual void Awake()
    {
        m_NameP = this.GetType().ToString();
        Debug.Log("NAME: " + m_NameP);
        UIConsole.Instance.m_List.Add(m_NameP, this);
    }

    /// <summary>
    /// 面板的显示控制
    /// </summary>
    /// <param name="b"></param>
    public virtual void Active(bool b)
    {
        if (m_Content != null)
        {
            m_Content.SetActive(b);
        }
    }
}

public interface IGlobalPanel
{

}
