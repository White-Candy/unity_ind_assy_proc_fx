using Cysharp.Threading.Tasks;
using LitJson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownLoadEvent : BaseEvent
{
    public override async void OnPrepare(params object[] args)
    {
        await UniTask.SwitchToMainThread();
        DownLoadPanel._instance.Active(true);

        do
        {
            DownLoadPanel._instance.SetPercent(NetworkClientTCP.percent);
        } 
        while (NetworkClientTCP.percent < 100.0f);
    }

    public override async void OnEvent(params object[] args)
    {
        await UniTask.RunOnThreadPool(() =>
        {
            var mp = args[0] as MessPackage;
            FilePackage fp = JsonMapper.ToObject<FilePackage>(mp.ret);
            string savePath = Application.streamingAssetsPath + "\\Data\\" + fp.relativePath;
            Tools.Bytes2File(fp.fileData, savePath);
        });
    }
}

/// <summary>
/// ÎÄ¼þ°ü
/// </summary>
public class FilePackage
{
    public string fileName;
    public string relativePath;
    public byte[] fileData;
}
