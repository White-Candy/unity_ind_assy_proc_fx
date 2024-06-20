using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickMethod : MonoBehaviour
{
    private RaycastHit hit;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                //Debug.Log("hit name: " + hit.collider.name);
                GameMode.Instance.SetToolName(hit.collider.name);
            }
        }
    }
}
