using System.Collections.Generic;
using UnityEngine;

public class MulitPanel : BasePanel
{
    public TopicItem topicItem;
    public Transform parentTrans;
        
    private List<MulitChoice> m_mulitList = new List<MulitChoice>();
    private List<TopicItem> m_itemList = new List<TopicItem>();

    public override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="mulitList"></param>
    public void Init(List<MulitChoice> mulitList)
    {
        m_mulitList = new List<MulitChoice>(mulitList);
        List<string> nameList = new List<string>();
        for (int i = 0; i < m_mulitList.Count; ++i)
        {
            MulitChoice item = m_mulitList[i];
            nameList.Clear();
            foreach (var option in item.Options) { nameList.Add(option.Content); }
            TopicItem topic = GameObject.Instantiate(topicItem, parentTrans);
            topic.Init(i + 1, item.Topic, nameList);
            topic.gameObject.SetActive(true);
            m_itemList.Add(topic);            
        }
    }

    /// <summary>
    /// 提交
    /// </summary>
    /// <returns></returns>
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
        m_mulitList.Clear();
    }    
}