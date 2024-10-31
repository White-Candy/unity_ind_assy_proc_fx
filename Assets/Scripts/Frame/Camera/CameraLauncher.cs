using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Camera 初始化启动器
public class CameraLauncher : MonoBehaviour
{
    private void Awake()
    {

    }

    public void Start()
    {
        CameraControl.main = GameObject.Find("Main Camera");

        // 先关闭所有镜头
        CameraControl.CloseAll();

        // 再重置主镜头
        CameraControl.SetMain();
    }

    private void Update()
    {
        float num = RenderSettings.skybox.GetFloat("_Rotation");
        RenderSettings.skybox.SetFloat("_Rotation", num + 0.002f);
    }
}
