using Cysharp.Threading.Tasks;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UITools
{
    /// <summary>
    /// 提示窗口加載
    /// </summary>
    /// <param name="mess"></param>
    /// <param name="duration"></param>
    public static void ShowMessage(string mess, float duration = 3.0f)
    {
        MessagePanel panel = UIConsole.FindAssetPanel<MessagePanel>();
        panel.Show(mess, duration);
    }

    /// <summary>
    /// 加載scene場景，之前還要顯示一個加載中的UI。
    /// </summary>
    /// <param name="scene"> 需要顯示的場景 </param>
    /// <param name="real"> 如果real為True異步加載模型場景，否在異步加載UI場景 </param>
    /// <param name="model_name"></param>
    public static void Loading(string scene)
    {
        LoadingPanel load_panel = UIConsole.FindAssetPanel<LoadingPanel>();
        load_panel.LoadScene(scene);
    }

    /// <summary>
    /// 下载窗口
    /// </summary>
    /// <param name="f"></param>
    public static DownLoadPanel SpawnDownLoad()
    {
        return UIConsole.FindAssetPanel<DownLoadPanel>();
    }

    public static void OpenDialog(string title, string info, params ButtonData[] buttonsData)
    {
        DialogPanel.OpenDialog(title, info, buttonsData);
    }

    /// <summary>
    /// 检测UI层的输入框是否为空
    /// </summary>
    /// <param name="input"></param>
    /// <param name="hint"></param>
    /// <returns></returns>
    public static bool InputFieldCheck(string content, string hint)
    {
        if (string.IsNullOrEmpty(content))
        {
            ShowMessage(hint);
            return true;
        }
        return false;
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

    public static float old_Percent = 0.0f;
    public async static UniTask DownLoadPrepare(int fileCount)
    {
        await UniTask.WaitUntil(() => 
        {
            // Debug.Log($"TCP.percent: {GlobalData.DownloadParcent}");
            if (GlobalData.DownloadParcent == old_Percent) return false; 
            old_Percent = GlobalData.DownloadParcent;
            DownLoadPanel._instance.SetDLPercent(GlobalData.DownloadParcent);
            // Debug.Log("========================= DownLoadPrepare: " +  old_Percent + " | " + fileCount);
            if (old_Percent == 100.0f) 
            {
                old_Percent = 0.0f;
                fileCount--;
            }
            return fileCount == 0; 
        });
                        
        await UniTask.WaitUntil(() => GlobalData.Downloaded == true);
        GlobalData.Downloaded = false;
        //Debug.Log("break! " + fileCount);
    }

    /// <summary>
    /// 修改ui控件img
    /// </summary>
    /// <param name="img"></param>
    /// <param name="imagePath"></param>
    public static void SetImage(ref Image img, string imagePath)
    {
        img.sprite = Resources.Load<Sprite>(imagePath);
    }
}

