using LitJson;
using sugar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public class LoginData
//{
//    public string username;
//    public string password;
//}

public class Client : Singleton<Client>
{
    [HideInInspector]
    public Server m_Server;

    public override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        m_Server = GetComponent<Server>();
    }

    /// <summary>
    /// Client ��¼����
    /// </summary>
    /// <param name="path"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public async void Login(string path, string username, string password)
    {
        await TCPHelper.UserLoginReq(username, password);
    }
}
