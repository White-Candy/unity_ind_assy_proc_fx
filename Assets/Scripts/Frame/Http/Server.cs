using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using LitJson;

public class Server : MonoBehaviour
{

    /// <summary>
    /// Http Post Type is Json.
    /// </summary>
    /// <param name="url"></param>
    /// <param name="body"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    public IEnumerator Post(string url, string body, Action<string> callback)
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

            yield return req.SendWebRequest();
            if (req.error == null) 
            { 
                while(!req.isDone)
                {
                    yield return new WaitForEndOfFrame();
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
        }
    }
}
