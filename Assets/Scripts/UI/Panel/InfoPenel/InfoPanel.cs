using sugar;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : BasePanel
{
    public Button m_View; // 点击按钮显示施工提示面板
    public Button m_Audio; // 声音按钮
    public Button m_Step; // 步骤展示按钮
    public TextMeshProUGUI m_StepText; // 步骤面板Text
    public TextMeshProUGUI m_Count; // 需要拖入工具数量
    public TextMeshProUGUI m_IntroduceText; // 施工要点面板Text

    public GameObject m_Introduce; // 施工要点面板
    // public GameObject m_StepPanel; // 步骤选择面板
    public GameObject m_Hint; // 提示面板
    public GameObject m_StepHint; // 步骤提示面板
    // public GameObject m_Minmap; // 小地图

    public static InfoPanel _instance;

    // public bool m_showMap = true; // 是否展示小地图

    public override void Awake()
    {
        base.Awake();
        _instance = this;
        Active(false);
    }

    private void Start()
    {
        //Debug.Log("m_IntroduceText: " + m_IntroduceText.text);
        m_View.onClick.AddListener(() =>
        {
            ActiveConstructionPanel();
        });

        m_Step.onClick.AddListener(() =>
        {
            // ActiveStepPanel();
        });

        Init();
    }

    private void FixedUpdate()
    {
        // m_Count.text = GameMode.Instance.NumberOfToolsRemaining();
        UpdateInfo();
    }

    private void Init()
    {
        m_Introduce?.gameObject.SetActive(false);
    }

    private void UpdateInfo()
    {
        if (GlobalData.StepIdx >= 0 && GlobalData.StepIdx < GlobalData.stepStructs.Count)
        {
            m_StepText.text = ModelAnimControl._Instance.m_ConPtStep?[GlobalData.StepIdx].step;
            m_IntroduceText.text = ModelAnimControl._Instance.m_ConPtStep?[GlobalData.StepIdx].constrPt;
        }
    }

    private void ActiveConstructionPanel()
    {
        bool b = m_Introduce.gameObject.activeSelf;
        m_Introduce.gameObject.SetActive(!b);
    }

    // 实训模式隐藏一些窗口
    public void TrainingModeUIClose()
    {
        m_Audio.gameObject.SetActive(false);
        m_StepHint.gameObject.SetActive(false);
        m_Hint.gameObject.SetActive(false);
    }
}
