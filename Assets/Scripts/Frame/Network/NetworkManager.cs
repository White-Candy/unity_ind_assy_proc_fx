using Cysharp.Threading.Tasks;
using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

        // ��ַ�������������
        NetworkClientTCP.Connect("192.168.3.34", 5800);
    }

    public void Update()
    {
        // if (NetworkClientTCP.m_FrontQueue.Count > 0)
        // {
        //     FrontMp fp = NetworkClientTCP.m_FrontQueue.Dequeue();
        //     BaseEvent @event = Tools.CreateObject<BaseEvent>(fp.event_type);
        //     @event.OnPrepare(fp);
        // }

        if (NetworkClientTCP.m_MessQueue.Count > 0)
        {
            MessPackage pkg = NetworkClientTCP.m_MessQueue.Dequeue();
            BaseEvent @event = Tools.CreateObject<BaseEvent>(pkg.event_type);
            @event.OnEvent(pkg);
        }
    }

    /// <summary>
    /// ���d�ęn
    /// </summary>
    /// <param name="url"></param>
    /// <param name="callback"></param>
    public async UniTask DownLoadTextFromServer(string url, Action<string> callback)
    {
        await Utilly.DownLoadTextFromServer(url, callback);
    }

    /// <summary>
    /// ����Config�ĵ�
    /// </summary>
    /// <param name="callback"></param>
    /// <returns></returns>
    public async UniTask DownLoadConfigAsync(string name, Action<List<string>> callback)
    {
        //Debug.Log(FPath.JiaoAnPath);
        string suffix = Tools.GetModulePath(name);
        string path = FPath.AssetRootPath + GlobalData.currModuleCode + suffix;

        UnityWebRequest req = UnityWebRequest.Get(path + "/Config.txt");
        await req.SendWebRequest();

        string content = req.downloadHandler.text;
        string[] strs = content.Split('_');

        List<string> paths = new List<string>();
        foreach (string str in strs)
        {
            paths.Add(path + "/" + str + ".pdf");
        }
        callback(paths);
    }

    /// <summary>
    /// ������Դ�ļ�ConfigĿ¼����
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public async UniTask<List<string>> DownLoadAasetAsync(string name)
    {
        string suffix = Tools.GetModulePath(name);
        string path = FPath.AssetRootPath + GlobalData.currModuleCode + suffix;

        UnityWebRequest req = UnityWebRequest.Get(path + "/Config.txt");
        await req.SendWebRequest();

        string content = req.downloadHandler.text;
        string[] strs = content.Split('_');

        List<string> paths = new List<string>();
        foreach (string str in strs)
        {
            paths.Add(path + "/" + str);
        }
        return paths;
    }

    /// <summary>
    /// ����
    /// </summary>
    public void OnDestroy()
    {
        StorageExpand.SaveToDisk();
    }
}
