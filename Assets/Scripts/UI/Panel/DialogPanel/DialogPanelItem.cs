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
    public TextMeshProUGUI m_Text;

    // 按钮点击事件
    private Action OnButtonClicked = () => { };

    private void Start()
    {
        m_Button.onClick.AddListener(() => {  OnButtonClicked(); });
    }

    public void UpdateData(string text)
    {
        //m_Text.ForEach(a => a.text = text);
        m_Text.text = text;
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
        m_Text.text = "";
        //m_Text.ForEach(a => a.text = "");
    }
}
