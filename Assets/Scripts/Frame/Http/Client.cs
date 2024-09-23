using UnityEngine;

//public class LoginData
//{
//    public string username;
//    public string password;
//}

public class Client : MonoBehaviour
{
    [HideInInspector]
    public Server m_Server;

    public void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        m_Server = GetComponent<Server>();
    }

    /// <summary>
    /// Client ��¼����
    /// </summary>
    /// <param name="path"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    public static void Login(string path, string username, string password)
    {
        TCPHelper.UserLoginReq(username, password);
    }
}
