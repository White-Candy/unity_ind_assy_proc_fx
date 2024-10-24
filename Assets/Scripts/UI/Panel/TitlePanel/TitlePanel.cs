using LitJson;

using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : BasePanel
{
    public Button exitButton;

    public TextMeshProUGUI m_Title;

    public static TitlePanel _instance;

    public override void Awake()
    {
        base.Awake();
        _instance = this;
        m_Title.text = $"ģ������-{GlobalData.currModuleName}";
        exitButton.onClick.AddListener(OnExitBtnClicked);
    }

    private void OnExitBtnClicked()
    {
        Debug.Log("Exit Game");
        //CameraControl.SetMain();
        if (GlobalData.SceneModel != null)
        {
            Utilly.ExitModeSceneAction();
            
#if UNITY_WEBGL
            GlobalData.currModuleName = "";
            UITools.Loading("Menu");
#endif            
        }
        else
        {
            GlobalData.currModuleName = "";
            UITools.Loading("Menu");
        }

    }

    public void SetTitle(string title)
    {
        m_Title.text = $"ģ������-{title}";
    }
}
