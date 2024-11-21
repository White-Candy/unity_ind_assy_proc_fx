using System;
using System.Collections;
using System.Collections.Generic;
using DG.DemiLib;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public class CameraMovementController : Singleton<CameraMovementController>
{
    [SerializeField]public Transform m_target;

    

    [SerializeField]private float m_RotateSpeed = 10f;//旋转速度

    [SerializeField]private float m_RotLerpSpeed = 10f;
    
    [SerializeField]private float m_Distance = 1.5f;//相机与模型的距离

    [SerializeField]private float m_MinYawAngle = -360f;
    [SerializeField]private float m_MaxYawAngle = 360f;
    [SerializeField]private float m_MinPitchAngle = 1f;
    [SerializeField]private float m_MaxPitchAngle = 90f;
    
    [SerializeField]private bool isAutoRotate;
    
    [SerializeField]private float m_AutoRotateTime = 2f;//无操作两秒后进行自旋转
    [SerializeField]private float m_AutoRotateSpeed = 10f;

    [SerializeField]private float m_ZoomSpeed = 50f;
    [SerializeField]private float m_ZoomSmooth = 10f;

    [SerializeField]private float m_CameraFov = 40f;
    [SerializeField]private float m_MinFov = 10f;
    [SerializeField]private float m_MaxFov = 40f;
    
    private Camera m_Camera;
    private float m_IdleTime = 0f;
    
    
    private float m_Yaw = 0f;//偏航角

    private float m_Pitch = 0f;//俯仰角


    private Quaternion m_DefultRotate;
    private Vector3 m_DefultPos;
    private float m_DefultFov;

    public override void Awake()
    {
        base.Awake();
    }

    public void Start()
    {
        m_Camera = this.GetComponent<Camera>();
        m_Camera.fieldOfView = m_CameraFov;

        m_DefultRotate = m_Camera.transform.rotation;
        m_DefultPos = m_Camera.transform.position;
        m_DefultFov = m_Camera.fieldOfView;
    }

    private void LateUpdate()
    {
        if (m_target == null)
            return;
        if (Input.GetMouseButton(1))
        {
            //CursorActive(false);
            m_Yaw += Input.GetAxis("Mouse X") * Time.deltaTime * m_RotateSpeed;
            m_Pitch -= Input.GetAxis("Mouse Y") * Time.deltaTime * m_RotateSpeed;
            
            m_Pitch = ClampAngle(m_Pitch, m_MinPitchAngle, m_MaxPitchAngle);
            m_Yaw = ClampAngle(m_Yaw, m_MinYawAngle, m_MaxYawAngle);
            m_IdleTime = 0f;
        }
        //else if (Input.GetMouseButtonUp(1)) { CursorActive(true); }

        if (isAutoRotate)
        {
            m_IdleTime += Time.deltaTime;
            if (m_IdleTime >= m_AutoRotateTime)
            {
                m_Yaw += 1f * Time.deltaTime * m_AutoRotateSpeed;
                m_Yaw = ClampAngle(m_Yaw, m_MinYawAngle, m_MaxYawAngle);
            }
        }
        
        Quaternion rot = Quaternion.Euler(m_Pitch,m_Yaw,0f);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, m_RotLerpSpeed *Time.deltaTime);
        transform.position = m_target.position - (transform.forward * m_Distance);
        
        m_CameraFov -= Input.GetAxis("Mouse ScrollWheel") * m_ZoomSpeed;
        m_CameraFov =  Mathf.Clamp(m_CameraFov, m_MinFov, m_MaxFov);
        m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, m_CameraFov, m_ZoomSmooth * Time.deltaTime);
    }

    private void CursorActive(bool b)
    {
        Cursor.visible = b;
        Cursor.lockState = b ? CursorLockMode.None : CursorLockMode.Locked;
    }

    public void UpdateData(Transform target = null)
    {
        m_target = target;
        ResetData();
    }

    public void Clear()
    {
        m_target = null;
        ResetData();
    }

    public void ResetData()
    {
        m_Camera.transform.position = m_DefultPos;
        m_Camera.transform.rotation = m_DefultRotate;
        m_Camera.fieldOfView = m_DefultFov;
        
        m_Pitch = 0;
        m_Yaw = 0;
        m_CameraFov = m_DefultFov;
        m_IdleTime = 0;
    }


    /// <summary>
    /// 限制角度0-360
    /// </summary>
    /// <param name="angle"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
}