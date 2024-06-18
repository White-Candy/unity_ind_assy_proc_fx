using LitJson;
using sugar;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public struct ConstrPtStep
{
    public string step; //当前步骤
    public string constrPt; // 施工要点
}

public enum AnimState
{
    None,
    Stop,
    Play
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

    private void Awake()
    {
        _Instance = this;
        DontDestroyOnLoad(this);

         ModelName = GlobalData.ModelTarget.modelName;
        // 获取 xxx.json 中的 当前步骤_施工要点
        NetworkManager._Instance.DownLoadTextFromServer(Application.streamingAssetsPath + "/ModelExplain/" + ModelName + ".json", (str) => 
        {
            Debug.Log(str);
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
    }

    public void Play(float f_start, float f_end)
    {
        // 设置相机

        StartCoroutine(PlayAnim(f_start, f_end));
    }

    private IEnumerator PlayAnim(float f_start, float f_end)
    {
        m_Animtor.SetBool("play", true);
        yield return new WaitForSeconds(0.1f);

        float start = f_start * (1 / 24.0f);
        float end = f_end * (1 / 24.0f);
        float animTime = (end - start); // f_start 和 f_end 两个帧时间间隔

        m_Animtor.PlayInFixedTime("Play", 0, start); // 从 start时间开始播放动画
        m_Animtor.speed = 1.0f;
        yield return new WaitForSeconds(animTime);

        // 播放完毕关闭动画
        m_Animtor.SetBool("play", false);
        yield return null;
    }
}
