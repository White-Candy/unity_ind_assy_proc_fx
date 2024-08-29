using Cysharp.Threading.Tasks;
using LitJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class TCP
{
    public static Socket m_Socket;

    private static IPEndPoint m_Ipend;

    private static int buf_length = 1024000;
    private static byte[] buffer = new byte[buf_length];

    // 内容包队列
    public static Queue<MessPackage> m_MessQueue = new Queue<MessPackage>();

    public static float percent;

    public static void Connect(string ip, int port)
    {
        m_Ipend = new IPEndPoint(IPAddress.Parse(ip), port);
        m_Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_Socket.BeginConnect(m_Ipend, ConnectCallbackAsync, null);
    }

    private static void ConnectCallbackAsync(IAsyncResult ar)
    {
        // Socket socket = (Socket)ar.AsyncState;
        if (m_Socket != null) 
        {
            // Debug.Log($"Socket Connect: {m_Socket.Connected}");
            m_Socket.EndConnect(ar);

            MessPackage mp = new MessPackage();
            m_Socket.BeginReceive(buffer, 0, buf_length, 0, ReviceAsyncCallback, mp);
        }
    }

    public static void ReviceAsyncCallback(IAsyncResult ar)
    {
        MessPackage mp = (MessPackage)ar.AsyncState;
        int length = m_Socket.EndReceive(ar);
        try
        {
            string mess = Encoding.Default.GetString(buffer, 0, length);
            Array.Clear(buffer, 0, buffer.Length);
            Debug.Log("ReviceAsyncCallback: " + mess);
        
            string[] lengthSplit = mess.Split("|");
            string totalLength = lengthSplit[0];
            if (!mp.get_length && !string.IsNullOrEmpty(totalLength))
            {
                Debug.Log("GET LENGTH: " + totalLength);
                mp.length = int.Parse(totalLength);
                mp.get_length = true;
                mp.ret += lengthSplit[1];
                totalLength = "";         
            }
            else
            {
                Debug.Log("GET MESSAGE: ");
                if (mp.length >= mp.ret.Count())
                {
                    mp.ret += mess;
                }
            }
            check(mp);
            m_Socket.BeginReceive(buffer, 0, buf_length, 0, ReviceAsyncCallback, mp);
        }
        catch
        {

        }
    }

    /// <summary>
    /// 异步发送信息
    /// </summary>
    /// <param name="mess">内容</param>
    /// <param name="event_type">事件类型</param>
    public static async void SendAsync(string mess, EventType event_type, OperateType operateType)
    {
        await UniTask.RunOnThreadPool(() => 
        {
            string front = FrontPackage(mess, event_type, operateType);
            string totalInfoPkg = $"|{front}#{mess}@";
            long totalLength = totalInfoPkg.Count();
            string finalPkg = totalLength.ToString() + totalInfoPkg;
            // Debug.Log(finalPkg);

            var outputBuffer = Encoding.Default.GetBytes(finalPkg);
            m_Socket.BeginSend(outputBuffer, 0, outputBuffer.Length, SocketFlags.None, SendAsyncCbk, null);
        });
    }

    /// <summary>
    /// 前置包
    /// </summary>
    /// <param name="mess"></param>
    /// <param name="event_type"></param>
    public static string FrontPackage(string mess, EventType event_type, OperateType operateType)
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

    /// <summary>
    /// 异步发送回调
    /// </summary>
    /// <param name="ar"></param>
    private static void SendAsyncCbk(IAsyncResult ar)
    {
        try
        {
            if (m_Socket != null)
            {
                m_Socket.EndSend(ar);
            }
        }
        catch (SocketException e)
        {
            Debug.Log("socket send fail" + e.ToString());
        }
    }

    /// <summary>
    /// 为消息队列 Clone pkg 并且存放
    /// </summary>
    /// <param name="pkg"></param>
    public static void MessQueueAdd(MessPackage mp)
    {
        MessPackage pkg = new MessPackage(mp);
        m_MessQueue.Enqueue(pkg);
    }

    /// <summary>
    /// 前置包和内容包解析
    /// </summary>
    /// <param name="pkg"></param>
    public static void ParsingThePackageBody(string package, MessPackage mp)
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
    /// 进度检查
    /// </summary>
    /// <param name="pkg"></param>
    public static void check(MessPackage mp)
    {
        string[] mainSplit = mp.ret.Split("@");
        mp.ret = mainSplit[0];
        
        percent = mp.ret.Count() * 1.0f / (mp.length - 2) * 1.0f * 100.0f;
        Debug.Log("----------" + " | " + percent + "%");  // Add message package for queue.

        if (percent >= 100.0f)
        {
            mp.finish = true;
            ParsingThePackageBody(mp.ret, mp);
        }

        if (mainSplit.Length > 1 && mainSplit[1].Length > 0)
        {           
            Debug.Log("@@@@@@@@@@@@@@@@@@@@@@@: " + mainSplit[1] + " | " + mainSplit[1].Length);
            string mess = mainSplit[1];
            string[] lengthSplit = mess.Split("|");
            string totalLength = lengthSplit[0];   
            if (!mp.get_length && !string.IsNullOrEmpty(totalLength))
            {
                Debug.Log("GET LENGTH: " + totalLength);
                mp.length = int.Parse(totalLength);
                mp.get_length = true;
                mp.ret += lengthSplit[1];
            }
            else
            {
                Debug.Log("GET MESSAGE: ");
                if (mp.length >= mp.ret.Count())
                {
                    mp.ret += mess;
                }
            }
            check(mp);
        }
    }

    public static void Close()
    {
        m_Socket.Close();
    }
}

/// <summary>
/// 这是一个接受完整信息的 信息包类
/// </summary>
public class MessPackage
{
    // public Socket socket = default; // 发送信息的soket
    public string ip = ""; // 他的ip
    public string ret = ""; // 他发送的信息
    public string operate_type = "";
    public string event_type = ""; // 这个信息属于什么类型
    public int length = 0; // 这个包的总长度
    public bool finish = false; // 是否完全收包
    public bool get_length = false; // 是否已经通过前置包获取到了内容包的总长度

    public void Clear()
    {
        // socket = default;
        ip = "";
        ret = "";
        event_type = "";
        length = 0;
        finish = false;
        get_length = false;
    }

    public MessPackage() { }

    public MessPackage(MessPackage mp)
    {
        // socket = mp.socket;
        ip = mp.ip;
        ret = mp.ret;
        event_type = mp.event_type;
        length = mp.length;
        finish = mp.finish;
        get_length = mp.get_length;
    }
}

/// <summary>
/// 前置包结构
/// </summary>
public class FrontMp
{
    public string ip;
    public string length;
    public string event_type;
    public string operate_type;
}
