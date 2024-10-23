using Cysharp.Threading.Tasks;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;

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
}
