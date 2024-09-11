using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
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
    public string className;
    public bool login = false;
    public string hint = "";
}

namespace sugar
{
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

        private async void Awake()
        {
            _instance = this;
            await Utilly.DownLoadTextFromServer(Application.streamingAssetsPath + "/IP.txt", (content) =>
            {
                GlobalData.IP = content;
                GlobalData.SetUrl(content, "8096");
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
                TCP.SendAsync("[]", EventType.ClassEvent, OperateType.GET);
                RegisterPanel._instance.Active(true);
                Active(false);
            });
        }

        void Update()
        {

        }

        /// <summary>
        /// 登录请求
        /// </summary>
        private void LoginRequest()
        {
            if (UITools.InputFieldCheck(m_UserIF.text, "用户名不能为空")) { return; }
            if (UITools.InputFieldCheck(m_PwdIF.text, "密码不能为空")) { return; }
            
            // 客户端请求登录
            Client.Instance.Login(URL.URL_LOGIN, m_UserIF.text, m_PwdIF.text);
        }
    }
}
