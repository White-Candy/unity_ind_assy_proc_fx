using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class UITools
{
    /// <summary>
    /// 提示窗口加載
    /// </summary>
    /// <param name="mess"></param>
    /// <param name="duration"></param>
    public static void ShowMessage(string mess, float duration = 3.0f)
    {
        MessagePanel panel = FindAssetPanel<MessagePanel>();
        panel.Show(mess, duration);
    }

    /// <summary>
    /// 加載scene場景，之前還要顯示一個加載中的UI。
    /// </summary>
    /// <param name="scene">需要顯示的場景</param>
    /// <param name="real"> 如果real為True異步加載模型場景，否在異步加載UI場景 </param>
    /// <param name="model_name"></param>
    public static void Loading(string scene, bool real = true, string model_name = "")
    {
        LoadingPanel load_panel = FindAssetPanel<LoadingPanel>();
        load_panel.LoadScene(scene, model_name, real);
    }

    public static T FindAssetPanel<T>() where T : BasePanel
    {
        T t = UIConsole.Instance.FindPanel<T>();
        if (t == null)
        {
            string path = "Prefabs/UI/Panel/" + typeof(T).ToString();
            GameObject go = GameObject.Instantiate(Resources.Load<GameObject>(path));
            if (go != null)
            {
                t = go.GetComponent<T>();
                go.name = typeof(T).Name;
                if (t is IGlobalPanel)
                {
                    go.transform.SetParent(GlobalCanvas.Instance.transform);
                }
                else
                {
                    go.transform.SetParent(GameObject.Find("Canvas/").transform);
                }
                go.transform.localScale = Vector3.one;
                RectTransform rect = go.transform as RectTransform;
                rect.offsetMax = Vector2.zero;
                rect.offsetMin = Vector2.zero;
            }
        }
        return t;
    }
}

