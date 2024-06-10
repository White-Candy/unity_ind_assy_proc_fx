using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginData
{
    public string m_Username;
    public string m_Password;
}

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

    public void Login(string path, string username, string password)
    {
        LoginData login_data = new LoginData();
        login_data.m_Username = username;
        login_data.m_Password = password;
        string json = LitJson.JsonMapper.ToJson(login_data);

        StartCoroutine(m_Server.Post(path, json, (data) =>
        {
            Debug.Log(data);
        }));
    }
}
