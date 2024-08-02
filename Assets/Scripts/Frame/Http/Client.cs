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
    /// Client µÇÂ¼ÇëÇó
    /// </summary>
    /// <param name="path"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public async void Login(string path, string username, string password)
    {
        await NetworkTCPExpand.UserLoginReq(username, password);
        // LoginData login_data = new LoginData();
        // login_data.username = username;
        // login_data.password = password;
        // string json = LitJson.JsonMapper.ToJson(login_data);
        // UITools.Loading("Menu");
        //await m_Server.Post(path, json, (body) =>
        //{
        //    // Debug.Log(body);
        //    JsonData data = JsonMapper.ToObject(body);
        //    GlobalData.token = data["token"]?.ToString();
        //
        //    PlayerPrefs.SetString("username", username);
        //    PlayerPrefs.SetString("password", password);
        //
        //    UITools.Loading("Menu");
        //});
    }
}
