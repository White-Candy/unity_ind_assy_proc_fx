
using Cysharp.Threading.Tasks;
using LitJson;

public class JsonHelper
{
    /// <summary>
    /// 异步 object => json string
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public async static UniTask<string> AsyncToJson(object obj)
    {
        string result = "";
        await UniTask.RunOnThreadPool(() => 
        {
            result = JsonMapper.ToJson(obj);
        });
        return result;
    }
}