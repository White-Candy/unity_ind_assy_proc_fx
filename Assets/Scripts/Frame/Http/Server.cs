
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;
using Cysharp.Threading.Tasks;
using System.Threading.Tasks;

public class Server : MonoBehaviour
{

    /// <summary>
    /// Http Post Type is Json.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="body"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public async UniTask Post(string url, string body, Action<string> callback)
    {
        //Debug.Log("Body: " + body);
        byte[] bytes = null;
        if (body != null)
        {
            string str = System.Text.RegularExpressions.Regex.Unescape(body);
            bytes = Encoding.UTF8.GetBytes(str);
        }
        using(UnityWebRequest req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST)) 
        {
            if (!string.IsNullOrEmpty(GlobalData.token))
            {
                req.SetRequestHeader("Authorization", $"Bearer {GlobalData.token}");
            }

            req.downloadHandler = new DownloadHandlerBuffer();
            req.uploadHandler = new UploadHandlerRaw(bytes);
            req.SetRequestHeader("Content-Type", "application/json;charset=utf8");

            await req.SendWebRequest();
            if (req.error == null) 
            {
                // Debug.Log("服务器链接成功");
                while(!req.isDone)
                {
                    await UniTask.Yield();
                }

                JsonData data = JsonMapper.ToObject(req.downloadHandler.text);
                //Debug.Log("Return Body: " + req.downloadHandler.text);

                if (Tools.CheckMessageSuccess(int.Parse(data["code"].ToString())))
                {
                    if(callback != null && req.error == null)
                    {
                        callback(req.downloadHandler.text);
                    }
                }
                else
                {
                    try
                    {
                        UITools.ShowMessage(data["msg"].ToString());
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                    }
                }
            }
            else
            {
                // Debug.Log("无法与服务器建立连接");
                UITools.ShowMessage("无法与服务器建立连接，请联系后台管理员");
            }
        }
    }

    public async UniTask Get_SetHeader(string url, Action<string> callback)
    {
        using (UnityWebRequest req = UnityWebRequest.Get(url))
        {
            //Debug.Log($"AuthorizationBearer {GlobalData.token}");
            req.SetRequestHeader("Authorization", $"Bearer {GlobalData.token}");
            await req.SendWebRequest();

            if (req.error != null)
            {
                Debug.LogError(req.error + " = " + url);
            }

            while (!req.isDone)
            {
                await UniTask.Yield();
            }

            if (callback != null)
            {
                callback(req.downloadHandler.text);
            }
        }
    }
}
