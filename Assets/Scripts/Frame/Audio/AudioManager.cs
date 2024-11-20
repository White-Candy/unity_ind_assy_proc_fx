using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    private AudioSource m_AudioSource;

    public bool m_IsPlaying 
    { 
        get 
        {
            if (m_AudioSource != null)
            {
                return m_AudioSource.isPlaying;
            }
            return false;
        }
    }

    public override void Awake()
    {
        base.Awake();
        Debug.Log("Audio Awake");
        if (m_AudioSource == null)
        {
            DontDestroyOnLoad(gameObject);
            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.playOnAwake = false;
            //gameObject.TryGetComponent(out m_AudioSource);
            //if (m_AudioSource == null)
            //{
            //    DontDestroyOnLoad(gameObject);
            //    m_AudioSource = gameObject.AddComponent<AudioSource>();
            //    m_AudioSource.playOnAwake = false;
            //}
        }
        Debug.Log("Audio Awake End.");
    }

    public void Start()
    {

    }

    /// <summary>
    /// 播放新的clip
    /// </summary>
    /// <param name="clip"></param>
    public async void Play(AudioClip clip)
    {
        if (m_AudioSource != null && clip != null)
        {
            if (m_AudioSource.isPlaying)
                m_AudioSource.Stop();

            await UniTask.Delay(1000);
            m_AudioSource.clip = clip;
            m_AudioSource.Play();
        }
    }

    /// <summary>
    /// 暂停
    /// </summary>
    public void Pause()
    {
        if (m_AudioSource != null)
        {
            m_AudioSource.Pause();
        }
    }

    /// <summary>
    /// 停止
    /// </summary>
    public void Stop()
    {
        if (m_AudioSource != null)
        {
            m_AudioSource.Stop();
        }
    }

    /// <summary>
    /// 重新播放
    /// </summary>
    public void Restart()
    {
        if (m_AudioSource != null)
        {
            m_AudioSource.time = 0;
            m_AudioSource.Play();
        }
    }

    /// <summary>
    /// 音量调节
    /// </summary>
    /// <param name="volume"></param>
    public void SetVolume(float volume)
    {
        m_AudioSource.volume = volume;
    }
}
