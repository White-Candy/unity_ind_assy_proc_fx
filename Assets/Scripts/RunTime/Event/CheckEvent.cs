using Cysharp.Threading.Tasks;
using LitJson;
using sugar;

public class CheckEvent : BaseEvent
{
    public override async void OnPrepare(params object[] args) { await UniTask.Yield(); }

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
                NetworkTCPExpand.DownLoadResourcesReq(info.relaPath);
            }
            else
            {
                //StorageExpand.UpdateThisFileInfo(info);
                GlobalData.IsLatestRes = true;
            }
        });
    }
}
