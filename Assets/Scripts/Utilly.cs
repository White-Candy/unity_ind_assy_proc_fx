using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace sugar
{
    public static class Utilly
    {
        /// <summary>
        /// 根据 path_url 找到文本内容
        /// </summary>
        /// <param name="path_url"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static async UniTask DownLoadTextFromServer(string path_url, Action<string> callback = null)
        {
            UnityWebRequest req = UnityWebRequest.Get(path_url);
            await req.SendWebRequest();

            string content = req.downloadHandler.text;

            if (callback != null)
            {
                callback(content);
            }
        }

        /// <summary>
        /// 退出模型场景行为
        /// </summary>
        public static void ExitModeSceneAction()
        {
            CameraControl.SetMain();

            InfoPanel._instance.Active(false);
            //InfoPanel._instance.m_token.Dispose();
            InfoPanel._instance.CancelCountDownToken();

            MinMap._instance.Active(false);
            SelectStepPanel._instance.Active(false);
            MenuPanel._instance.Active(true);
            MenuPanel._instance.SetActiveMenuList(true);
            TitlePanel._instance.SetTitle(GlobalData.currModuleName);

            UnityEngine.Object.Destroy(GlobalData.SceneModel.gameObject);
            GlobalData.DestroyModel = true;
            GlobalData.StepIdx = 0;
            GlobalData.currItemMode = "";

            GameMode.Instance.totalScore = 0f;
        }
    }
}
