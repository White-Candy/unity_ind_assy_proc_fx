using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class PDFExpansion : MonoBehaviour
{
    private Button exp_Close;

    private void Awake()
    {
        exp_Close = GameObject.Find("Close").gameObject.GetComponent<Button>();

        if (exp_Close != null)
        {
            exp_Close.onClick.AddListener(() =>
            {
                gameObject.SetActive(false);
            });
        }
    }
}
