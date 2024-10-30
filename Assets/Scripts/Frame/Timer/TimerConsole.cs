using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TimerConsole : Singleton<TimerConsole>
{
    // ��ʱ�������
    public PoolList<Timer> m_Pool = new PoolList<Timer>();

    // �б�
    public List<Timer> m_List = new List<Timer>();

    // Root
    private GameObject m_Root;

    public override void Awake()
    {
        base.Awake();

    }

    public void Start()
    {
        m_Root = new GameObject("TimerList");
        m_Pool.AddListener(Instance);

        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnUnSceneLoaded;

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(m_Root);
    }

    public Timer CreateTimer(bool global = false)
    {
        Timer timer = m_Pool.Create("Static_Timer");
        timer.m_Global = global;
        return timer;
    }

    /// <summary>
    /// ʵ����
    /// </summary>
    /// <returns></returns>
    private new Timer Instance()
    {
        GameObject obj = new GameObject();
        obj.transform.SetParent(m_Root.transform);
        obj.transform.localScale = Vector3.one;

        Timer timer = obj.AddComponent<Timer>();
        m_List.Add(timer);
        return timer;
    }

    /// <summary>
    /// ��������
    /// </summary>
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

    }

    /// <summary>
    /// ����ж��
    /// </summary>
    /// <param name="arg0"></param>
    /// <exception cref="NotImplementedException"></exception>
    private void OnUnSceneLoaded(Scene arg0)
    {
        foreach (Timer item in m_List)
        {
            if (!item.m_Global)
            {
                item.Destroy();
            }
        }
    }

    /// <summary>
    /// ����
    /// </summary>
    /// <param name="t"></param>
    public void Destroy(Timer t)
    {
        m_Pool.Destroy(t);
    }
}
