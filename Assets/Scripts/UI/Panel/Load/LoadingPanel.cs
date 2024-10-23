
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

/// <summary>
/// ���d����UI�ű�
/// </summary>
public class LoadingPanel : BasePanel, IGlobalPanel
{
    public Slider m_ProgressSlider; // �M�ȗl
    public TextMeshProUGUI m_ProgressPercent; // �@ʾ�M�ȗl�ٷֱ�
    public float m_UiPercent = 0.0f;

    public override void Awake()
    {
        base.Awake();

        m_ProgressSlider.onValueChanged.AddListener( (value) => 
        {
            float progress = (float)value;
            m_ProgressPercent.text = progress.ToString("0.00%");
        });
    }

    // �������d
    public void LoadScene(string scene)
    {
        //StartCoroutine(UnRealLoad(scene, model_name));
        LoadAsync(scene);
    }

    public async void SetPercentUI(float percent)
    {
        while (m_UiPercent < percent)
        {
            m_UiPercent += 0.05f;
            m_ProgressSlider.value = m_UiPercent;
            await UniTask.WaitForSeconds(0.05f);
        }

        // OnLoaded();
    }

    private IEnumerator RealLoad(string name)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(name);
        async.allowSceneActivation = false; // �������@ʾ��ǰ̨���F�����_���d
        while (!async.isDone)
        {
            m_ProgressSlider.value = async.progress;
            Debug.Log(m_ProgressSlider.value);
            if (async.progress >= 0.9f) //�������d�� progress ��0.9f�����ѽ����d����
            {
                m_ProgressSlider.value = 1.0f;
                yield return null;
                async.allowSceneActivation = true;
                OnLoaded();
                yield break;
            }
        }
        async.allowSceneActivation = true; // ���_���d�ꮅ�ᣬ���@ʾ��ǰ�_ȥ
        OnLoaded();
    }

    /// <summary>
    /// �ٽ���������
    /// PanelLoad��UI Percent�򠑼��d̫���ֱ�����^...
    /// ...����׌���������@ʾ�����@��ȥ̎���M�ȗl��percent
    /// </summary>
    /// <param name="name"></param>
    /// <param name="model_name"></param>
    /// <returns></returns>
    private IEnumerator UnRealLoad(string name, string model_name)
    {
        float time = Time.realtimeSinceStartup;
        // Unity��������
        AsyncOperation scene_async = SceneManager.LoadSceneAsync(name);
        scene_async.allowSceneActivation = false; // �������@ʾ��ǰ̨���F�����_���d
        float real_percent;
        float percent = 0.0f;
        while (percent < 1.0f)
        {
            real_percent = scene_async.progress;
            if (real_percent >= 0.9f) //�挍�ļ��d�ٷֱ�
            {
                real_percent = 1.0f;
            }

            if (real_percent > percent) // �@ʾLoading���ڣ����^ƽ���Ļ����M�ȗl��
            {
                percent += Time.deltaTime;
                percent = Mathf.Clamp01(percent);
            }
            m_ProgressSlider.value = percent;
            yield return new WaitForEndOfFrame();
        }
        Debug.Log($"IEnumerator Load Time: {(Time.realtimeSinceStartup - time) * 1000:F6}����");
        scene_async.allowSceneActivation = true; // ���_���d�ꮅ�ᣬ���@ʾ��ǰ�_ȥ
        OnLoaded();
    }

    private async void LoadAsync(string name)
    {
        // Unity��������
        AsyncOperation scene_async = SceneManager.LoadSceneAsync(name);
        scene_async.allowSceneActivation = false; // �������@ʾ��ǰ̨���F�����_���d
        float real_percent;
        float percent = 0.0f;
        while (percent < 1.0f)
        {
            real_percent = scene_async.progress;
            if (real_percent >= 0.9f) //�挍�ļ��d�ٷֱ�
            {
                real_percent = 1.0f;
            }

            if (real_percent > percent) // �@ʾLoading���ڣ����^ƽ���Ļ����M�ȗl��
            {
                percent += Time.deltaTime;
                percent = Mathf.Clamp01(percent);
            }
            m_ProgressSlider.value = percent;
            await UniTask.WaitForEndOfFrame(this);
        }
        scene_async.allowSceneActivation = true; // ���_���d�ꮅ�ᣬ���@ʾ��ǰ�_ȥ
        OnLoaded();
    }

    public void Close()
    {
        m_UiPercent = 0.0f;
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// ���d����ᣬ�N�����d����
    /// </summary>
    private void OnLoaded()
    {
        //DestroyImmediate(gameObject);
        Destroy(gameObject);
    }
}
