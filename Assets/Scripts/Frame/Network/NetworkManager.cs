using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager _Instance;

    private async void Awake()
    {

    }

    public async void Start()
    {
        _Instance = this;
        DontDestroyOnLoad(this);
        await DownLoadTextFromServer(Application.streamingAssetsPath + "\\IP.txt", (ip) =>
        {
            URL.IP = $"http://{ip}/";

            // 与局服务器连接请求
            //string[] split = ip.Split(":");
            //TCP.Connect(split[0], int.Parse(split[1]));
        });
    }

    public void Update()
    {
        if (HTTPConsole.m_MessQueue.Count > 0)
        {
            MessPackage pkg = HTTPConsole.m_MessQueue.Dequeue();
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
        //TCP.Close();
    }
}
