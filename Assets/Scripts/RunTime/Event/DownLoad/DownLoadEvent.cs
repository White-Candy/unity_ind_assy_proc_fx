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
            FilePackage fp = JsonMapper.ToObject<FilePackage>(mp.ret);
            //string savePath = Application.streamingAssetsPath + "\\Data\\" + fp.relativePath;

            DownLoadPanel._instance.m_NeedWt.Add(fp); //���������ļ����ݼ��ص��ڴ���ȥ

            if (DownLoadPanel._instance.m_NeedDL.Count == DownLoadPanel._instance.m_NeedWt.Count) 
                GlobalData.Downloaded = true;
        });
    }
}

/// <summary>
/// �ļ���
/// </summary>
public class FilePackage
{
    public string fileName;
    public string relativePath;
    public byte[] fileData;
}
