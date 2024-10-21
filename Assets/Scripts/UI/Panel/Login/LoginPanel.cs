using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using LitJson;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

/// <summary>
/// 用户信息包
/// </summary>
public class UserInfo
{
    public string userName;
    public string password;
    public string Name;
    public string Gender;
    public string Age;
    public string Identity;
    public string idCoder;
    public string Contact;
    public string UnitName;
    public bool login = false;
    public string hint = "";
}


public class LoginPanel : BasePanel
{
    /// <summary>
    /// IF => InputField
    /// </summary>
    public TMP_InputField m_UserIF;
    public TMP_InputField m_PwdIF;
    public Button m_Login;
    public Button m_Close;
    public Button m_Regiester;

    public static LoginPanel _instance;

    public override async void Awake()
    {
        _instance = this;

        await Utilly.DownLoadTextFromServer(Application.streamingAssetsPath + "/IP.txt", (content) =>
        {
            GlobalData.IP = content;
            GlobalData.SetUrl(content, "5800");
        });

        if (m_Login != null)
        {
            m_Login.onClick.AddListener(LoginRequest);
        }

        if (m_Close != null)
        {
            m_Close.onClick.AddListener(UITools.Quit);
        }

        m_Regiester?.onClick.AddListener(() => 
        {
            HTTPConsole.SendAsyncPost("[]", EventType.ClassEvent, OperateType.GET);
            RegisterPanel._instance.Active(true);
            Active(false);
        });

        // StartCoroutine(conect());
    }

    //IEnumerator conect()
    //{
    //    byte[] bytes = null;
    //    string body = "pppooo";
    //    if (body != null)
    //    {
    //        string str = System.Text.RegularExpressions.Regex.Unescape(body);
    //        bytes = Encoding.UTF8.GetBytes(str);
    //    }

    //    using (UnityWebRequest req = new UnityWebRequest(@"http://182.43.46.225:5800/", UnityWebRequest.kHttpVerbPOST))
    //    {
    //        yield return new WaitForSeconds(1);
    //        req.downloadHandler = new DownloadHandlerBuffer();
    //        req.uploadHandler = new UploadHandlerRaw(bytes);
    //        req.SetRequestHeader("Content-Type", "application/json;charset=utf8");

    //        yield return req.SendWebRequest();
    //        Debug.Log("99999");
    //        if (req.error == null)
    //        {
    //            Debug.Log("服务器链接成功");
    //            if (req.isDone)
    //            {
    //                Debug.Log("-----------");
    //                Debug.Log(req.downloadHandler.text);
    //                yield break;
    //            }
    //        }
    //        else
    //        {
    //            Debug.Log("无法与服务器建立连接" + req.error);
    //            UITools.ShowMessage("无法与服务器建立连接，请联系后台管理员");
    //        }
    //    }
    //}

    public async void Start()
    {

    //     await Client.m_Server.Post(URL.IP, "", (text) => 
    //     {
    //         Debug.Log("Post Return: " + text);
    // #if UNITY_EDITOR
    //         FileHelper.WriteFileByLine(Application.streamingAssetsPath, "HTTPLog.txt", text);
    // #endif
    //         // HttpFieldProcessing(text);
    //     });      
    }

    /// <summary>
    /// 登录请求
    /// </summary>
    private void LoginRequest()
    {
        if (UITools.InputFieldCheck(m_UserIF.text, "用户名不能为空")) { return; }
        if (UITools.InputFieldCheck(m_PwdIF.text, "密码不能为空")) { return; }
        
        // 客户端请求登录
        UserInfo inf = new UserInfo()
        {
            userName = m_UserIF.text,
            password = m_PwdIF.text
        };
        HTTPConsole.SendAsyncPost(JsonMapper.ToJson(inf), EventType.UserLoginEvent, OperateType.NONE);
    }
}
