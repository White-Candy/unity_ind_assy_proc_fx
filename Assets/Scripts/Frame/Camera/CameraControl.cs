using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 用来存储场景中各种功能的相机
public static class CameraControl
{
    static public GameObject main; // 主相机
    static public GameObject animation; // 动画相机
    static public GameObject player; // 人物相机

    static public GameObject target; // 目前在使用的相机

    static public void CloseAll()
    {
        if (main) main?.SetActive(false);
        if (animation) animation?.SetActive(false);
        if (player) player?.SetActive(false);
        target = null;
    }

    /// <summary>
    /// 普通模式显示player相机，可以用于浏览场景
    /// </summary>
    static public void SetPlayer()
    {
        CloseAll();
        player?.SetActive(true); 
        target = player;
    }

    /// <summary>
    /// 播放动画 使用动画相机.
    /// </summary>
    static public void SetAnimation()
    {
        CloseAll();
        animation?.SetActive(true);
        target = animation;
    }

    /// <summary>
    /// 显示主相机
    /// </summary>
    static public void SetMain()
    {
        CloseAll();
        main?.SetActive(true);
        target = main;
    }

    // 我知道这样写拓展性很差，结构写的很奇怪，是的，我觉得以后不会再这个里面添加新的Camera obj了...
    //... 所以就这么写把。。。
}
