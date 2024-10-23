
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Cysharp.Threading.Tasks;

public class Server : MonoBehaviour
{
    /// <summary>
    /// Http Post Type is Json.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="body"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public async UniTask Post(string url, string body)
    {
        // Debug.Log("Body: " + body);
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
                Debug.Log(req.downloadHandler.text);
                req.Dispose();
            }
            else
            {
                UITools.ShowMessage("无法与服务器建立连接，请联系后台管理员");
            }
        }
    }

    /// <summary>
    /// Http Post Type is Json.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="body"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public async UniTask Post(string url, string body, Action<string> callback)
    {
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
                while(!req.isDone)
                {
                    await UniTask.Yield();
                }
                Debug.Log(req.downloadHandler.text);
                if (!string.IsNullOrEmpty(req.downloadHandler.text)) callback(req.downloadHandler.text);
                req.Dispose();
            }
            else
            {
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