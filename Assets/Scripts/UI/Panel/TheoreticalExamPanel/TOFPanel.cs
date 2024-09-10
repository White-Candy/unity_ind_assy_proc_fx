using System.Collections.Generic;
using UnityEngine;

public class TOFPanel : BasePanel
{
    public TopicItem topicItem;
    public Transform parentTrans;    
    
    private List<TOFChoice> m_tofList = new List<TOFChoice>();
    private List<TopicItem> m_itemList = new List<TopicItem>();    
    
    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="tofList"></param>
    public void Init(List<TOFChoice> tofList)
    {
        m_tofList = new List<TOFChoice>(tofList);
        List<string> nameList = new List<string>();
        for (int i = 0; i < m_tofList.Count; ++i)
        {
            TOFChoice item = m_tofList[i];
            nameList.Clear();
            nameList.Add(item.toA.m_content);
            nameList.Add(item.toB.m_content);

            TopicItem topic = GameObject.Instantiate(topicItem, parentTrans);
            topic.Init(i + 1, item.Topic, nameList);
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
            // Debug.Log($"Mulit Topic serial: {serial} | and User's answer: {usrAnswer}");
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
        m_tofList.Clear();
    }
}