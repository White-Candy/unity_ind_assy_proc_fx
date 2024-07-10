using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public TextMeshProUGUI m_Count;

    void Start()
    {
        
    }

    void Update()
    {
        m_Count.text = GameMode.Instance.NumberOfToolsRemaining();
        transform.LookAt(CameraControl.target.transform);
    }
}
