using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace sugar
{
    public class LoginPanel : MonoBehaviour
    {
        /// <summary>
        /// IF => InputField
        /// </summary>
        public TMP_InputField m_UserIF;
        public TMP_InputField m_PwdIF;
        public Button m_Login;

        private void Awake()
        {
            StartCoroutine(Utilly.DownLoadTextFromServer(Application.streamingAssetsPath + "/IP.txt", (content) =>
            {
                GlobalData.IP = content;
                GlobalData.SetUrl(content, "8096");
            }));

            if (m_Login != null)
            {
                m_Login.onClick.AddListener(LoginRequest);
            }
        }

        void Update()
        {

        }

        /// <summary>
        /// 登录请求
        /// </summary>
        private void LoginRequest()
        {
            if (string.IsNullOrEmpty(m_UserIF.text))
            {
                UITools.ShowMessage("用户名不能为空");
                return;
            }
            if (string.IsNullOrEmpty(m_PwdIF.text))
            {
                UITools.ShowMessage("密码不能为空");
                return;
            }
            
            // 客户端请求登录
            Client.Instance.Login(URL.URL_LOGIN, m_UserIF.text, m_PwdIF.text);
        }
    }
}
