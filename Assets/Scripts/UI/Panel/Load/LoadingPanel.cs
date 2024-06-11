using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 加載窗口UI脚本
/// </summary>
public class LoadingPanel : BasePanel, IGlobalPanel
{
    public Slider m_ProgressSlider; // 進度條
    public TextMeshProUGUI m_ProgressPercent; // 顯示進度條百分比

    public override void Awake()
    {
        base.Awake();

        m_ProgressSlider.onValueChanged.AddListener( (value) => 
        {
            float progress = (float)value;
            m_ProgressPercent.text = progress.ToString("0.00%");
        });
    }

    // 場景加載
    public void LoadScene(string scene, string model_name, bool real)
    {
        if (real)
        {
            StartCoroutine(ModelLoad(scene));
        }
        else
        {
            StartCoroutine(PanelLoad(scene, model_name));
        }
    }

    private IEnumerator ModelLoad(string name)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(name);
        async.allowSceneActivation = false; // 場景不顯示在前台，現在後臺加載
        while (!async.isDone)
        {
            m_ProgressSlider.value = async.progress;
            Debug.Log(m_ProgressSlider.value);
            if (async.progress >= 0.9f) //異步加載中 progress 為0.9f，就已經加載好了
            {
                m_ProgressSlider.value = 1.0f;
                yield return null;
                async.allowSceneActivation = true;
                OnLoaded();
                yield break;
            }
        }
        async.allowSceneActivation = true; // 後臺加載完畢後，在顯示到前臺去
        OnLoaded();
    }

    /// <summary>
    /// PanelLoad的UI Percent因爲加載太快會直接跳過...
    /// ...爲了讓他流暢的顯示，故這樣去處理進度條的percent
    /// </summary>
    /// <param name="name"></param>
    /// <param name="model_name"></param>
    /// <returns></returns>
    private IEnumerator PanelLoad(string name, string model_name)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(name);
        async.allowSceneActivation = false; // 場景不顯示在前台，現在後臺加載
        float real_percent;
        float percent = 0.0f;
        while (percent < 1.0f)
        {
            real_percent = async.progress;
            if (real_percent >= 0.9f) //真實的加載百分比
            {
                real_percent = 1.0f;
            }

            if (real_percent > percent) // 顯示Loading窗口，比較平緩的滑動進度條。
            {
                if (real_percent <= 0.9f) 
                {
                    percent += Time.deltaTime;
                }
                else
                {
                    percent += Time.deltaTime * 0.2f;
                }
                percent = Mathf.Clamp01(percent);
            }
            m_ProgressSlider.value = percent;
            yield return new WaitForEndOfFrame();
        }

        async.allowSceneActivation = true; // 後臺加載完畢後，在顯示到前臺去
        OnLoaded();
    }

    /// <summary>
    /// 加載完成後，銷毀加載窗口
    /// </summary>
    private void OnLoaded()
    {
        //DestroyImmediate(gameObject);
        Destroy(gameObject);
    }
}
