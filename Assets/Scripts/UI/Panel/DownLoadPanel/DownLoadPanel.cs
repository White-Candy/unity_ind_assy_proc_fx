using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DownLoadPanel : BasePanel
{
    public Slider m_ProgressSlider; // �M�ȗl
    public TextMeshProUGUI m_ProgressPercent; // �@ʾ�M�ȗl�ٷֱ�
    public TextMeshProUGUI m_Hint; // ��ʾ�ı�
    public Button m_Finish; // ��ɰ�ť

    [HideInInspector]
    public bool m_Finished; // �˴θ���ȫ�����

    [HideInInspector]
    public List<string> m_NeedDL = new List<string>(); // ��Ҫ�������ظ��µ��ļ�

    [HideInInspector]
    public List<FilePackage> m_NeedWt = new List<FilePackage>(); // ��Ҫд�뵽���ص��ļ�

    public static DownLoadPanel _instance;


    // private float m_uiPercent; // UI��������ʾ
    private float m_Percent = 0.0f; // �ܽ���
    private float m_bufPercent = 0.0f; // �����ļ��Ľ���

    public override void Awake()
    {
        base.Awake();
        // Debug.Log("DownLoadPanel Awake");
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
    }

    public void Start()
    {
        Active(false);
        m_Finish.enabled = false;
    }

    /// <summary>
    /// ���ý������ٷֱ�
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
    /// ����д���ļ�ʱ�Ľ�����
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
    /// UI������
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
            // Debug.Log("������ɣ�" + " || " + percent);
            m_Hint.text = $"������ɣ�";
            m_Finish.enabled = true;
        }
        else if (m_ProgressSlider.value >= 0.9f)
        {
            // Debug.Log("�ļ�д�룡" + " || " + percent);
            m_Hint.text = $"�ȴ��ļ�д�뵽����...";
        }
        else
        {
            // Debug.Log("����������Դ " + " || " + percent);
            m_Hint.text = $"����������Դ...";
            m_Finish.enabled = false;
            m_Finished = false;
        }
        await UniTask.Yield();
        //}
    }

    /// <summary>
    /// ���
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

        m_Hint.text = $"����������Դ...";
        m_ProgressSlider.value = 0.0f;
        m_ProgressPercent.text = "0.00%";

        GlobalData.Checked = false;
        GlobalData.Downloaded = false;
        GlobalData.IsLatestRes = false;
    }
}
