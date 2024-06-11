using sugar;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager _Instance;

    private void Awake()
    {
        _Instance = this;
    }

    /// <summary>
    /// общdнд≥n
    /// </summary>
    /// <param name="url"></param>
    /// <param name="callback"></param>
    public void DownLoadTextFromServer(string url, Action<string> callback)
    {
        StartCoroutine(Utilly.DownLoadTextFromServer(url, callback));
    }
}
