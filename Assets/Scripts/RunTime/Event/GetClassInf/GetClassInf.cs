
using Cysharp.Threading.Tasks;
using LitJson;
using sugar;

public class GetClassInf : BaseEvent
{
    public override async void OnEvent(params object[] args)
    {
        MessPackage mp = args[0] as MessPackage;
        // Debug.Log("GetProjInfo OnEvent");
        JsonData jsonData = JsonMapper.ToObject(mp.ret);

        await UniTask.Yield();
    }
}