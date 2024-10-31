using Cysharp.Threading.Tasks;
using LitJson;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Unity.XR.Oculus.Input;
using UnityEngine;

public enum AnimState
{
    None,
    Playing,
    Puase,
    Stop
}

public struct ConstrPtStep
{
    public string step; //��ǰ����
    public string constrPt; // ʩ��Ҫ��
}

public class ModelAnimControl : MonoBehaviour
{
    public static ModelAnimControl _Instance;

    public GameObject m_animCamera; // �������
    public GameObject m_player; // �������

    private Animator m_Animtor; // Animtor���

    //private float totalScore = 0;

    public List<ConstrPtStep> m_ConPtStep = new List<ConstrPtStep>();

    private string ModelName; //ģ�͵� adressables Group Default Name

    private AnimState m_AnimState = AnimState.None; // ��¼�����Ĳ���״̬

    private async void Awake()
    {
        _Instance = this;
    
    }

    private async void Start()
    {
        DontDestroyOnLoad(_Instance);
        m_Animtor = GetComponent<Animator>();

        ModelName = GlobalData.ProjGroupName;
        // ��ȡ xxx.json �е� ��ǰ����_ʩ��Ҫ��
        await NetworkManager._Instance.DownLoadTextFromServer(Application.streamingAssetsPath + "/ModelExplain/" + GlobalData.ProjGroupName + "\\TXT.json", (str) =>
        {
            // Debug.Log(str);
            JsonData js_data = JsonMapper.ToObject<JsonData>(str);
            foreach (var item in js_data.Keys)
            {
                //Debug.Log(item);
                string[] fields = js_data[item].ToString().Split('_');

                ConstrPtStep step = new ConstrPtStep();
                step.step = fields[0];
                step.constrPt = fields[1];
                m_ConPtStep.Add(step);
            }
        });

        CameraControl.player = m_player;
        CameraControl.animation = m_animCamera;
        CameraControl.SetPlayer();
        // Debug.Log("Start Slice");
        await Slice(0f, 0.5f);
    }

    void Update()
    {
        if (GlobalData.DestroyModel)
        {
            DataRecycling();
            GlobalData.DestroyModel = false;
        }

        // TODO..����Ҫ�ĳ��첽
        if (Input.GetKeyDown(KeyCode.Space) && m_AnimState == AnimState.Playing)
        {
            // Debug.Log("Space!!");
            m_Animtor.speed = m_Animtor.speed == 0f ? 1f : 0f;
        }
    }


    public async UniTask PlayAnim(float f_start, float f_end)
    {
        if (f_start == f_end)
        {
            await Slice(f_start, f_end);
            return;
        }

        //Debug.Log("PlayAnim..");
        // �л����������
        GameObject canvas = GameObject.Find("MainCanvas").gameObject;
        canvas.gameObject.SetActive(false); // ���Ŷ�����ʱ�� �ر�UI��
        // Debug.Log("Begin canvas active: " + canvas.gameObject.activeSelf);
        CameraControl.SetAnimation();

        GameMode.Instance.ArrowActive(false); // ���ؼ�ͷ

        // ������������UI
        // InfoPanel._instance.gameObject.SetActive(false); 

        if (AudioManager.Instance.m_IsPlaying) // ������ڲ��������ر�����
        {
            AudioManager.Instance.Pause();
        }

        await Slice(f_start, f_end);
        await UniTask.WaitUntil( () => 
        {
            return m_AnimState != AnimState.Playing;
        });

        CameraControl.SetPlayer(); // �л��� Player�����
        // InfoPanel._instance.gameObject.SetActive(true);
        canvas.gameObject.SetActive(true);
        // Debug.Log("Over canvas active: " + canvas.gameObject.activeSelf);

        // ���贰�ڰ�ť����
        if (GlobalData.stepStructs[GlobalData.StepIdx].stepName.Count() > 0)
        {
            int stepIdx = SelectStepPanel._instance.stepNameList.FindIndex(x => x == GlobalData.stepStructs[GlobalData.StepIdx].stepName);
            if (stepIdx >= 0)
                SelectStepPanel._instance.SetStepStatus(stepIdx, SelectStepPanel.EStepStatus.Finish);
        }

        GameMode.Instance.NextStep(); // ���Ž��� ��ʼ��һ��
    }

    public void Play()
    {
        m_Animtor.SetBool("play", true);
        m_Animtor.speed = 1.0f;
        m_AnimState = AnimState.Playing;
    }

    public void Stop()
    {
        m_Animtor.SetBool("play", false);
        m_AnimState = AnimState.Stop;
    }

    public void GoOn()
    {
        m_Animtor.speed = 1.0f;
        m_AnimState = AnimState.Playing;
    }

    public void Puase()
    {
        m_Animtor.speed = 0.0f;
        m_AnimState = AnimState.Puase;
    }

    // ���Ŷ���ĳһ��֡�Ķ���
    public async UniTask Slice(float f_start, float f_end)
    {
        // Debug.Log("In Slice!");
        float start = f_start * (1 / 30.0f);
        float end = f_end * (1 / 30.0f);
        float animTime = (end - start); // f_start �� f_end ����֡ʱ����

        Play();
        await UniTask.WaitForSeconds(0.1f);

        m_Animtor.PlayInFixedTime("Play", 0, start); // �� startʱ�俪ʼ���Ŷ���
        GoOn();

        await Delay(animTime);

        //Debug.Log("Close ANim");
        // ���������ͣ����
        Puase();
        await UniTask.Yield();
    }

    /// <summary>
    /// ���Կ��ƶ�������ͣ�Ͳ���
    /// ���ʹ�õĲ���Delay,����UniTask.WaitSecond()
    /// ��ô��Ȼ�ҵ�animtor.speed = 0������UniTask��ʵ�����ڵ���
    /// �������ǻ�����ͣ����û�в��Ž�����״̬����ͣ�˶�����
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private async UniTask Delay(float time)
    {
        while (time > 0)
        {
            if (m_Animtor.speed != 0)
            {
                time -= 0.11f;
            }
            // Debug.Log(time);
            await UniTask.Delay(100);
        }
    }

    /// <summary>
    /// ���ݻ���
    /// </summary>
    /// <param name="msg"></param>
    public void DataRecycling()
    {
        //Debug.Log("DataRecycling");
        CameraControl.SetMain();
        CameraControl.animation = null;
        CameraControl.player = null;
        Destroy(gameObject);
    }
}
