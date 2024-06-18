using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Camera 初始化启动器
public class CameraLauncher : MonoBehaviour
{
    private void Awake()
    {
        CameraControl.main = GameObject.Find("Main Camera");

        if (CameraControl.player != null)
        {
            CameraControl.player.transform.Find("Capsule").gameObject.GetComponent<MeshRenderer>().enabled = false;
        }

        // 先关闭所有镜头
        CameraControl.CloseAll();
    }

    void Start()
    {
        // 在重置玩家镜头
        CameraControl.SetNormal();
    }

}
