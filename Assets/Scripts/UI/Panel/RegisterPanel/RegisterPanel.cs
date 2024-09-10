using LitJson;
using sugar;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    public TMP_InputField account;
    public TMP_InputField password;
    public TMP_InputField verify;

    public Button register;
    public Button comeback;
    public Button quit;

    public static RegisterPanel _instance;

    public override void Awake()
    {
        base.Awake();
        Active(false);
        _instance = this;
    }

    public void Start()
    {
        comeback?.onClick.AddListener(() =>
        {
            LoginPanel._instance.Active(true);
            Active(false);
        });

        register?.onClick.AddListener(() =>
        {
            TCPHelper.Register(account?.text, password?.text, verify?.text);
        });

        quit?.onClick.AddListener(UITools.Quit);
    }
}
