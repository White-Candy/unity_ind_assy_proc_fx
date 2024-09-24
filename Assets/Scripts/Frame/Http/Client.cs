using LitJson;
using UnityEngine;

//public class LoginData
//{
//    public string username;
//    public string password;
//}

public class Client : MonoBehaviour
{
    [HideInInspector]
    public static Server m_Server;

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        m_Server = GetComponent<Server>();
    }

    /// <summary>
    /// Client µÇÂ¼ÇëÇó
    /// </summary>
    /// <param name="path"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public static void Login(string path, string username, string password)
    {
        NetHelper.UserLoginReq(username, password);
        //TODO..
    }
}
