using Cysharp.Threading.Tasks;
using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UITools
{
    /// <summary>
    /// ��ʾ���ڼ��d
    /// </summary>
    /// <param name="mess"></param>
    /// <param name="duration"></param>
    public static void ShowMessage(string mess, float duration = 3.0f)
    {
        MessagePanel panel = UIConsole.Instance.FindAssetPanel<MessagePanel>();
        panel.Show(mess, duration);
    }

    /// <summary>
    /// ���dscene������֮ǰ߀Ҫ�@ʾһ�����d�е�UI��
    /// </summary>
    /// <param name="scene"> ��Ҫ�@ʾ�Ĉ��� </param>
    /// <param name="real"> ���real��True�������dģ�͈��������ڮ������dUI���� </param>
    /// <param name="model_name"></param>
    public static void Loading(string scene, string model_name = "")
    {
        LoadingPanel load_panel = UIConsole.Instance.FindAssetPanel<LoadingPanel>();
        load_panel.LoadScene(scene, model_name);
    }

    /// <summary>
    /// ���ش���
    /// </summary>
    /// <param name="f"></param>
    public static DownLoadPanel SpawnDownLoad()
    {
        return UIConsole.Instance.FindAssetPanel<DownLoadPanel>();
    }

    public static void OpenDialog(string title, string info, Action callback, bool single = false)
    {
        DialogPanel.OpenDialog(title, info, callback, single);
    }

    /// <summary>
    /// ���UI���������Ƿ�Ϊ��
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
    /// �P�]�Α�
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
            if (TCP.percent == old_Percent) return false; 
            old_Percent = TCP.percent;
            DownLoadPanel._instance.SetDLPercent(TCP.percent);
            // Debug.Log("========================= DownLoadPrepare: " +  old_Percent + " | " + fileCount);
            if (old_Percent == 100.0f) 
            {
                old_Percent = 0.0f;
                fileCount--;
            }
            return fileCount == 0; 
        });
        // while(fileCount > 0)
        // {
        //     if (TCP.percent == old_Percent) continue; 
        //     Debug.Log("========================= DownLoadPrepare: " +  TCP.percent + " | " + fileCount);
        //     DownLoadPanel._instance.SetDLPercent(TCP.percent);
        //     old_Percent = TCP.percent;
        //     if (TCP.percent == 100.0f) 
        //     {
        //         old_Percent = 0.0f;
        //         fileCount--;
        //     }
        // }
                        
        await UniTask.WaitUntil(() => GlobalData.Downloaded == true);
        GlobalData.Downloaded = false;
        //Debug.Log("break! " + fileCount);
    }
}

