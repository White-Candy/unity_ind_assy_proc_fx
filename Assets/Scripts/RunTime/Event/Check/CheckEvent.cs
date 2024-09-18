using System.IO;
using Cysharp.Threading.Tasks;
using LitJson;
using sugar;
using UnityEngine;
public class CheckEvent : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        MessPackage mp = args[0] as MessPackage;
        UpdatePackage up = JsonMapper.ToObject<UpdatePackage>(mp.ret);

        foreach (string path in GlobalData.CurrActionPathList)
        {
            int eraseIdx = -1;
            eraseIdx = up.filesInfo.FindIndex(x => x.relaPath == path);
            if (eraseIdx == -1)
            {
                string erasePath = Application.streamingAssetsPath + "\\Data\\" + path;
                //Debug.Log($"Delete path: {erasePath}");
                File.Delete(erasePath);

                int deleteIdx = -1;
                deleteIdx = StorageExpand.Storage.rsCheck.FindIndex(x => x.relaPath == path);
                if (deleteIdx != -1) StorageExpand.Storage.rsCheck.RemoveAt(deleteIdx);
            }
        }

        foreach (var info in up.filesInfo)
        {
            if (info.need_updata)
            { 
                info.need_updata = false;
                StorageExpand.UpdateThisFileInfo(info);

                string path = info.relaPath;
                DownLoadPanel._instance.m_NeedDL.Add(path);
            }
        }
        
        GlobalData.Checked = true;
        await UniTask.Yield();
    }
}
