using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConsole : MonoBehaviour
{
    public static Dictionary<string, BasePanel> m_List = new Dictionary<string, BasePanel>();

    public void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public static T FindPanel<T>() where T : class, IBasePanel
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

    public static void AddPanel(string key, BasePanel panel)
    {
        if (!m_List.ContainsKey(key))
        {
            Debug.Log("AddPanel: " + key);
            m_List.Add(key, panel);
        }
    }

    /// <summary>
    /// UI�YԴ�ļ��d
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T FindAssetPanel<T>() where T : BasePanel
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
                    // ȫ��UI���d
                    go.transform.SetParent(GlobalCanvas.Instance.transform);
                }
                else
                {
                    // ����UI���d
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
