using Cysharp.Threading.Tasks;
using LitJson;
using sugar;

public class CheckEvent : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        await UniTask.RunOnThreadPool(() =>
        {
            MessPackage mp = args[0] as MessPackage;
            ResourcesInfo info = JsonMapper.ToObject<ResourcesInfo>(mp.ret);
            if (info.need_updata)
            {
                info.need_updata = false;
                StorageExpand.UpdateThisFileInfo(info);

                string path = info.relaPath;
                DownLoadPanel._instance.m_NeedDL.Add(path);
            }
            GlobalData.Checked = true;
        });
    }
}
