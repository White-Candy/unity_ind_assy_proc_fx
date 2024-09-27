
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using Unity.VisualScripting;

public class HTTPConsole
{
    // 内容包队列
    public static Queue<MessPackage> m_MessQueue = new Queue<MessPackage>();

    private static float percent;

    private static MessPackage mp = new MessPackage();

    public static void HttpFieldProcessing(string mess)
    {
        string[] messages = mess.Split("@");
        foreach (var message in messages)
        {
            InforProcessing(message, mp);
        }           
    }

    public static async void SendAsyncPost(string mess, EventType event_type, OperateType operateType)
    {
        string front = FrontPackage(mess, event_type, operateType);
        string totalInfoPkg = $"|{front}#{mess}@";
        byte[] byt = System.Text.Encoding.UTF8.GetBytes(totalInfoPkg);
        //utf-8 byte数组转string
        totalInfoPkg = System.Text.Encoding.UTF8.GetString(byt);

        long totalLength = totalInfoPkg.Count();
        string finalPkg = totalLength.ToString() + totalInfoPkg;

        Debug.Log($"{totalLength} | {finalPkg}");
        await Client.m_Server.Post(URL.IP, finalPkg, (text) => 
        {
            Debug.Log("Post Return: " + text);
#if UNITY_EDITOR
            FileHelper.WriteFileByLine(Application.streamingAssetsPath, "HTTPLog.txt", text);
#endif
            HttpFieldProcessing(text);
        });       
    }

    /// <summary>
    /// 前置包
    /// </summary>
    /// <param name="mess"></param>
    /// <param name="event_type"></param>
    static string FrontPackage(string mess, EventType event_type, OperateType operateType)
    {
        FrontMp mpinfo = new FrontMp()
        {
            ip = Tools.GetIPForTypeIPV4(),
            length = mess.Count().ToString(),
            event_type = event_type.ToSafeString(),
            operate_type = operateType.ToSafeString()
        };

        string s_info = JsonMapper.ToJson(mpinfo);
        return s_info;
    }
    
    static void InforProcessing(string mess, MessPackage mp)
    {
        if (string.IsNullOrEmpty(mess) || mess.Count() == 0) 
        {
            // Debug.Log("its null string..");
            return;
        }

        string[] lengthSplit = mess.Split("|");
        string totalLength = lengthSplit[0];
        if (!mp.get_length && !string.IsNullOrEmpty(totalLength))
        {
            mp.length = int.Parse(totalLength);
            mp.get_length = true;
            mp.ret += lengthSplit[1];
        }
        else
        {
            if (mp.length > mp.ret.Count())
            {
                mp.ret += mess;
            }
        }
        check(mp);                  
    }

    /// <summary>
    /// 进度检查
    /// </summary>
    /// <param name="pkg"></param>
    static void check(MessPackage mp)
    {
        int finalLength = mp.ret.Count() + 2;
        percent = finalLength * 1.0f / mp.length * 100.0f;
        if (GlobalData.currEventType == EventType.DownLoadEvent) GlobalData.DownloadParcent = percent;
        // Debug.Log($" ============ {mp.ret.Count() + 2.0f} || {mp.length} || {percent}");
        // Debug.Log($" ============ {finalLength} || {mp.length} || {percent}"); 
        if (percent == 100.0f)
        {
            mp.finish = true;
            ParsingThePackageBody(mp.ret, mp);
        }
    }

    /// <summary>
    /// 前置包和内容包解析
    /// </summary>
    /// <param name="pkg"></param>
    static void ParsingThePackageBody(string package, MessPackage mp)
    {
        string[] Split = package.Split("#");
        string front = Split[0];
        string main = Split[1];

        JsonData data = JsonMapper.ToObject(front);
        mp.ip = data["ip"].ToString();
        mp.length = int.Parse(data["length"].ToString());
        mp.event_type = data["event_type"].ToString();
        mp.operate_type = data["operate_type"].ToString();
        mp.get_length = true;
        mp.ret = main;
        MessQueueAdd(mp);
        mp.Clear();
        percent = 0.0f;
    }

    /// <summary>
    /// 为消息队列 Clone pkg 并且存放
    /// </summary>
    /// <param name="pkg"></param>
    static void MessQueueAdd(MessPackage mp)
    {
        MessPackage pkg = new MessPackage(mp);
        m_MessQueue.Enqueue(pkg);
    }
}