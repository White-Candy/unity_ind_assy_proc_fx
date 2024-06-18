using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用来存储场景中各种功能的相机
public static class CameraControl
{
    static public GameObject main; // 主相机
    static public GameObject animation; // 动画相机
    static public GameObject player; // 人物相机

    static public void CloseAll()
    {
        main?.SetActive(false);
        animation?.SetActive(false);
        player?.SetActive(false);
    }

    static public void SetNormal()
    {
        main?.SetActive(false);
        animation?.SetActive(false);
        player?.SetActive(true);
    }
}
