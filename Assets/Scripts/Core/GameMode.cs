using sugar;
using System.Collections;
using System.Collections.Generic;
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

    public Queue<string> m_Tools = new Queue<string>(); // 目前步骤需要处理的工具


    private void FixedUpdate()
    {
        StateMachine();
    }

    private void StateMachine()
    {
        //if (m_State == GameState.Prepare && m_Tools.Count > 0)
        //{
        //    if (m_Method == GameMethod.Clicked)
        //    {
        //        PrepareClickStep(); // 点击模式下，根据不同string，实现不同工具的闪烁
        //    }
        //}
    }

    public void Start()
    {
        Prepare();
    }

    /// <summary>
    /// 每次步骤的最终目的是播放动画，
    /// 但是有的是拖放播放动画，有的是点击播放动画，所以需要根据不同的情况编写。
    /// </summary>
    public void Prepare()
    {
        if (!m_Prepare)
        {
            string method = GlobalData.stepStructs[GlobalData.StepIdx].method;
            foreach (var tool in GlobalData.stepStructs[GlobalData.StepIdx].tools)
            {
                m_Tools.Enqueue(tool); // 新的步骤更新新的工具库
            }

            if (method == "点击")
            {
                m_Method = GameMethod.Clicked;
                CameraControl.target.AddComponent<HighlightingEffect>();
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

    public static void PerformThisStep()
    {
        
    }

    public void ArrowActive(bool b)
    { 
        m_Arrow.SetActive(b); 
    }

    private void PrepareDragStep()
    {
        
    }

    private void PrepareClickStep()
    {
        string tool = m_Tools.Dequeue();
        GameObject go = GameObject.Find(tool);
        if (go != null)
        {
            HighlightableObject ho = go.AddComponent<HighlightableObject>();
            ho.FlashingOn(Color.green, Color.red, 2f);
        }
        m_State = GameState.Playing; // 准备阶段结束，进入游戏阶段
    }

    public void NextStep()
    {
        if (GlobalData.StepIdx < GlobalData.stepStructs.Count)
        {
            GlobalData.StepIdx++;
            Prepare();
        }
    }

    // 用户可以选择不同的步骤进行游戏
    public void SetStep(int i)
    {
        if (i > 0 && i < GlobalData.stepStructs.Count)
        {
            GlobalData.StepIdx = i;
            Prepare();
        }
    }
}
