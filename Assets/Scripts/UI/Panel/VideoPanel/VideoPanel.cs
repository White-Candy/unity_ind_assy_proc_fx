using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPanel : BasePanel
{
    public GameObject m_Item; // 列表item模板

    public GameObject m_ItemParent; // Item的父控件

    private List<GameObject> m_Items = new List<GameObject>(); // 用来存放列表Item对象

    public VideoControl m_Control;

    private bool m_PlayerIsStart = false; // 视频开始播放

    public GameObject m_VideoPanel; // 视频播放器主界面

    public RectTransform m_VideoRect; // 播放动画的控件RectTranform

    public Button m_Close; // 关闭窗口

    public Button m_MinMax; // 放大缩小窗口

    //最小化父物体
    public RectTransform m_MinParent;

    //最大化父物体
    public RectTransform m_MaxParent;

    private void Start()
    {
        m_Close.onClick.AddListener(Close);
        m_MinMax.onClick.AddListener(MinOrMax);
    }

    private void Update()
    {
        if (m_PlayerIsStart)
        {
            m_Control.Update(); // 开始每帧刷新图片播放动画
        }
    }

    public void Init(List<string> paths)
    {
        SpawnVideoItem(paths);
    }

    private void SpawnVideoItem(List<string> paths)
    {
        foreach (string path in paths)
        {
            GameObject item = GameObject.Instantiate(m_Item, m_ItemParent.transform);
            item.gameObject.SetActive(true);

            Button itemBtn = item.GetComponent<Button>();
            string videoName = Path.GetFileNameWithoutExtension(path);
            itemBtn.GetComponentInChildren<TextMeshProUGUI>().text = videoName;
            itemBtn.onClick.AddListener(() => { OnItemClick(path);  });
            m_Items.Add(item);
        }
    }

    private void OnItemClick(string path)
    {
        //Debug.Log(path);
        m_Control.Prepare(path);
        m_PlayerIsStart = true;

        m_VideoPanel.SetActive(true);
        m_Control.Play();
    }

    public void Exit()
    {
        if (this != null)
        {
            foreach (GameObject item in m_Items)
            {
                item.gameObject.SetActive(false);
                Destroy(item);
            }
            m_Items.Clear();

            Close();
            m_VideoPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 窗口关闭
    /// </summary>
    public void Close()
    {
        m_Control.Close();
        m_PlayerIsStart = false;
        m_VideoPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 窗口放大缩小
    /// </summary>
    public void MinOrMax()
    {
        //最大化
        if (m_VideoRect.transform.parent == m_MinParent.transform)
        {
            m_VideoRect.transform.parent = m_MaxParent.transform;
        }
        else //最小化
        {
            m_VideoRect.transform.parent = m_MinParent.transform;
        }

        m_VideoRect.transform.localScale = Vector3.one;
        m_VideoRect.offsetMin = Vector2.zero;
        m_VideoRect.offsetMax = Vector2.zero;
    }

    private void OnDestroy()
    {
        m_Control.Destroy();
    }
}

/// <summary>
/// 视频播放器控制面板
/// </summary>
[System.Serializable]
public class VideoControl
{
    // 播放按钮
    public Button m_Play; 

    // 暂停按钮
    public Button m_Pause;

    // 重新播放
    public Button m_RePlay;

    // 视频进度条
    public Slider m_Slider;

    // 进度条值
    public MethodInfo m_SliderValue;

    // 视频播放器
    public VideoPlayerControl m_Player;

    // 准备阶段
    public void Prepare(string url)
    {
        m_Player.Prepare(url);

        m_Play?.onClick.AddListener(() =>
        {
            m_Player.Play();
            SwitchPlayBtn(false);
        });

        m_Pause?.onClick.AddListener(() =>
        {
            m_Player.Pause();
            SwitchPlayBtn(true);
        });

        m_RePlay?.onClick.AddListener(() =>
        {
            m_Player.RePlay();
            SwitchPlayBtn(false);
        });

        m_Slider?.onValueChanged.AddListener((a) =>
        {
            m_Player.frame = (long)(a * m_Player.frameCount);
        });

        // 获取进度条数值改变调用进度条的方法
        if (m_Slider != null)
        {
            GetSliderMethod();
        }
    }

    public void Update()
    {
        m_Player.Update();
        if (m_Slider != null)
        {
            SetSliderNoEvent(m_Player.normalizeTime);
        }
    }

    /// <summary>
    /// 设置进度条值不触发事件
    /// </summary>
    private void SetSliderNoEvent(float f)
    {
        m_Slider.SetValueWithoutNotify(f);
        //m_SliderValue.Invoke(m_Slider, new object[] { f, false });
    }

    //开始播放
    public void Play()
    {
        m_Player.Play();
        SwitchPlayBtn(false);
    }

    // 关闭
    public void Close()
    {
        m_Player.Stop();
        SwitchPlayBtn(false);
    }

    // 销毁
    public void Destroy()
    {
        m_Player.Destroy();
    }

    private void SwitchPlayBtn(bool b)
    {
        m_Play.gameObject.SetActive(b);
        m_Pause.gameObject.SetActive(!b);
    }

    private void GetSliderMethod()
    {
        Type type = m_Slider.GetType();
        m_SliderValue = type.GetMethod("Set", BindingFlags.Instance | BindingFlags.NonPublic,
            null, new Type[] { typeof(float), typeof(bool) }, null);
    }
}

/// <summary>
/// 视频播放器
/// </summary>
[System.Serializable]
public class VideoPlayerControl
{
    // 媒体播放对象
    public VideoPlayer m_Player;

    // Audio Player
    public AudioSource m_Audio;

    // 视频每一帧显示的图片
    public RawImage m_RawImg;

    // 默认视频显示的图片
    private Texture m_DefaultTexture;

    //渲染贴图
    private RenderTexture m_Texture;

    // 总时间
    public double m_TotalTime { get { return m_Player.frameCount / m_Player.frameRate; } }

    //当前时间
    public double time { get { return m_Player.time; } set { m_Player.time = value; } }

    //当前时间（0-1）
    public float normalizeTime { get { return m_Player.frameCount == 0 ? 0 : 1.0f * m_Player.frame / m_Player.frameCount; } }

    //当前帧数
    public long frame { get { return m_Player.frame; } set { m_Player.frame = value; } }

    //总帧数
    public ulong frameCount { get { return m_Player.frameCount; } }

    //播放速度
    //public float speed { get { return m_Player.playbackSpeed; } set { m_Player.playbackSpeed = value; } }

    // 视频资源路径
    [HideInInspector]
    public string m_Path;

    //视频宽度
    [HideInInspector]
    public int m_VideoWidth = -1;

    //视频高度
    [HideInInspector]
    public int m_VideoHeight = -1;

    public void Prepare(string url)
    {
        m_DefaultTexture = m_RawImg.texture;

        m_Player.source = VideoSource.Url;
        m_Player.audioOutputMode = VideoAudioOutputMode.AudioSource;

        m_Player.EnableAudioTrack(0, true);
        m_Player.SetTargetAudioSource(0, m_Audio);
        m_Player.controlledAudioTrackCount = 1;
        m_Audio.volume = 1.0f;
        m_Player.playOnAwake = false;
        m_Player.IsAudioTrackEnabled(0);

        m_Player.url = url;
        m_Player.prepareCompleted += LoadVideoCompleted;
        m_Player.Prepare();
    }

    /// <summary>
    /// 视频加载完成调用
    /// </summary>
    private void LoadVideoCompleted(VideoPlayer p)
    {
        // 视频加载Error
        if (p.texture == null || (p.texture.width == m_VideoWidth && p.texture.height == m_VideoHeight))
        {
            return;
        }

        m_Player.targetTexture = null;
        m_RawImg.texture = null;

        //销毁
        if (m_Texture != null)
        {
            RenderTexture.ReleaseTemporary(m_Texture);
            m_Texture = null;
        }

        m_VideoWidth = p.texture.width;
        m_VideoHeight = p.texture.height;

        //根据视频尺寸创建RenderTexture
        m_Texture = RenderTexture.GetTemporary(p.texture.width, p.texture.height);
        m_Player.targetTexture = m_Texture;
        m_RawImg.texture = m_Texture;

        Debug.Log("Video Play");
        m_Player.Play();
    }

    /// <summary>
    /// 启动播放
    /// </summary>
    public void Play()
    {
        if (m_Player.isPrepared)
        {
            m_Player.Play();
            m_Audio.Play();
        }
    }

    /// <summary>
    /// 载入路径播放
    /// </summary>
    public void Play(string path)
    {
        if (!string.IsNullOrEmpty(path))
        {
            m_Path = path;
            m_Player.url = path;
            m_Player.Play();
        }
    }

    /// <summary>
    /// 暂停
    /// </summary>
    public void Pause()
    {
        if (m_Player.isPrepared)
        {
            m_Player.Pause();
        }
    }

    /// <summary>
    /// 停止播放
    /// </summary>
    public void Stop()
    {
        m_VideoWidth = 0;
        m_VideoHeight = 0;
        m_Player.prepareCompleted -= LoadVideoCompleted;

        //Debug.Log("Video Stop");
        m_Player.Stop();
    }

    /// <summary>
    /// 重新播放
    /// </summary>
    public void RePlay()
    {
        if (m_Player.isPrepared)
        {
            m_Player.time = 0;
            m_Player.Play();
        }
        else
        {
            Play(m_Path);
        }
    }

    /// <summary>
    /// 更新
    /// </summary>
    public void Update()
    {
        if (1.0f * m_Player.frame >= m_Player.frameCount || 1.0f * m_Player.frame <= 0)
        {
            m_RawImg.texture = m_DefaultTexture;
        }
        else
        {
            m_RawImg.texture = m_Texture;
        }
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public void Destroy()
    {
        // Debug.Log("Video Destroy");
        if (m_Texture != null)
        {
            RenderTexture.ReleaseTemporary(m_Texture);
            m_Texture = null;
        }
        m_Player.prepareCompleted -= LoadVideoCompleted;
        m_VideoWidth = 0;
        m_VideoHeight = 0;
    }
}
