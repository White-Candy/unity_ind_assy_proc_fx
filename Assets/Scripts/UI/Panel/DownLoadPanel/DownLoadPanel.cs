using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class DownLoadPanel : BasePanel
{
    public Slider m_ProgressSlider; // 進度條
    public TextMeshProUGUI m_ProgressPercent; // 顯示進度條百分比
    public TextMeshProUGUI m_Hint; // 提示文本
    public Button m_Finish; // 完成按钮

    public static DownLoadPanel _instance;

    public override void Awake()
    {
        base.Awake();

        _instance = this;

        m_ProgressSlider.onValueChanged.AddListener((value) => 
        {
            Debug.Log("m_ProgressSlider.onValueChanged");

        });

        m_Finish.onClick.AddListener(() => 
        {
            Debug.Log("Go Finish~");
            Active(false);
        });

        Active(false);
        m_Finish.enabled = false;
    }

    /// <summary>
    /// 设置进度条百分比
    /// </summary>
    /// <param name="percent"></param>
    public void SetPercent(float percent)
    {   
        m_ProgressSlider.value = percent / 100.0f;
        m_ProgressPercent.text = $"{percent}%";

        if (m_ProgressSlider.value >= 1.0f)
        {
            m_Hint.text = $"更新完成！";
            m_Finish.enabled = true;
        }
        else
        {
            m_Hint.text = $"正在更新资源...";
            m_Finish.enabled = false;
        }
    }
}
