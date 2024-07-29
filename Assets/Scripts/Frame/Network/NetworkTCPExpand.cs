


using LitJson;

public static class NetworkTCPExpand
{
    /// <summary>
    /// 文件下载请求
    /// </summary>
    /// <param name="relative"></param>
    public static void DownLoadResourcesReq(string relative)
    {
        JsonData js = new JsonData();
        js["relaPath"] = relative;
        NetworkClientTCP.SendAsync(JsonMapper.ToJson(js), EventType.DownLoadEvent);
    }

    /// <summary>
    /// 请求检查文件更新
    /// </summary>
    /// <param name="code"></param>
    /// <param name="moduelName"></param>
    public static void CheckResourceReq(string relative)
    {
        var Rsinfo = StorageExpand.FindRsInfo(relative);
        string s_info = JsonMapper.ToJson(Rsinfo);
        NetworkClientTCP.SendAsync(s_info, EventType.CheckEvent);
    }
}