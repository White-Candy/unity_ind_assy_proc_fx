using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum GameMethod
{
    None,
    Drag,
    Clicked
}

public enum GameState // 目前这一步的状态
{
    Prepare,
    Playing,
    Stop
}

public class GameMode : Singleton<GameMode>
{
    public GameObject m_Arrow; // 箭头[用来识别工具是否正确的]
    private List<GameObject> m_arrowTrans = new List<GameObject>(); // 箭头不同步骤的位置
    public List<AudioClip> m_AudioClip = new List<AudioClip>(); // 每个步骤前的语音提示
    private bool m_Prepare;
    private GameMethod m_Method; // 该步骤的游戏方法
    private GameState m_State;

    [HideInInspector]
    public List<string> m_Tools = new List<string>(); // 目前步骤需要处理的工具

    // 目前步骤工具的索引 (老板说不能只按照【顺序】读取工具，用户【乱序】进行本步骤的工具也要可以 QAQ...
    // ...所以就不需要向队列这样用idx去对照了，直接去 find 就好
    // [会去写一个 checkTool 类似的函数来判断用户拖拽的工具是否在这个步骤{m_Tools}中包含。])
    // private int m_ToolIdx = 0; 

    private string currToolName = ""; // 玩家操作的的这个工具的名字

    [HideInInspector]
    public float oneStepScore; // 考核模式一个步骤的分数

    [HideInInspector]
    public float totalScore; // 用户作答实训考试总成绩

    private bool m_Init = false; // 初始化成功

    public async void Start()
    {
        await UniTask.WaitUntil(() => GlobalData.stepStructs.Count != 0);
        m_arrowTrans = GameObject.FindGameObjectsWithTag("trans").ToList();
        Debug.Log("GameMode Start: " + m_arrowTrans.Count);

        // 提示物体位置的调整
        for (int i = 0; i < m_arrowTrans.Count && i < GlobalData.stepStructs.Count; i++)
        {
            // Debug.Log("Looper: " + m_arrowTrans[i].position);
            var step_info = GlobalData.stepStructs[i];
            step_info.arrowTrans = m_arrowTrans[i].transform;
            GlobalData.stepStructs[i] = step_info;
        }

        // 音频装载
        for (int i = 0; i < m_AudioClip.Count && i < GlobalData.stepStructs.Count; i++)
        {
            // Debug.Log("Looper: " + m_arrowTrans[i].position);
            var step_info = GlobalData.stepStructs[i];
            step_info.clip = m_AudioClip[i];
            GlobalData.stepStructs[i] = step_info;
        }

        Debug.Log("mode: " + GlobalData.mode);
        // 播放第一个提示音
        if (GlobalData.mode != Mode.Examination)
        {
            //Debug.Log("Audio Play");
            // AudioManager.Instance.Play(GlobalData.stepStructs[GlobalData.StepIdx].clip);
        }
        m_Init = true;

        // 考核模式显示 考核模式的UI
        if (GlobalData.mode == Mode.Examination)
        {
            InfoPanel._instance.SetActiveOfExamUI(true);
            await InfoPanel._instance.StartCountDown().SuppressCancellationThrow();
        }
        else
        {
            InfoPanel._instance.SetActiveOfExamUI(false);
        }
    }

    public void Update() { StateMachine(); }

    private void StateMachine()
    {
        if (m_State == GameState.Prepare && m_Tools.Count > 0)
        {
            // 点击模式下，根据不同string，实现不同工具的闪烁
            if (m_Method == GameMethod.Clicked)
            {
                PrepareClickStep();
            }
            else
            {
                PrepareDragStep();
            }
        }
        else if (m_State == GameState.Playing && currToolName.Length > 0)
        {
            PerformThisStep();
        }
    }

    /// <summary>
    /// 每次步骤的最终目的是播放动画，
    /// 但是有的是拖放播放动画，有的是点击播放动画，所以需要根据不同的情况编写。
    /// </summary>
    public void Prepare()
    {
        if (!m_Prepare)
        {
            string method = GlobalData.stepStructs?[GlobalData.StepIdx].method;

            m_Tools.Clear();
            foreach (var t in GlobalData.stepStructs[GlobalData.StepIdx].tools)
            {
                m_Tools.Add(t);
            }

            // Debug.Log("Prepare: " + m_Tools.Count);
            if (method == "点击")
            {
                m_Method = GameMethod.Clicked;
                //CameraControl.target.AddComponent<HighlightingEffect>();
                transform.AddComponent<ClickMethod>();
                ArrowActive(false);
            }
            else
            {
                m_Method = GameMethod.Drag;

                // 拖拽方法前的准备工作
                ArrowActive(true);
            }
            m_Prepare = true;
            m_State = GameState.Prepare;
        }
    }

    /// <summary>
    /// 处理这一次 拖拽/点击的物体信息
    /// </summary>
    /// <param name="name"></param>
    public async void PerformThisStep()
    {
        if (m_Tools.Count > 0 && checkToolInThisStep())
        {
            m_Tools.Remove(currToolName);
            currToolName = "";

            // 如果本次需要用到的工具用户已经全部用到了，就可以播放动画了
            if (m_Tools.Count == 0)
            {   
                if (GlobalData.mode == Mode.Examination) totalScore += oneStepScore;
                //Debug.Log($"name : {GlobalData.stepStructs[GlobalData.StepIdx].stepName}, {GlobalData.stepStructs[GlobalData.StepIdx].animLimite[0]} | {GlobalData.stepStructs[GlobalData.StepIdx].animLimite[1]}");
                float start = float.Parse(GlobalData.stepStructs[GlobalData.StepIdx].animLimite[0]);
                float end = float.Parse(GlobalData.stepStructs[GlobalData.StepIdx].animLimite[1]);
                await ModelAnimControl._Instance.PlayAnim(start, end); // 播放这次流程步骤的动画
            }
        }
    }

    public bool checkToolInThisStep()
    {
        return m_Tools.Find(x => x == currToolName) == currToolName;
    }

    public async UniTask UpdateArrowTrans()
    {
        if (GlobalData.stepStructs != null && GlobalData.stepStructs.Count > 0)
        {
            await UniTask.WaitUntil(() => m_Init == true);
            // Debug.Log($"PrepareDragStep: {GlobalData.StepIdx}");
            // Debug.Log($"PrepareDragStep: {GlobalData.stepStructs[GlobalData.StepIdx].arrowTrans}");

            var new_trans = GlobalData.stepStructs[GlobalData.StepIdx].arrowTrans;
            m_Arrow.transform.localPosition = new_trans.localPosition;
            m_Arrow.transform.localRotation = new_trans.localRotation;
        }
    }

    private async void PrepareDragStep()
    {
        await UpdateArrowTrans();
        m_State = GameState.Playing; // 准备阶段结束，进入游戏阶段
    }

    private void PrepareClickStep()
    {
        // string tool = m_Tools[m_ToolIdx];
        // GameObject go = GameObject.Find(tool);
        // if (go != null)
        // {
        //     //HighlightableObject ho = go.AddComponent<HighlightableObject>();
        //     //ho.FlashingOn(Color.green, Color.red, 2f);
        // }
        m_State = GameState.Playing; // 准备阶段结束，进入游戏阶段
    }

    public void ArrowActive(bool b)
    {
        m_Arrow.SetActive(b);
    }

    public void SetToolName(string name)
    {
        //Debug.Log("SetToolName");
        currToolName = name;
    }

    public void NextStep()
    {
        // UnityEventCenter.DistributeEvent(EnumDefine.EventKey.NotificationCallback, null); // 更新一下实训考核成绩body内存内容
        if (GlobalData.StepIdx + 1 < GlobalData.stepStructs.Count)
        {
            GlobalData.StepIdx++;
            //StateMachine();

            SetStep(GlobalData.StepIdx);
        }
        else
        {
            GlobalData.StepIdx++;
            m_Arrow.SetActive(false);
        }
    }

    /// <summary>
    /// 用户可以选择不同的步骤进行游戏
    /// </summary>
    /// <param name="i"></param>
    /// <param name="isplay"></param>
    public async void SetStep(int i, bool isplay = true)
    {
        // Debug.Log("Step: " + i + " || " + GlobalData.stepStructs.Count);
        if (i >= 0 && i < GlobalData.stepStructs.Count)
        {
            GlobalData.StepIdx = i;
            m_Prepare = false;
            // m_ToolIdx = 0;
            currToolName = "";

            if (isplay)
            {
                // TODO..这部分代码可能存在冗余，后续要修改
                float frame = float.Parse(GlobalData.stepStructs[i].animLimite[0]);
                // Debug.Log(frame);
                await ModelAnimControl._Instance.Slice(frame, frame + 0.1f); // 这是为了 显示这一步场景中模型的状态[每一步模型都会改变]
                //AudioManager.Instance.Play(GlobalData.stepStructs[i].clip);
            }
            Prepare();
        }
    }

    // 目前还需要几个工具才能激活动画
    public string NumberOfToolsRemaining()
    {
        return m_Tools.Count.ToString();
    }

    public void OnDestroy()
    {
        // Debug.Log("GameMode On Destroy!");
        m_Init = false;
        GlobalData.stepStructs = new List<StepStruct>();
    }

    // private void UpdateRealBody(IMessage msg)
    // {
    //     if (GlobalData.mode != Mode.Examination) { return; }
    //     List<AnswerDetailVoListItem> realList = new List<AnswerDetailVoListItem>();

    //     AnswerDetailVoListItem avi = new AnswerDetailVoListItem
    //     {
    //         resourceId = GlobalData.codeVSidDic[GlobalData.ProjGroupName],
    //         userScore = GlobalData.totalScore.ToString()
    //     };
    //     realList.Add(avi);

    //     //GlobalData.m_RealExamBody = realList;
    // }
}
