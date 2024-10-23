
using UnityEngine;
using Cysharp.Threading.Tasks;
using LitJson;
using Unity.Mathematics;
using System.Linq;
using System.Text;

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

        await UniTask.WaitUntil(() => 
        {
            result = JsonMapper.ToJson(obj);
            return result.Count() != 0;
        });
        return result;
    }

    /// <summary>
    /// 判断是否为规范的json
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static bool isJson(string json)
    {
        return json[0] == '{' && json[json.Count() - 1] == '}';
    }
}