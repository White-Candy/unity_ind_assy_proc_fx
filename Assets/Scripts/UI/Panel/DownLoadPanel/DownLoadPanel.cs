using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DownLoadPanel : BasePanel
{
    public Slider m_ProgressSlider; // 進度條
    public TextMeshProUGUI m_ProgressPercent; // 顯示進度條百分比
    public TextMeshProUGUI m_Hint; // 提示文本
    public Button m_Finish; // 完成按钮

    [HideInInspector]
    public bool m_Finished; // 此次更新全部完成

    [HideInInspector]
    public List<string> m_NeedDL = new List<string>(); // 需要请求下载更新的文件

    [HideInInspector]
    public List<FilePackage> m_NeedWt = new List<FilePackage>(); // 需要写入到本地的文件

    public static DownLoadPanel _instance;


    // private float m_uiPercent; // UI进度条显示
    private float m_Percent = 0.0f; // 总进度
    private float m_bufPercent = 0.0f; // 单个文件的进度

    public override void Awake()
    {
        base.Awake();
        // Debug.Log("DownLoadPanel Awake");
    }

    public void Start()
    {
        _instance = this;
        m_Finished = false;
        m_Finish.onClick.AddListener(() =>
        {
            // Debug.Log("Go Finish~");
            Active(false);

            // m_uiPercent = 0.0f;
            // m_bufPercent = 0.0f;
            // m_Percent = 0.0f;
            Clear();
            m_Finished = true;
        });

        Active(false);
        m_Finish.enabled = false;
    }

    /// <summary>
    /// 设置进度条百分比
    /// </summary>
    /// <param name="percent"></param>
    public void SetDLPercent(float percent)
    {
        m_Percent = m_bufPercent + percent / m_NeedDL.Count * 0.9f;
        //Debug.Log("===================== SetDLPercent: " + percent);
        SetUIPercent(m_Percent);

        if (percent == 100.0f)
        {
            m_bufPercent = m_Percent;
        }

        // Debug.Log($"=========== {m_Percent} || {m_bufPercent} || {percent} || {m_NeedDL.Count}");
    }

    /// <summary>
    /// 设置写入文件时的进度条
    /// </summary>
    /// <param name="percent"></param>
    public void SetWritePercent()
    {
        m_Percent = m_bufPercent + 10.0f / m_NeedWt.Count;
        m_bufPercent = m_Percent;
        // Debug.Log($" SetWritePercent =========== {m_Percent} || {m_bufPercent}");
        SetUIPercent(m_Percent);
    }

    /// <summary>
    /// UI进度条
    /// </summary>
    /// <param name="percent"></param>
    private async void SetUIPercent(float percent)
    {
        // await UniTask.SwitchToMainThread();
        // Debug.Log("Set UI Percent: " + percent);
        //while (m_uiPercent < percent)
        //{
            //m_uiPercent += 0.5f;
        m_ProgressSlider.value = percent / 100.0f;
        m_ProgressPercent.text = percent.ToString("f1") + "%";
        // Debug.Log("=============== SetUIPercent: " + percent * 1.0f);

        if (m_ProgressSlider.value == 1.0f)
        {
            // Debug.Log("更新完成！" + " || " + percent);
            m_Hint.text = $"更新完成！";
            m_Finish.enabled = true;
        }
        else if (m_ProgressSlider.value >= 0.9f)
        {
            // Debug.Log("文件写入！" + " || " + percent);
            m_Hint.text = $"等待文件写入到本地...";
        }
        else
        {
            // Debug.Log("正在下载资源 " + " || " + percent);
            m_Hint.text = $"正在下载资源...";
            m_Finish.enabled = false;
            m_Finished = false;
        }
        await UniTask.Yield();
        //}
    }

    /// <summary>
    /// 清空
    /// </summary>
    public void Clear()
    {
        m_NeedWt.Clear();
        m_NeedDL.Clear();
        m_Finished = false;
        m_Percent = 0.0f;
        m_bufPercent = 0.0f;
        // m_uiPercent = 0.0f;

        GlobalData.DownloadParcent = 0.0f;
        GlobalData.currEventType = EventType.None;

        m_Hint.text = $"正在下载资源...";
        m_ProgressSlider.value = 0.0f;
        m_ProgressPercent.text = "0.00%";

        GlobalData.Checked = false;
        GlobalData.Downloaded = false;
        GlobalData.IsLatestRes = false;
    }
}
