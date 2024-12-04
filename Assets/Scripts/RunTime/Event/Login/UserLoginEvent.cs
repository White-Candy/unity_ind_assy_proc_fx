using Cysharp.Threading.Tasks;
using LitJson;

using UnityEngine;
using UnityEngine.Networking;

public class UserLoginEvent : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        MessPackage mp = args[0] as MessPackage;
        await UniTask.SwitchToMainThread();
        UserInfo info = JsonMapper.ToObject<UserInfo>(mp.ret);
        GlobalData.usrInfo = info;

        if (info.login)
        {
            UITools.Loading("Menu");

            NumOfPeopleInfo nop = new NumOfPeopleInfo()
            { 
                moduleName = GlobalData.courseName,
                count = 1
            };
            string nopReqStr = JsonMapper.ToJson(nop);
            HTTPConsole.NativeHttpSend(URL.numberOfPeople, nopReqStr, (text) => { }, UnityWebRequest.kHttpVerbPOST);
        }

        UITools.ShowMessage(info.hint);
    }
}
