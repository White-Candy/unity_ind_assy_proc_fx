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
    public Button m_View; // �����ť��ʾʩ����ʾ���
    public Button m_Audio; // ������ť
    public Button m_Step; // ����չʾ��ť
    public TextMeshProUGUI m_StepText; // �������Text
    public TextMeshProUGUI m_Count; // ��Ҫ���빤������
    public TextMeshProUGUI m_IntroduceText; // ʩ��Ҫ�����Text

    public GameObject m_Introduce; // ʩ��Ҫ�����
    // public GameObject m_StepPanel; // ����ѡ�����
    public GameObject m_Hint; // ��ʾ���
    public GameObject m_StepHint; // ������ʾ���
    // public GameObject m_Minmap; // С��ͼ

    public GameObject ExamineTime; // ����ģʽ����ʱ

    public Button m_Submit; // �ύ��ť

    public Button intrCloseButton; // �������رհ�ť

    public static InfoPanel _instance;

    private CancellationTokenSource m_cts = new CancellationTokenSource();

    // public bool m_showMap = true; // �Ƿ�չʾС��ͼ

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

        // ����ύ�ɼ���ť
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

    // ʵѵģʽ����һЩ����
    public void TrainingModeUIClose()
    {
        m_Audio.gameObject.SetActive(false);
        m_StepHint.gameObject.SetActive(false);
        m_Hint.gameObject.SetActive(false);
    }

    // ����ģʽ�ɼ��ύ
    private void SubmitScore()
    {
        UITools.OpenDialog("�����ύ", $"�Ƿ��ύ{GlobalData.ProjGroupName}��ʵѵ�ɼ���",
                new ButtonData("ȡ��", FPath.DialogWhite, () => { }),
                new ButtonData("ȷ��", FPath.DialogBlue, () => { ExamineSubmit(); }));
    }


    // ��ʼ����ʱ
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
            UITools.OpenDialog("ʱ�䳬ʱ", "ʱ�䵽�����Զ�����",
                new ButtonData("ȡ��", FPath.DialogWhite, () => { }),
                new ButtonData("ȷ��", FPath.DialogBlue, () => { ExamineSubmit(); }));
        }
    }

    // �޸�UI��ʱ��
    private void UpdateTimeOnUI(int time)
    {
        int hour = time / 3600;
        int min = (time - hour * 3600) / 60;
        int second = time - hour * 3600 - min * 60;

        string str_time = $"{Tools.FillingForTime(hour.ToString()) + ":" + Tools.FillingForTime(min.ToString()) + ":" + Tools.FillingForTime(second.ToString())}";
        ExamineTime.GetComponentInChildren<TextMeshProUGUI>().SetText(str_time);
    }

    /// <summary>
    /// �ύ
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

    // ֹͣCountDown���߳�
    public void CancelCountDownToken()
    {
        m_cts.Cancel();
        m_cts.Dispose();
        m_cts = new CancellationTokenSource();
    }
}
