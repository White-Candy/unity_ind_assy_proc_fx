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

public enum GameState // Ŀǰ��һ����״̬
{
    Prepare,
    Playing,
    Stop
}

public class GameMode : Singleton<GameMode>
{
    public GameObject m_Arrow; // ��ͷ[����ʶ�𹤾��Ƿ���ȷ��]
    private List<GameObject> m_arrowTrans = new List<GameObject>(); // ��ͷ��ͬ�����λ��
    public List<AudioClip> m_AudioClip = new List<AudioClip>(); // ÿ������ǰ��������ʾ
    private bool m_Prepare;
    private GameMethod m_Method; // �ò������Ϸ����
    private GameState m_State;

    [HideInInspector]
    public List<string> m_Tools = new List<string>(); // Ŀǰ������Ҫ����Ĺ���

    // Ŀǰ���蹤�ߵ����� (�ϰ�˵����ֻ���ա�˳�򡿶�ȡ���ߣ��û������򡿽��б�����Ĺ���ҲҪ���� QAQ...
    // ...���ԾͲ���Ҫ�����������idxȥ�����ˣ�ֱ��ȥ find �ͺ�
    // [��ȥдһ�� checkTool ���Ƶĺ������ж��û���ק�Ĺ����Ƿ����������{m_Tools}�а�����])
    // private int m_ToolIdx = 0; 

    private string currToolName = ""; // ��Ҳ����ĵ�������ߵ�����

    [HideInInspector]
    public float oneStepScore; // ����ģʽһ������ķ���

    [HideInInspector]
    public float totalScore; // �û�����ʵѵ�����ܳɼ�

    private bool m_Init = false; // ��ʼ���ɹ�

    public async void Start()
    {
        await UniTask.WaitUntil(() => GlobalData.stepStructs.Count != 0);
        m_arrowTrans = GameObject.FindGameObjectsWithTag("trans").ToList();
        Debug.Log("GameMode Start: " + m_arrowTrans.Count);

        // ��ʾ����λ�õĵ���
        for (int i = 0; i < m_arrowTrans.Count && i < GlobalData.stepStructs.Count; i++)
        {
            // Debug.Log("Looper: " + m_arrowTrans[i].position);
            var step_info = GlobalData.stepStructs[i];
            step_info.arrowTrans = m_arrowTrans[i].transform;
            GlobalData.stepStructs[i] = step_info;
        }

        // ��Ƶװ��
        for (int i = 0; i < m_AudioClip.Count && i < GlobalData.stepStructs.Count; i++)
        {
            // Debug.Log("Looper: " + m_arrowTrans[i].position);
            var step_info = GlobalData.stepStructs[i];
            step_info.clip = m_AudioClip[i];
            GlobalData.stepStructs[i] = step_info;
        }

        Debug.Log("mode: " + GlobalData.mode);
        // ���ŵ�һ����ʾ��
        if (GlobalData.mode != Mode.Examination)
        {
            //Debug.Log("Audio Play");
            // AudioManager.Instance.Play(GlobalData.stepStructs[GlobalData.StepIdx].clip);
        }
        m_Init = true;

        // ����ģʽ��ʾ ����ģʽ��UI
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
            // ���ģʽ�£����ݲ�ͬstring��ʵ�ֲ�ͬ���ߵ���˸
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
    /// ÿ�β��������Ŀ���ǲ��Ŷ�����
    /// �����е����ϷŲ��Ŷ������е��ǵ�����Ŷ�����������Ҫ���ݲ�ͬ�������д��
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
            if (method == "���")
            {
                m_Method = GameMethod.Clicked;
                //CameraControl.target.AddComponent<HighlightingEffect>();
                transform.AddComponent<ClickMethod>();
                ArrowActive(false);
            }
            else
            {
                m_Method = GameMethod.Drag;

                // ��ק����ǰ��׼������
                ArrowActive(true);
            }
            m_Prepare = true;
            m_State = GameState.Prepare;
        }
    }

    /// <summary>
    /// ������һ�� ��ק/�����������Ϣ
    /// </summary>
    /// <param name="name"></param>
    public async void PerformThisStep()
    {
        if (m_Tools.Count > 0 && checkToolInThisStep())
        {
            m_Tools.Remove(currToolName);
            currToolName = "";

            // ���������Ҫ�õ��Ĺ����û��Ѿ�ȫ���õ��ˣ��Ϳ��Բ��Ŷ�����
            if (m_Tools.Count == 0)
            {   
                if (GlobalData.mode == Mode.Examination) totalScore += oneStepScore;
                //Debug.Log($"name : {GlobalData.stepStructs[GlobalData.StepIdx].stepName}, {GlobalData.stepStructs[GlobalData.StepIdx].animLimite[0]} | {GlobalData.stepStructs[GlobalData.StepIdx].animLimite[1]}");
                float start = float.Parse(GlobalData.stepStructs[GlobalData.StepIdx].animLimite[0]);
                float end = float.Parse(GlobalData.stepStructs[GlobalData.StepIdx].animLimite[1]);
                await ModelAnimControl._Instance.PlayAnim(start, end); // ����������̲���Ķ���
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
        m_State = GameState.Playing; // ׼���׶ν�����������Ϸ�׶�
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
        m_State = GameState.Playing; // ׼���׶ν�����������Ϸ�׶�
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
        // UnityEventCenter.DistributeEvent(EnumDefine.EventKey.NotificationCallback, null); // ����һ��ʵѵ���˳ɼ�body�ڴ�����
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
    /// �û�����ѡ��ͬ�Ĳ��������Ϸ
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
                // TODO..�ⲿ�ִ�����ܴ������࣬����Ҫ�޸�
                float frame = float.Parse(GlobalData.stepStructs[i].animLimite[0]);
                // Debug.Log(frame);
                await ModelAnimControl._Instance.Slice(frame, frame + 0.1f); // ����Ϊ�� ��ʾ��һ��������ģ�͵�״̬[ÿһ��ģ�Ͷ���ı�]
                //AudioManager.Instance.Play(GlobalData.stepStructs[i].clip);
            }
            Prepare();
        }
    }

    // Ŀǰ����Ҫ�������߲��ܼ����
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
