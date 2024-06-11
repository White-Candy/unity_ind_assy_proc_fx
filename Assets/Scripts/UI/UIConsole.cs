using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConsole : Singleton<UIConsole>
{
    public Dictionary<string, BasePanel> m_List = new Dictionary<string, BasePanel>();

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }

    public T FindPanel<T>() where T : class, IBasePanel
    {
        foreach (var panel in m_List.Values)
        {
            if (panel is T)
            {
                Debug.Log(typeof(T).Name);
                return panel as T;
            }
        }
        return null;
    }

    public void AddPanel(string key, BasePanel panel)
    {
        if (!m_List.ContainsKey(key))
        {
            m_List.Add(key, panel);
        }
    }
}

public interface IBasePanel
{

}
