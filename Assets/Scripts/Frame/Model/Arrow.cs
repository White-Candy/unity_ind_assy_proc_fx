using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Arrow : MonoBehaviour
{
    public TextMeshProUGUI countText;

    void Start()
    {
        
    }

    void Update()
    {
        countText.text = GameMode.Instance.NumberOfToolsRemaining();
        transform.LookAt(CameraControl.target.transform);
    }
}
