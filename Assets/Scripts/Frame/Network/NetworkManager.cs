using Cysharp.Threading.Tasks;
using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager _Instance;

    private void Awake()
    {
        _Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
      
    }

    /// <summary>
    /// 下載文檔
    /// </summary>
    /// <param name="url"></param>
    /// <param name="callback"></param>
    public void DownLoadTextFromServer(string url, Action<string> callback)
    {
        StartCoroutine(Utilly.DownLoadTextFromServer(url, callback));
    }

    /// <summary>
    /// 下载Config文档
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public async UniTask DownLoadConfig(Action<List<string>> callback)
    {
        //Debug.Log(FPath.JiaoAnPath);
        UnityWebRequest req = UnityWebRequest.Get(FPath.JiaoAnPath + "/Config.txt");
        await req.SendWebRequest();

        string content = req.downloadHandler.text;
        string[] strs = content.Split('_');

        List<string> paths = new List<string>();
        foreach (string str in strs)
        {
            paths.Add(FPath.JiaoAnPath + "/" + str + ".pdf");
        }
        callback(paths);
    }
}
