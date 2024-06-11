using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager _Instance;

    private void Awake()
    {
        _Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
      
    }

    /// <summary>
    /// ÏÂÝdÎÄ™n
    /// </summary>
    /// <param name="url"></param>
    /// <param name="callback"></param>
    public void DownLoadTextFromServer(string url, Action<string> callback)
    {
        StartCoroutine(Utilly.DownLoadTextFromServer(url, callback));
    }
}
