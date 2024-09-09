using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SinglePanel : BasePanel
{
    public TopicItem topicItem;
    public Transform parentTrans;

    private List<SingleChoice> m_singleList;

    private List<TopicItem> m_itemList = new List<TopicItem>();

    public override void Awake()
    {
        base.Awake();
    }

    public void Init(List<SingleChoice> singleList)
    {
        m_singleList = singleList;
        List<string> list = new List<string>();
        for (int i = 0; i < singleList.Count; ++i)
        {
            var item = singleList[i];

            list.Clear();
            list.Add(item.toA.m_content);
            list.Add(item.toB.m_content);
            list.Add(item.toC.m_content);
            list.Add(item.toD.m_content);

            var topic = GameObject.Instantiate(topicItem, parentTrans);
            topic.Init(i + 1, item.Topic, list);
            topic.gameObject.SetActive(true);
            m_itemList.Add(topic);
        }
    }

    public void Clear()
    {
        foreach (var item in m_itemList)
        {
            item.Clear();
            item.gameObject.SetActive(false);
            Destroy(item);
        }
        m_itemList.Clear();
    }
}