using Cysharp.Threading.Tasks;
using LitJson;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    public GameObject ExamineTime; // 考核模式倒计时

    public Button m_Submit; // 提交按钮

    public Button intrCloseButton; // 介绍面板关闭按钮

    public static InfoPanel _instance;

    private CancellationTokenSource m_cts = new CancellationTokenSource();

    // public bool m_showMap = true; // 是否展示小地图

    private bool m_OpenAudio = false;

    public override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        _instance = this;
        Active(false);

        //Debug.Log("m_IntroduceText: " + m_IntroduceText.text);
        m_View.onClick.AddListener(() =>
        {
            ActiveConstructionPanel();
        });

        m_Step.onClick.AddListener(() =>
        {
            // ActiveStepPanel();
        });

        intrCloseButton.onClick.AddListener(() => { ActiveConstructionPanel(); });

        m_Audio.onClick.AddListener(() => { AudioButtonClicked(); });

        // 点击提交成绩按钮
        m_Submit.onClick.AddListener(SubmitScore);

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

        //if (GlobalData.mode == Mode.Examination)
        //{
        //    await StartCountDown();
        //}
    }

    private void UpdateInfo()
    {
        if (GlobalData.StepIdx >= 0 && GlobalData.StepIdx < ModelAnimControl._Instance.m_ConPtStep.Count)
        {
            m_StepText.text = ModelAnimControl._Instance.m_ConPtStep?[GlobalData.StepIdx].step;
            m_IntroduceText.text = "     " + ModelAnimControl._Instance.m_ConPtStep?[GlobalData.StepIdx].constrPt;
        }
    }

    private void AudioButtonClicked()
    {
        if (m_OpenAudio)
            m_Audio.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/NewUI/Training/Audio");
        else
            m_Audio.GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/NewUI/Training/AudioOpen");

        m_OpenAudio = !m_OpenAudio;
    }

    private void ActiveConstructionPanel()
    {
        bool b = m_Introduce.gameObject.activeSelf;
        m_Introduce.gameObject.SetActive(!b);
        Image img = m_View.GetComponent<Image>();
        if (b)
            UITools.SetImage(ref img, "Textures/NewUI/Training/View");
        else
            UITools.SetImage(ref img, "Textures/NewUI/Training/ViewOpen");
    }

    // 实训模式隐藏一些窗口
    public void TrainingModeUIClose()
    {
        m_Audio.gameObject.SetActive(false);
        m_StepHint.gameObject.SetActive(false);
        m_Hint.gameObject.SetActive(false);
    }

    // 考核模式成绩提交
    private void SubmitScore()
    {
        UITools.OpenDialog("考核提交", $"是否提交{GlobalData.ProjGroupName}的实训成绩？",
                new ButtonData("取消", FPath.DialogWhite, () => { }),
                new ButtonData("确定", FPath.DialogBlue, () => { ExamineSubmit(); }));
    }


    // 开始倒计时
    public async UniTask StartCountDown()
    {
        int time = int.Parse(GlobalData.currExamsInfo.TrainingTime) * 60;
        await UniTask.WaitUntil(() => m_Visible == true);

        while (time > 0 && m_Visible)
        { 
            time--;
            UpdateTimeOnUI(time); 
            await UniTask.Delay(1000, cancellationToken: m_cts.Token);
            // Debug.Log(time);
        }

        if (time <= 0)
        {
            UITools.OpenDialog("时间超时", "时间到，已自动交卷。",
                new ButtonData("取消", FPath.DialogWhite, () => { }),
                new ButtonData("确定", FPath.DialogBlue, () => { ExamineSubmit(); }));
        }
    }

    // 修改UI的时间
    private void UpdateTimeOnUI(int time)
    {
        int hour = time / 3600;
        int min = (time - hour * 3600) / 60;
        int second = time - hour * 3600 - min * 60;

        string str_time = $"{Tools.FillingForTime(hour.ToString()) + ":" + Tools.FillingForTime(min.ToString()) + ":" + Tools.FillingForTime(second.ToString())}";
        ExamineTime.GetComponentInChildren<TextMeshProUGUI>().SetText(str_time);
    }

    /// <summary>
    /// 提交
    /// </summary>
    public void ExamineSubmit()
    {
        // GlobalData.currModuleName = "";
        float trainingScore = GameMode.Instance.totalScore;
        GlobalData.currScoreInfo.trainingScore = trainingScore.ToString();
        GlobalData.currScoreInfo.trainingFinished = true;
        // Debug.Log("training Score total: " + trainingScore);
        HTTPConsole.SendAsyncPost(JsonMapper.ToJson(GlobalData.currScoreInfo), EventType.ScoreEvent, OperateType.REVISE);
        Utilly.ExitModeSceneAction();
        TitlePanel._instance.SetTitlePanelActive(true);
    }

    public void SetActiveOfExamUI(bool b)
    {
        ExamineTime.gameObject.SetActive(b);
        m_Submit.gameObject.SetActive(b);
    }

    // 停止CountDown的线程
    public void CancelCountDownToken()
    {
        m_cts.Cancel();
        m_cts.Dispose();
        m_cts = new CancellationTokenSource();
    }
}
