using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DialogPanelItem : MonoBehaviour
{
    // 按钮
    public Button m_Button;

    // 文本显示
    public List<TextMeshProUGUI> m_Text = new List<TextMeshProUGUI>();

    // 按钮点击事件
    private Action OnButtonClicked = () => { };

    private void Start()
    {
        m_Button.onClick.AddListener(() => {  OnButtonClicked(); });
    }

    public void UpdateData(string text)
    {
        m_Text.ForEach(a => a.text = text);
    }

    /// <summary>
    /// 添加事件
    /// </summary>
    public void AddListener(Action callback)
    {
        OnButtonClicked = callback;
    }


    /// <summary>
    /// 清理
    /// </summary>
    public void Clear()
    {
        m_Text.ForEach(a => a.text = "");
    }
}
