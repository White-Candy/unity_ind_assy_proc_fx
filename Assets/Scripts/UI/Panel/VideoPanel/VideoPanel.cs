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
    public GameObject m_Item; // �б�itemģ��

    public GameObject m_ItemParent; // Item�ĸ��ؼ�

    private List<GameObject> m_Items = new List<GameObject>(); // ��������б�Item����

    public VideoControl m_Control;

    private bool m_PlayerIsStart = false; // ��Ƶ��ʼ����

    public GameObject m_VideoPanel; // ��Ƶ������������

    public RectTransform m_VideoRect; // ���Ŷ����Ŀؼ�RectTranform

    public Button m_Close; // �رմ���

    public Button m_MinMax; // �Ŵ���С����

    //��С��������
    public RectTransform m_MinParent;

    //��󻯸�����
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
            m_Control.Update(); // ��ʼÿ֡ˢ��ͼƬ���Ŷ���
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
    /// ���ڹر�
    /// </summary>
    public void Close()
    {
        m_Control.Close();
        m_PlayerIsStart = false;
        m_VideoPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// ���ڷŴ���С
    /// </summary>
    public void MinOrMax()
    {
        //���
        if (m_VideoRect.transform.parent == m_MinParent.transform)
        {
            m_VideoRect.transform.parent = m_MaxParent.transform;
        }
        else //��С��
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
/// ��Ƶ�������������
/// </summary>
[System.Serializable]
public class VideoControl
{
    // ���Ű�ť
    public Button m_Play; 

    // ��ͣ��ť
    public Button m_Pause;

    // ���²���
    public Button m_RePlay;

    // ��Ƶ������
    public Slider m_Slider;

    // ������ֵ
    public MethodInfo m_SliderValue;

    // ��Ƶ������
    public VideoPlayerControl m_Player;

    // ׼���׶�
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

        // ��ȡ��������ֵ�ı���ý������ķ���
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
    /// ���ý�����ֵ�������¼�
    /// </summary>
    private void SetSliderNoEvent(float f)
    {
        m_Slider.SetValueWithoutNotify(f);
        //m_SliderValue.Invoke(m_Slider, new object[] { f, false });
    }

    //��ʼ����
    public void Play()
    {
        m_Player.Play();
        SwitchPlayBtn(false);
    }

    // �ر�
    public void Close()
    {
        m_Player.Stop();
        SwitchPlayBtn(false);
    }

    // ����
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
/// ��Ƶ������
/// </summary>
[System.Serializable]
public class VideoPlayerControl
{
    // ý�岥�Ŷ���
    public VideoPlayer m_Player;

    // Audio Player
    public AudioSource m_Audio;

    // ��Ƶÿһ֡��ʾ��ͼƬ
    public RawImage m_RawImg;

    // Ĭ����Ƶ��ʾ��ͼƬ
    private Texture m_DefaultTexture;

    //��Ⱦ��ͼ
    private RenderTexture m_Texture;

    // ��ʱ��
    public double m_TotalTime { get { return m_Player.frameCount / m_Player.frameRate; } }

    //��ǰʱ��
    public double time { get { return m_Player.time; } set { m_Player.time = value; } }

    //��ǰʱ�䣨0-1��
    public float normalizeTime { get { return m_Player.frameCount == 0 ? 0 : 1.0f * m_Player.frame / m_Player.frameCount; } }

    //��ǰ֡��
    public long frame { get { return m_Player.frame; } set { m_Player.frame = value; } }

    //��֡��
    public ulong frameCount { get { return m_Player.frameCount; } }

    //�����ٶ�
    //public float speed { get { return m_Player.playbackSpeed; } set { m_Player.playbackSpeed = value; } }

    // ��Ƶ��Դ·��
    [HideInInspector]
    public string m_Path;

    //��Ƶ���
    [HideInInspector]
    public int m_VideoWidth = -1;

    //��Ƶ�߶�
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
    /// ��Ƶ������ɵ���
    /// </summary>
    private void LoadVideoCompleted(VideoPlayer p)
    {
        // ��Ƶ����Error
        if (p.texture == null || (p.texture.width == m_VideoWidth && p.texture.height == m_VideoHeight))
        {
            return;
        }

        m_Player.targetTexture = null;
        m_RawImg.texture = null;

        //����
        if (m_Texture != null)
        {
            RenderTexture.ReleaseTemporary(m_Texture);
            m_Texture = null;
        }

        m_VideoWidth = p.texture.width;
        m_VideoHeight = p.texture.height;

        //������Ƶ�ߴ紴��RenderTexture
        m_Texture = RenderTexture.GetTemporary(p.texture.width, p.texture.height);
        m_Player.targetTexture = m_Texture;
        m_RawImg.texture = m_Texture;

        // Debug.Log("Video Play");
        m_Player.Play();
    }

    /// <summary>
    /// ��������
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
    /// ����·������
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
    /// ��ͣ
    /// </summary>
    public void Pause()
    {
        if (m_Player.isPrepared)
        {
            m_Player.Pause();
        }
    }

    /// <summary>
    /// ֹͣ����
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
    /// ���²���
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
    /// ����
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
    /// ����
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
