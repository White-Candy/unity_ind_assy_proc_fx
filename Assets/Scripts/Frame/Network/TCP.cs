using Cysharp.Threading.Tasks;
using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
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

    // ���ݰ�����
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

    /// <summary>
    /// ������Ϣ���ܵ�callback
    /// </summary>
    /// <param name="ar"></param>
    public static void ReviceAsyncCallback(IAsyncResult ar)
    {
        MessPackage mp = (MessPackage)ar.AsyncState;
        int length = m_Socket.EndReceive(ar);
        try
        {
            string mess = Encoding.Default.GetString(buffer, 0, length);
            Array.Clear(buffer, 0, buffer.Length);
            // Debug.Log($"========== mess : {mess}.");
            // using (StreamWriter write = new StreamWriter("C:\\Users\\Administrator\\Desktop\\FxLog.txt"))
            // {
            //     write.WriteLine(mess);
            // }
            string[] messages = mess.Split("@");
            foreach (var message in messages)
            {
                InforProcessing(message, mp);
            }          
            m_Socket.BeginReceive(buffer, 0, buf_length, 0, ReviceAsyncCallback, mp);
        }
        catch { }
    }

    /// <summary>
    /// �첽������Ϣ
    /// </summary>
    /// <param name="mess">����</param>
    /// <param name="event_type">�¼�����</param>
    public static void SendAsync(string mess, EventType event_type, OperateType operateType)
    {
        string front = FrontPackage(mess, event_type, operateType);
        string totalInfoPkg = $"|{front}#{mess}@";
        long totalLength = totalInfoPkg.Count();
        string finalPkg = totalLength.ToString() + totalInfoPkg;
        // Debug.Log(finalPkg);

        var outputBuffer = Encoding.Default.GetBytes(finalPkg);
        m_Socket.BeginSend(outputBuffer, 0, outputBuffer.Length, SocketFlags.None, SendAsyncCbk, null);
    }

    /// <summary>
    /// �첽���ͻص�
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
    /// ǰ�ð�
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
    /// Ϊ��Ϣ���� Clone pkg ���Ҵ��
    /// </summary>
    /// <param name="pkg"></param>
    public static void MessQueueAdd(MessPackage mp)
    {
        MessPackage pkg = new MessPackage(mp);
        m_MessQueue.Enqueue(pkg);
    }

    /// <summary>
    /// ǰ�ð������ݰ�����
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
    /// ���ȼ��
    /// </summary>
    /// <param name="pkg"></param>
    public static void check(MessPackage mp)
    {
        int finalLength = mp.ret.Count() + 2;
        percent = finalLength * 1.0f / mp.length * 100.0f;
        // Debug.Log($" ============ {mp.ret.Count() + 2.0f} || {mp.length} || {percent}");
        // Debug.Log($" ============ {finalLength} || {mp.length} || {percent}"); 
        if (percent == 100.0f)
        {
            mp.finish = true;
            ParsingThePackageBody(mp.ret, mp);
        }
    }

    /// <summary>
    /// ���ܵ�����Ϣ����
    /// </summary>
    /// <param name="mess"></param>
    /// <param name="mp"></param>
    public static void InforProcessing(string mess, MessPackage mp)
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
    /// ���͹ر�����
    /// </summary>
    public static void Close()
    {
        var outputBuffer = Encoding.Default.GetBytes("Close");
        m_Socket.BeginSend(outputBuffer, 0, outputBuffer.Length, SocketFlags.None, SendAsyncCbk, null);    
        m_Socket.Close();    
    }
}

/// <summary>
/// ����һ������������Ϣ�� ��Ϣ����
/// </summary>
public class MessPackage
{
    // public Socket socket = default; // ������Ϣ��soket
    public string ip = ""; // ����ip
    public string ret = ""; // �����͵���Ϣ
    public string operate_type = "";
    public string event_type = ""; // �����Ϣ����ʲô����
    public int length = 0; // ��������ܳ���
    public bool finish = false; // �Ƿ���ȫ�հ�
    public bool get_length = false; // �Ƿ��Ѿ�ͨ��ǰ�ð���ȡ�������ݰ����ܳ���

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
/// ǰ�ð��ṹ
/// </summary>
public class FrontMp
{
    public string ip;
    public string length;
    public string event_type;
    public string operate_type;
}
