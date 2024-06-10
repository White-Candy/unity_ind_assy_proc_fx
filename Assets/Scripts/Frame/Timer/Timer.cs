using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public float m_CurrTime = 0;

    public float m_Time = 0;

    public Action m_Callback = null;

    public bool m_Running = false;

    public bool m_Destroy = false;

    // 是否为全局对象，如果不是的话，在TimerConsole的场景卸载的时候会Destroy掉。
    public bool m_Global = false;
    private void FixedUpdate()
    {
        if (m_Running)
        {
            if (m_CurrTime >= m_Time)
            {
                m_Running = false;
                if (m_Callback != null)
                {
                    m_Callback();
                }

                if (m_Destroy)
                {
                    Destroy();
                }
            }
            else
            {
                m_CurrTime += Time.fixedDeltaTime;
            }
        }
    }

    /// <summary>
    /// 倒计时
    /// </summary>
    /// <param name="time"></param>
    /// <param name="callback"></param>
    /// <param name="destroy">执行结束是否销毁</param>
    public void Run(float time, Action callback, bool destroy = false)
    {
        m_Time = time;
        m_CurrTime = 0;
        m_Callback = callback;
        m_Destroy = destroy;
        m_Running = true;
    }

    public void Stop()
    {
        m_Running = false;
    }

    public void Destroy()
    {
        m_Running = false;
        m_Callback = null;
        TimerConsole.Instance.Destroy(this);
    }
}
