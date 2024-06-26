using sugar;
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
    public void LoadScene(string scene, string model_name)
    {
        //StartCoroutine(UnRealLoad(scene, model_name));
        LoadAsync(scene, model_name);
    }

    private IEnumerator RealLoad(string name)
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
    /// 假进度条加载
    /// PanelLoad的UI Percent因爲加載太快會直接跳過...
    /// ...爲了讓他流暢的顯示，故這樣去處理進度條的percent
    /// </summary>
    /// <param name="name"></param>
    /// <param name="model_name"></param>
    /// <returns></returns>
    private IEnumerator UnRealLoad(string name, string model_name)
    {
        float time = Time.realtimeSinceStartup;
        // Unity场景加载
        AsyncOperation scene_async = SceneManager.LoadSceneAsync(name);
        scene_async.allowSceneActivation = false; // 場景不顯示在前台，現在後臺加載
        float real_percent;
        float percent = 0.0f;
        while (percent < 1.0f)
        {
            real_percent = scene_async.progress;
            if (real_percent >= 0.9f) //真實的加載百分比
            {
                real_percent = 1.0f;
            }

            if (real_percent > percent) // 顯示Loading窗口，比較平緩的滑動進度條。
            {

                percent += Time.deltaTime;

                percent = Mathf.Clamp01(percent);
            }
            m_ProgressSlider.value = percent;
            yield return new WaitForEndOfFrame();
        }
        Debug.Log($"IEnumerator Load Time: {(Time.realtimeSinceStartup - time) * 1000:F6}毫秒");
        scene_async.allowSceneActivation = true; // 後臺加載完畢後，在顯示到前臺去
        OnLoaded();
    }


    private async void LoadAsync(string name, string model_name)
    {
        // Unity场景加载
        AsyncOperation scene_async = SceneManager.LoadSceneAsync(name);
        scene_async.allowSceneActivation = false; // 場景不顯示在前台，現在後臺加載
        float real_percent;
        float percent = 0.0f;
        while (percent < 1.0f)
        {
            real_percent = scene_async.progress;
            if (real_percent >= 0.9f) //真實的加載百分比
            {
                real_percent = 1.0f;
            }

            if (real_percent > percent) // 顯示Loading窗口，比較平緩的滑動進度條。
            {
                percent += Time.deltaTime;
                percent = Mathf.Clamp01(percent);
            }
            m_ProgressSlider.value = percent;
            await UniTask.WaitForEndOfFrame(this);
        }
        scene_async.allowSceneActivation = true; // 後臺加載完畢後，在顯示到前臺去
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
