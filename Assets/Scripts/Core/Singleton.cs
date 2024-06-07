using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour‘º ¯¿‡
/// </summary>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_instance;

    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                GameObject go = new GameObject(typeof(T).ToString());
                m_instance = go.AddComponent<T>();
                go.name = typeof(T).ToString();
            }
            return m_instance;
        }
    }

    public virtual void Awake()
    {
        m_instance = this as T;
    }

    public static bool IsNull
    {
        get
        {
            return (null == m_instance);
        }
    }
}
