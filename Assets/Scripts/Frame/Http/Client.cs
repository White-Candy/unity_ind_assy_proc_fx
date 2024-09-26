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

    /// <summary>
    /// Client 
    /// </summary>
    /// <param name="path"></param>
    /// <param name="username"></param>
    /// <param name="password"></param>
    // public static async void Login(string path, string username, string password)
    // {
        // var req = UnityWebRequest.Post("http://192.168.3.34:5800/", finalBody);
        // await req.SendWebRequest();
        // if (req.error == null) 
        // {
        //     while(!req.isDone)
        //     {
        //         await UniTask.Yield();
        //     }

        //     Debug.Log(req.downloadHandler.text);
            //JsonData data = JsonMapper.ToObject(req.downloadHandler.text);
            //Debug.Log("Return Body: " + req.downloadHandler.text);

            // if (Tools.CheckMessageSuccess(int.Parse(data["code"].ToString())))
            // {
            //     if(callback != null && req.error == null)
            //     {
            //         callback(req.downloadHandler.text);
            //     }
            // }
            // else
            // {
            //     try
            //     {
            //         UITools.ShowMessage(data["msg"].ToString());
            //     }
            //     catch (Exception e)
            //     {
            //         Debug.Log(e);
            //     }
            // }
        //}
        //else
        //{
            // Debug.Log("??????????");
        //    UITools.ShowMessage("???????????????????");
        //}
    //}
}
