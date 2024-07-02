using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class TheoreticalExamQuestion : MonoBehaviour
{
    // 问题数据
    public QuestionData m_Data;

    // 问题文本
    public TextMeshProUGUI m_TextQuestion;

    // 问题项
    public GameObject m_ItemQuestion;

    //问题选项模板
    public GameObject m_TemplateOption;

    //模板父物体
    public GameObject m_TemplateParent;

    // 问题字体大小
    private int m_FontSizeQuestion;

    // 问题类型字体大小[多选 or 单选]
    private int m_FontSizeQuestionType;

    // 选项列表
    public List<TheoreticalExamOption> m_chooseList = new List<TheoreticalExamOption>();

    // 选项对象内存池
    private PoolList<TheoreticalExamOption> m_Pool = new PoolList<TheoreticalExamOption>();

    // 玩家每个问题给出的答案
    private string m_Answer;

    private void Awake()
    {
        m_Pool.AddListener(Instance);

        m_FontSizeQuestion = (int)m_TextQuestion.fontSize;
        m_FontSizeQuestionType = m_FontSizeQuestion - 6;
    }

    // 对象池中的对象内存申请方法
    private TheoreticalExamOption Instance()
    {
        GameObject go = GameObject.Instantiate(m_TemplateOption, m_TemplateParent.transform);
        go.transform.localScale = Vector3.one;

        TheoreticalExamOption op = go.GetComponent<TheoreticalExamOption>();
        return op;
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="data"></param>
    public void Init(QuestionData data)
    {
        m_Data = data;

        // 问题初始化
        InitQuestion(data.number, data.type, data.text);

        for (int i = 0; i < data.options.Count; ++i)
        {
            OptionData item = data.options[i];
            TheoreticalExamOption op_item = m_Pool.Create("Item_Option" + item.order.ToString());
            op_item.transform.SetAsLastSibling();
            op_item.onToggle = OnItemToggle;
            op_item.Init(data.type, item); // 对每个选项进行初始化
            m_chooseList.Add(op_item); // 保存这个问题的每个选项
        }
    }

    /// <summary>
    /// 获取理论考核的Body信息
    /// </summary>
    /// <returns></returns>
    public (float, string, int) GetExamBody()
    {
        var c_user = m_Answer.ToUpper().ToCharArray(); // 玩家的答案
        var c_sys = m_Data.answer.ToUpper().ToCharArray(); // 正确答案

        // 为了避免多选题乱序，所以需要排列一下
        Array.Sort(c_user);
        Array.Sort(c_sys);

        var s_user = new string(c_user);
        var s_sys = new string(c_sys);
        float score = 0;

        if (s_user.Equals(s_sys))
        {
            score = m_Data.score;
        }
        else
        {
            score = 0;
        }
        return (score, s_user, m_Data.ID);
    }

    /// <summary>
    /// 点击选择toggle
    /// 更新玩家答案
    /// </summary>
    /// <param name="data"></param>
    private void OnItemToggle()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in m_chooseList)
        {
            sb.Append(item.m_Answer);
        }
        m_Answer = sb.ToString();
    }

    /// <summary>
    /// 初始化每一道题目的Item控件内容
    /// </summary>
    /// <param name="number"></param>
    /// <param name="type"></param>
    /// <param name="text"></param>
    private void InitQuestion(int number, QuestionType type, string text)
    {
        //Debug.Log("Question: " + text);
        text = Tools.checkLength(text, 27);
        m_TextQuestion.text = string.Format($"{number}.{text}\n{GetQuestionType(type, m_FontSizeQuestionType)}");
        m_ItemQuestion.transform.SetAsLastSibling();
    }

    /// <summary>
    /// 获取 问题类型 原始文本
    /// eg: (本题是多选题)
    /// </summary>
    /// <param name="type"></param>
    /// <param name="size"></param>
    /// <returns></returns>
    private string GetQuestionType(QuestionType type, int size)
    {
        return string.Format($"<color=#FF7900FF><size={size}>{QuestionTypeToString(type)}</size></color>");
    }

    private string QuestionTypeToString(QuestionType type)
    {
        if (type == QuestionType.Single)
            return "（单项选择题）";
        else if (type == QuestionType.Multiple)
            return "（多项选择题）";
        else if (type == QuestionType.TrueOrFalse)
            return "（判断题）";
        else if (type == QuestionType.Fill)
            return "（填空题）";
        else
            return "";
    }

    /// <summary>
    /// 控件交互控制
    /// </summary>
    /// <param name="b"></param>
    public void AllControlActive(bool b)
    {
        foreach (var item in m_chooseList)
        {
            item.AllToggleActive(b);
        }
    }

    public void Clear()
    {
        m_Answer = null;
        foreach (var item in m_chooseList)
        {
            item.Clear();
            m_Pool.Destroy(item);
        }
        m_chooseList.Clear();
    }
}
