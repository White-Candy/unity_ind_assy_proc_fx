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

    private int m_Serial = -1;
    private List<ChoiceItem> m_itemList = new List<ChoiceItem>();

    public void Init(int serial, string topic, List<string> choices)
    {
        // Debug.Log(topic + " || " + choices.Count);
        m_Serial = serial;
        content.text = $"{serial}. {topic}";
        for (int i = 0; i < choices.Count; ++i)
        {
            ChoiceItem item = GameObject.Instantiate(choiceItem, parentTrans);
            item.Init(i, choices[i]);
            item.gameObject.SetActive(true);
            m_itemList.Add(item);
        }
    }

    /// <summary>
    /// 返回这道题的答案信息
    /// </summary>
    /// <returns></returns>
    public string OutputResult() // (序号，用户的答案)
    {
        string result = "";
        foreach (var item in m_itemList)
        {
            if (item.select.isOn)
            {
                string[] split = item.serial.text.Split(".");
                result += split[0];
            }
        }
        // Debug.Log($"OutputResult: {result}");
        return result;
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
    }
}