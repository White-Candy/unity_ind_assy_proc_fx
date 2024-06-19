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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            ModelAnimControl._Instance.Play(0, 100);
        }
    }

    void Start()
    {
        // 再重置玩家镜头
        CameraControl.SetNormal();
        ModelAnimControl._Instance.Play(0, 0); // 这并不是为了 播放什么动画，只是为了重置场景模型
    }
}
