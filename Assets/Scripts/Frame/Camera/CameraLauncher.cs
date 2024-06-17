using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Camera ³õÊ¼»¯Æô¶¯Æ÷
public class CameraLauncher : MonoBehaviour
{


    void Start()
    {
        CameraControl.main = GameObject.Find("Main Camera");
        CameraControl.player = transform.Find("PlayerCapsule")?.gameObject;
        CameraControl.animation = transform.Find(ModelAnimControl._Instance.m_animCameraName)?.gameObject;

        if (CameraControl.player != null)
        {
            CameraControl.player.transform.Find("Capsule").gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
    }

    private void Init()
    {
        CameraControl.main.SetActive(false);
    }

}
