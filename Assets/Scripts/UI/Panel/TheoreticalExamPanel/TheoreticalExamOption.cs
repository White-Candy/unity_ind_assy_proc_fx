using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TheoreticalExamOption : MonoBehaviour
{
    // 选项数据
    public OptionData m_Data;

    // 按钮开关
    public Toggle m_Toggle;

    // 按钮开关组
    // 对于单选题多个选项只能由一个是ison，所以需要ToggleGroup进行控制
    public ToggleGroup m_Group;

    //显示题目文本
    public TextMeshProUGUI m_Text;

    // 用户选择的答案
    public string m_Answer { get { return m_Toggle.isOn ? m_Data.order.ToString() : ""; } }

    // 问题类型
    public QuestionType m_Type;



    public Action onToggle = () => { };

    public void Init(QuestionType type, OptionData data)
    {
        m_Type = type;
        m_Data = data;

        // 更新 UI状态/内容
        UpdateToggle(type);
        UpdateText(data.order, data.text);

        m_Toggle.onValueChanged.AddListener(OnToggleClick);
    }

    /// <summary>
    /// 更新选择按钮的状态
    /// </summary>
    public void UpdateToggle(QuestionType type)
    {
        // 如果是多选题那么不需要加入到Group中去
        if (type == QuestionType.Multiple)
        {
            m_Toggle.group = null;
        }
        else
        {
            m_Toggle.group = m_Group;
        }
    }

    /// <summary>
    /// 更新题目文本的内容
    /// </summary>
    public void UpdateText(OptionOrder order, string text)
    {
        text = Tools.checkLength(text, 26);
        m_Text.text = $"{order}.{text}";
    }

    /// <summary>
    /// 设置Toggle是否可交互
    /// </summary>
    /// <param name="b"></param>
    public void AllToggleActive(bool b)
    {
        m_Toggle.interactable = b;
    }

    /// <summary>
    /// Toggle控件点击触发
    /// </summary>
    /// <param name="b"></param>
    public void OnToggleClick(bool b)
    {
        onToggle();
    }

    /// <summary>
    /// 退出清理
    /// </summary>
    public void Clear()
    {
        onToggle = () => { };
        m_Group = null;
        m_Toggle.isOn = false;
    }
}
