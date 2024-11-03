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
/// �û���Ϣ��
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
    }

    public void Start()
    {
#if UNITY_WEBGL
        FileHelper.ReadTargetFileOnWebGL();    
#endif
    }

    /// <summary>
    /// ��¼����
    /// </summary>
    private void LoginRequest()
    {
        if (UITools.InputFieldCheck(m_UserIF.text, "�û�������Ϊ��")) { return; }
        if (UITools.InputFieldCheck(m_PwdIF.text, "���벻��Ϊ��")) { return; }
        
        // �ͻ��������¼
        UserInfo inf = new UserInfo()
        {
            userName = m_UserIF.text,
            password = m_PwdIF.text
        };
        HTTPConsole.SendAsyncPost(JsonMapper.ToJson(inf), EventType.UserLoginEvent, OperateType.NONE);
    }
}
