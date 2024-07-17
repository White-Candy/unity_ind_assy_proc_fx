using sugar;
using System;
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
        MessagePanel panel = UIConsole.Instance.FindAssetPanel<MessagePanel>();
        panel.Show(mess, duration);
    }

    /// <summary>
    /// 加載scene場景，之前還要顯示一個加載中的UI。
    /// </summary>
    /// <param name="scene">需要顯示的場景</param>
    /// <param name="real"> 如果real為True異步加載模型場景，否在異步加載UI場景 </param>
    /// <param name="model_name"></param>
    public static void Loading(string scene, string model_name = "")
    {
        LoadingPanel load_panel = UIConsole.Instance.FindAssetPanel<LoadingPanel>();
        load_panel.LoadScene(scene, model_name);
    }

    public static void OpenDialog(string title, string info, Action callback, bool single = false)
    {
        DialogPanel.OpenDialog(title, info, callback, single);
    }

    /// <summary>
    /// 關閉游戲
    /// </summary>
    public static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}

