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
    public string step; //当前步骤
    public string constrPt; // 施工要点
}

public class ModelAnimControl : MonoBehaviour
{
    public static ModelAnimControl _Instance;

    public GameObject m_animCamera; // 动画相机
    public GameObject m_player; // 人物相机

    private Animator m_Animtor; // Animtor组件

    //private float totalScore = 0;

    public List<ConstrPtStep> m_ConPtStep = new List<ConstrPtStep>();

    private string ModelName; //模型的 adressables Group Default Name

    private AnimState m_AnimState = AnimState.None; // 记录动画的播放状态

    private async void Awake()
    {
        _Instance = this;
    
    }

    private async void Start()
    {
        DontDestroyOnLoad(_Instance);
        m_Animtor = GetComponent<Animator>();

        ModelName = GlobalData.ProjGroupName;
        // 获取 xxx.json 中的 当前步骤_施工要点
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

        // TODO..后面要改成异步
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
        // 切换到动画相机
        GameObject canvas = GameObject.Find("MainCanvas").gameObject;
        canvas.gameObject.SetActive(false); // 播放动画的时候 关闭UI。
        // Debug.Log("Begin canvas active: " + canvas.gameObject.activeSelf);
        CameraControl.SetAnimation();

        GameMode.Instance.ArrowActive(false); // 隐藏箭头

        // 动画播放隐藏UI
        // InfoPanel._instance.gameObject.SetActive(false); 

        if (AudioManager.Instance.m_IsPlaying) // 如果还在播放声音关闭声音
        {
            AudioManager.Instance.Pause();
        }

        await Slice(f_start, f_end);
        await UniTask.WaitUntil( () => 
        {
            return m_AnimState != AnimState.Playing;
        });

        CameraControl.SetPlayer(); // 切换回 Player相机。
        // InfoPanel._instance.gameObject.SetActive(true);
        canvas.gameObject.SetActive(true);
        // Debug.Log("Over canvas active: " + canvas.gameObject.activeSelf);

        // 步骤窗口按钮更新
        if (GlobalData.stepStructs[GlobalData.StepIdx].stepName.Count() > 0)
        {
            int stepIdx = SelectStepPanel._instance.stepNameList.FindIndex(x => x == GlobalData.stepStructs[GlobalData.StepIdx].stepName);
            if (stepIdx >= 0)
                SelectStepPanel._instance.SetStepStatus(stepIdx, SelectStepPanel.EStepStatus.Finish);
        }

        GameMode.Instance.NextStep(); // 播放结束 开始下一步
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

    // 播放动画某一段帧的动画
    public async UniTask Slice(float f_start, float f_end)
    {
        // Debug.Log("In Slice!");
        float start = f_start * (1 / 30.0f);
        float end = f_end * (1 / 30.0f);
        float animTime = (end - start); // f_start 和 f_end 两个帧时间间隔

        Play();
        await UniTask.WaitForSeconds(0.1f);

        m_Animtor.PlayInFixedTime("Play", 0, start); // 从 start时间开始播放动画
        GoOn();

        await Delay(animTime);

        //Debug.Log("Close ANim");
        // 播放完毕暂停动画
        Puase();
        await UniTask.Yield();
    }

    /// <summary>
    /// 可以控制动画的暂停和播放
    /// 如果使用的不是Delay,而是UniTask.WaitSecond()
    /// 那么虽然我的animtor.speed = 0，但是UniTask其实还是在倒数
    /// 这样还是会在暂停动画没有播放结束的状态，暂停了动画！
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
    /// 数据回收
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
