using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    private AudioSource m_AudioSource;

    public bool m_IsPlaying { get { return m_AudioSource.isPlaying; } }

    public override void Awake()
    {
        base.Awake();
        // Debug.Log("Audio Awake");
        if (m_AudioSource == null)
        {
            gameObject.TryGetComponent(out m_AudioSource);
            if (m_AudioSource == null)
            {
                DontDestroyOnLoad(gameObject);
                m_AudioSource = gameObject.AddComponent<AudioSource>();
                m_AudioSource.playOnAwake = false;
            }
        }
    }

    public void Start()
    {

    }

    /// <summary>
    /// �����µ�clip
    /// </summary>
    /// <param name="clip"></param>
    public void Play(AudioClip clip)
    {
        if (m_AudioSource != null && clip != null)
        {
            if (m_AudioSource.isPlaying)
                m_AudioSource.Stop();

            m_AudioSource.clip = clip;
            m_AudioSource.Play();
        }
    }

    /// <summary>
    /// ��ͣ
    /// </summary>
    public void Pause()
    {
        if (m_AudioSource != null)
        {
            m_AudioSource.Pause();
        }
    }

    /// <summary>
    /// ֹͣ
    /// </summary>
    public void Stop()
    {
        if (m_AudioSource != null)
        {
            m_AudioSource.Stop();
        }
    }

    /// <summary>
    /// ���²���
    /// </summary>
    public void Restart()
    {
        if (m_AudioSource != null)
        {
            m_AudioSource.time = 0;
            m_AudioSource.Play();
        }
    }
}
