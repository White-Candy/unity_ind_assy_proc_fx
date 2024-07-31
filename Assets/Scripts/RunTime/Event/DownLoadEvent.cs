using Cysharp.Threading.Tasks;
using LitJson;
using sugar;
using UnityEngine;

public class DownLoadEvent : BaseEvent
{
    public float old_Percent = 0.0f;
    public async override void OnPrepare(params object[] args)
    {
        // await UniTask.SwitchToMainThread();

        float displayPercent = 0.0f;
        while(true)
        {
            await UniTask.WaitUntil(() => old_Percent != NetworkClientTCP.percent);

            // TODO..优化
            old_Percent = NetworkClientTCP.percent;
            while (displayPercent <= old_Percent)
            {
                displayPercent += 0.5f;
                DownLoadPanel._instance.SetDLPercent(displayPercent);
                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            if (NetworkClientTCP.percent == 100.0f)
            {
                old_Percent = 0.0f;
                break;
            }
        }
        Debug.Log("break!");
    }

    public override async void OnEvent(params object[] args)
    {
        await UniTask.RunOnThreadPool(() =>
        {
            var mp = args[0] as MessPackage;
            FilePackage fp = JsonMapper.ToObject<FilePackage>(mp.ret);
            //string savePath = Application.streamingAssetsPath + "\\Data\\" + fp.relativePath;

            DownLoadPanel._instance.m_NeedWt.Add(fp); //将二进制文件数据加载到内存中去
            GlobalData.Downloaded = true;
            //Tools.Bytes2File(fp.fileData, savePath);
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
