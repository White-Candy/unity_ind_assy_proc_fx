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
                //Debug.Log(typeof(T).Name);
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

    /// <summary>
    /// UI資源的加載
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T FindAssetPanel<T>() where T : BasePanel
    {
        T t = FindPanel<T>();
        if (t == null)
        {
            string path = "Prefabs/UI/Panel/" + typeof(T).ToString();
            GameObject go = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(path));
            if (go != null)
            {
                t = go.GetComponent<T>();
                go.name = typeof(T).Name;
                if (t is IGlobalPanel)
                {
                    // 全局UI加載
                    go.transform.SetParent(GlobalCanvas.Instance.transform);
                }
                else
                {
                    // 場景UI加載
                    go.transform.SetParent(GameObject.Find("MainCanvas/").transform);
                }
                go.transform.localScale = Vector3.one;
                RectTransform rect = go.transform as RectTransform;
                rect.offsetMax = Vector2.zero;
                rect.offsetMin = Vector2.zero;
            }
        }
        return t;
    }
}

public interface IBasePanel
{

}
