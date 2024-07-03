using sugar;
using System.Collections;
using System.Collections.Generic;
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

    private bool m_Prepare;

    private GameMethod m_Method; // 该步骤的游戏方法
    private GameState m_State;

    [HideInInspector]
    public List<string> m_Tools = new List<string>(); // 目前步骤需要处理的工具
    private int m_ToolIdx = 0; // 目前步骤工具的索引

    private string currToolName = ""; // 玩家操作的的这个工具的名字

    [HideInInspector]
    public float m_Score; // 考核模式一个步骤的分数

    private void FixedUpdate()
    {
        StateMachine();
    }

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
            //m_Tools.Clear();
            m_Tools = GlobalData.stepStructs[GlobalData.StepIdx].tools;
            //Debug.Log("Prepare: " + m_Tools.Count);
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
        if (m_ToolIdx < m_Tools.Count && currToolName == m_Tools[m_ToolIdx])
        {
            m_ToolIdx++;
            currToolName = "";
            if (m_ToolIdx >= m_Tools.Count)
            {
                float start = float.Parse(GlobalData.stepStructs[GlobalData.StepIdx].animLimite[0]);
                float end = float.Parse(GlobalData.stepStructs[GlobalData.StepIdx].animLimite[1]);
                await ModelAnimControl._Instance.PlayAnim(start, end);
            }
        }
    }

    private void PrepareDragStep()
    {
        m_State = GameState.Playing; // 准备阶段结束，进入游戏阶段
    }

    private void PrepareClickStep()
    {
        string tool = m_Tools[m_ToolIdx];
        GameObject go = GameObject.Find(tool);
        if (go != null)
        {
            //HighlightableObject ho = go.AddComponent<HighlightableObject>();
            //ho.FlashingOn(Color.green, Color.red, 2f);
        }
        m_State = GameState.Playing; // 准备阶段结束，进入游戏阶段
    }

    public void ArrowActive(bool b)
    {
        m_Arrow.SetActive(b);
    }

    public void SetToolName(string name)
    {
        currToolName = name;
    }

    public void NextStep()
    {
        if (GlobalData.StepIdx + 1 < GlobalData.stepStructs.Count)
        {
            GlobalData.StepIdx++;
            SetStep(GlobalData.StepIdx);
        }
    }

    // 用户可以选择不同的步骤进行游戏
    public async void SetStep(int i)
    {
        //Debug.Log("Step: " + i + " || " + GlobalData.stepStructs.Count);
        if (i >= 0 && i < GlobalData.stepStructs.Count)
        {
            GlobalData.StepIdx = i;
            m_Prepare = false;
            m_ToolIdx = 0;
            currToolName = "";

            float frame = float.Parse(GlobalData.stepStructs[i].animLimite[0]); // 这是为了 显示这一步场景中模型的状态[每一步模型都会改变]
            await ModelAnimControl._Instance.Slice(frame, frame);
            Prepare();
        }
    }

    // 目前还需要几个工具才能激活动画
    public string NumberOfToolsRemaining()
    {
        return (m_Tools.Count - m_ToolIdx).ToString();
    }
}
