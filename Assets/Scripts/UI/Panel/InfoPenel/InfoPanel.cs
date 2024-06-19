using sugar;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public Button m_View; // 点击按钮显示施工提示面板
    public Button m_Audio; // 声音按钮
    public TextMeshProUGUI m_StepText; // 步骤面板Text
    public TextMeshProUGUI m_Count; // 需要拖入工具数量
    public GameObject m_Introduce; // 施工要点面板

    private TextMeshProUGUI m_IntroduceText; // 施工要点面板Text

    private void Awake()
    {
        m_IntroduceText = GameObject.Find("InstroduceTx").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        //Debug.Log("m_IntroduceText: " + m_IntroduceText.text);
        m_View.onClick.AddListener(() =>
        {
            ActiveConstructionPanel();
        });

        Init();
    }

    private void FixedUpdate()
    {
        m_Count.text = GameMode.Instance.NumberOfToolsRemaining();
        UpdateInfo();
    }

    private void Init()
    {
        
    }

    private void UpdateInfo()
    {
        m_StepText.text = ModelAnimControl._Instance.m_ConPtStep?[GlobalData.StepIdx].step;
        m_IntroduceText.text = ModelAnimControl._Instance.m_ConPtStep?[GlobalData.StepIdx].constrPt;
    }

    private void ActiveConstructionPanel()
    {
        bool b = m_Introduce.gameObject.activeSelf;
        m_Introduce.gameObject.SetActive(!b);
    }
}
