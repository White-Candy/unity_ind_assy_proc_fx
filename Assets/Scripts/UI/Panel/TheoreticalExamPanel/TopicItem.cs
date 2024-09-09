using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 试题结构类
/// </summary>
public class TopicItem : MonoBehaviour
{
    public TextMeshProUGUI content;
    public ChoiceItem choiceItem;
    public Transform parentTrans;

    private List<ChoiceItem> m_itemList = new List<ChoiceItem>();

    public void Init(int serial, string topic, List<string> choices)
    {
        content.text = $"{serial}. {topic}";
        for (int i = 0; i < choices.Count; ++i)
        {
            ChoiceItem item = GameObject.Instantiate(choiceItem, parentTrans);
            item.Init(i, choices[i]);
            item.gameObject.SetActive(true);
            m_itemList.Add(item);
        }
    }

    public string OutputResult()
    {
        string result = "";
        foreach (var item in m_itemList)
        {
            if (item.select.isOn)
            {
                result += item.serial;
            }
        }
        Debug.Log($"OutputResult: {result}");
        return result;
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