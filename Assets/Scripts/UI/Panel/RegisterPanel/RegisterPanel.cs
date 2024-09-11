using Cysharp.Threading.Tasks;
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
    public TMP_InputField Name;
    public TMP_Dropdown className;

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

    public async void Start()
    {
        comeback?.onClick.AddListener(() =>
        {
            LoginPanel._instance.Active(true);
            Active(false);
        });

        register?.onClick.AddListener(() =>
        {
            string strClassName = "";
            if (className.options.Count > 0) strClassName = className.options[className.value].text;
            TCPHelper.Register(account?.text, password?.text, verify?.text, Name?.text, strClassName);
        });

        quit?.onClick.AddListener(UITools.Quit);

        await UniTask.WaitUntil(() => { return GlobalData.classList.Count != 0; });
        List<string> classNameList = new List<string>();
        foreach (var classInf in GlobalData.classList) { classNameList.Add(classInf.Class); }
        className.AddOptions(classNameList);
    }
}
