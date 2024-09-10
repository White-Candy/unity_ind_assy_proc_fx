using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SinglePanel : BasePanel
{
    public TopicItem topicItem;
    public Transform parentTrans;

    private List<SingleChoice> m_singleList = new List<SingleChoice>();

    private List<TopicItem> m_itemList = new List<TopicItem>();

    public override void Awake()
    {
        base.Awake();
    }
    
    /// <summary>
    ///  初始化
    /// </summary>
    /// <param name="singleList"></param>
    public void Init(List<SingleChoice> singleList)
    {
        m_singleList = new List<SingleChoice>(singleList);
        List<string> list = new List<string>();
        for (int i = 0; i < m_singleList.Count; ++i)
        {
            SingleChoice item = m_singleList[i];

            list.Clear();
            list.Add(item.toA.m_content);
            list.Add(item.toB.m_content);
            list.Add(item.toC.m_content);
            list.Add(item.toD.m_content);

            TopicItem topic = GameObject.Instantiate(topicItem, parentTrans);
            topic.Init(i + 1, item.Topic, list);
            topic.gameObject.SetActive(true);
            m_itemList.Add(topic);
        }
    }

    public List<string> Submit()
    {   
        List<string> results = new List<string>();
        foreach (TopicItem item in m_itemList)
        {
            string usrAnswer;
            usrAnswer = item.OutputResult();
            // Debug.Log($"Single Topic serial: {serial} | and User's answer: {usrAnswer}");

            results.Add(usrAnswer);
        }
        return results;
    }

    /// <summary>
    /// 清空
    /// </summary>
    public void Clear()
    {
        foreach (var item in m_itemList)
        {
            item.Clear();
            item.gameObject.SetActive(false);
            Destroy(item);
        }
        m_itemList.Clear();
        m_singleList.Clear();
    }
}