using System.Collections.Generic;
using UnityEngine;

public class UnityEventCenter
{
    private static UnityEventCenter m_instance;

    public static UnityEventCenter Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new UnityEventCenter();
            }
            return m_instance;
        }
    }

    public delegate void OnNotification(IMessage msg);

    public static Dictionary<EnumDefine.EventKey, OnNotification> eventListeners = new Dictionary<EnumDefine.EventKey, OnNotification>();

    public static void AddListener(EnumDefine.EventKey eventKey, OnNotification notify)
    {
        if (notify == null)
        {
            Debug.LogError("参数不全!!!");
            return;
        }

        if (!eventListeners.ContainsKey(eventKey))
        {
            eventListeners.Add(eventKey, notify);
        }
        else
        {
            eventListeners[eventKey] += notify;
        }
    }

    /// <summary>
    /// 该方法负责 事件的分发调用
    /// </summary>
    /// <param name="eventKey"></param>
    /// <param name="msg"></param>
    public static void DistributeEvent(EnumDefine.EventKey eventKey, IMessage msg)
    {
        if (!eventListeners.ContainsKey(eventKey))
        {
            Debug.Log("key值错误，请检查代码！！！= " + eventKey);
            return;
        }

        eventListeners[eventKey](msg);
    }

    /// <summary>
    /// 移除某一事件的全部监听
    /// </summary>
    /// <param name="eventKey"></param>
    public static void RemoveEventLister(EnumDefine.EventKey eventKey)
    {
        if (!eventListeners.ContainsKey(eventKey))
        {
            return;
        }
        eventListeners.Remove(eventKey);
    }

    /// <summary>
    /// 移除某一事件的某一个监听
    /// </summary>
    /// <param name="eventKey"></param>
    /// <param name="notify"></param>
    public static void RemoveLister(EnumDefine.EventKey eventKey, OnNotification notify)
    {
        if (!eventListeners.ContainsKey(eventKey))
        {
            return;
        }
        eventListeners[eventKey] -= notify;

        if (eventListeners[eventKey] == null)
        {
            eventListeners.Remove(eventKey);
        }
    }
}
