using LitJson;
using sugar;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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

    public GameObject m_animCamera; // 动画相机的名字
    public GameObject m_player; // 人物相机

    private Animator m_Animtor; // Animtor组件

    private float totalScore = 0;

    public List<ConstrPtStep> m_ConPtStep = new List<ConstrPtStep>();

    private string ModelName; //模型的 adressables Group Default Name

    private AnimState m_AnimState = AnimState.None; // 记录动画的播放状态

    private void Awake()
    {
        _Instance = this;
        DontDestroyOnLoad(_Instance);

        ModelName = GlobalData.ModelTarget.modelName;
        // 获取 xxx.json 中的 当前步骤_施工要点
        NetworkManager._Instance.DownLoadTextFromServer(Application.streamingAssetsPath + "/ModelExplain/" + ModelName + ".json", (str) =>
        {
            //Debug.Log(str);
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

        m_Animtor = GetComponent<Animator>();
    }

    private void Start()
    {
        CameraControl.player = m_player;
        CameraControl.animation = m_animCamera;
        CameraControl.SetPlayer();
        StartCoroutine(Slice(0f, 0f));
    }

    void Update()
    {
        if (GlobalData.DestroyModel)
        {
            DataRecycling();
            GlobalData.DestroyModel = false;
        }
    }


    public IEnumerator PlayAnim(float f_start, float f_end)
    {
        // 切换到动画相机
        GameObject canvas = GameObject.Find("Canvas").gameObject;
        Debug.Log(canvas.name);
        canvas.SetActive(false); // 播放动画的时候 关闭UI。
        CameraControl.SetAnimation();
        GameMode.Instance.ArrowActive(false); // 隐藏箭头

        StartCoroutine(Slice(f_start, f_end));
        yield return new WaitUntil( () => 
        {
            return m_AnimState != AnimState.Playing;
        });

        CameraControl.SetPlayer(); // 切换回 Player相机。
        canvas.SetActive(true);
        GameMode.Instance.NextStep();
    }

    public void Play()
    {
        m_Animtor.SetBool("play", true);
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
    public IEnumerator Slice(float f_start, float f_end)
    {
        //Debug.Log("In Slice!");
        float start = f_start * (1 / 24.0f);
        float end = f_end * (1 / 24.0f);
        float animTime = (end - start); // f_start 和 f_end 两个帧时间间隔

        Play();
        yield return new WaitForSeconds(0.1f);

        m_Animtor.PlayInFixedTime("Play", 0, start); // 从 start时间开始播放动画
        GoOn();
        yield return new WaitForSeconds(animTime);

        //Debug.Log("Close ANim");
        // 播放完毕暂停动画
        Puase();
        yield return null;
    }

    /// <summary>
    /// 数据回收
    /// </summary>
    /// <param name="msg"></param>
    public void DataRecycling()
    {
        Debug.Log("DataRecycling");
        CameraControl.SetMain();
        CameraControl.animation = null;
        CameraControl.player = null;
        Destroy(gameObject);
    }
}
