using Cysharp.Threading.Tasks;
using DG.Tweening.Plugins;
using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager _Instance;

    private async void Awake()
    {
        _Instance = this;
        DontDestroyOnLoad(this);

        await NetworkManager._Instance.DownLoadTextFromServer(Application.streamingAssetsPath + "\\IP.txt", (ip) => 
        {
            // 与局服务器连接请求
            TCP.Connect(ip, 5800);
        });
    }

    public void Update()
    {
        if (TCP.m_MessQueue.Count > 0)
        {
            MessPackage pkg = TCP.m_MessQueue.Dequeue();
            BaseEvent @event = Tools.CreateObject<BaseEvent>(pkg.event_type);
            @event.OnEvent(pkg);
        }
    }

    /// <summary>
    /// 下載文檔
    /// </summary>
    /// <param name="url"></param>
    /// <param name="callback"></param>
    public async UniTask DownLoadTextFromServer(string url, Action<string> callback)
    {
        await Utilly.DownLoadTextFromServer(url, callback);
    }

    /// <summary>
    /// 下载Config文档
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public async UniTask DownLoadConfigAsync(string name, Action<List<string>> callback)
    {
        //Debug.Log(FPath.JiaoAnPath);
        string suffix = Tools.GetModulePath(name);
        string path = FPath.AssetRootPath + GlobalData.ProjGroupName + suffix;

        UnityWebRequest req = UnityWebRequest.Get(path + "\\Config.txt");
        await req.SendWebRequest();

        string content = req.downloadHandler.text;
        string[] strs = content.Split('_');

        List<string> paths = new List<string>();
        foreach (string str in strs)
        {
            paths.Add(path + "\\" + str + ".pdf");
        }
        callback(paths);
    }

    /// <summary>
    /// 获取文件夹中所有满足后缀的文件
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public List<string> DownLoadAaset(string name, string extentsion)
    {
        GlobalData.CurrActionPathList = new List<string>();
        string suffix = Tools.GetModulePath(name);
        string path = FPath.AssetRootPath + GlobalData.ProjGroupName + suffix;

        FileInfo[] filesInfo = FileHelper.GetDirectoryFileInfo(path, extentsion);
        List<string> filesPath = new List<string>();
        foreach (var info in filesInfo)
        {
            GlobalData.CurrActionPathList.Add(GlobalData.ProjGroupName + suffix + "\\" + info.Name);
            filesPath.Add(info.FullName);
        }
        return filesPath;
    }

    /// <summary>
    /// 销毁
    /// </summary>
    public void OnDestroy()
    {
        StorageExpand.SaveToDisk();
        TCP.Close();
    }
}
