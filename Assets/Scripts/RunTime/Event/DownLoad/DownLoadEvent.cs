using System;
using System.IO;
using Cysharp.Threading.Tasks;
using LitJson;
using sugar;
using UnityEngine;

public class DownLoadEvent : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        await UniTask.RunOnThreadPool(() =>
        {
            var mp = args[0] as MessPackage;
            // Debug.Log("================================= DownLoadEvent: " + mp.ret);
            
            // using (StreamWriter sw = new StreamWriter("LogErrorTestFile.txt"))
            // {
            //     // Add some text to the file.
            //     sw.Write("This is the ");
            //     sw.WriteLine("header for the file.");
            //     sw.WriteLine("-------------------");
            //     sw.WriteLine(mp.ret);
            //     sw.Write("The date is: ");
            //     sw.WriteLine(DateTime.Now);
            // }
            FilePackage fp = JsonMapper.ToObject<FilePackage>(mp.ret);
            
            // string savePath = Application.streamingAssetsPath + "\\Data\\" + fp.relativePath;

            DownLoadPanel._instance.m_NeedWt.Add(fp); //将二进制文件数据加载到内存中去

            if (DownLoadPanel._instance.m_NeedDL.Count == DownLoadPanel._instance.m_NeedWt.Count) 
                GlobalData.Downloaded = true;
        });
    }
}

/// <summary>
/// 文件包
/// </summary>
public class FilePackage
{
    public string fileName;
    public string relativePath;
    public byte[] fileData;
}
