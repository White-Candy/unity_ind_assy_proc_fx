using LitJson;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : BasePanel
{
    public Button exitButton;

    public TextMeshProUGUI m_Title;

    public static TitlePanel _instance;

    public GameObject titlePanel;

    public override void Awake()
    {
        base.Awake();
        _instance = this;
    }

    public override void Start()
    {
        m_Title.text = $"{GlobalData.currModuleName}-{GlobalData.courseName}";
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
            SetTitlePanelActive(true);
#endif
        }
        else
        {
            GlobalData.currModuleName = "";
            UITools.Loading("Menu");
        }

    }

    public void SetTitlePanelActive(bool b)
    {
        titlePanel.SetActive(b);
        if (GlobalData.mode == Mode.Examination) 
            exitButton.gameObject.SetActive(b);
    }

    public void SetTitle(string title)
    {
        m_Title.text = $"{GlobalData.courseName}-{title}";
    }
}
