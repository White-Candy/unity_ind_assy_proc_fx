using Cysharp.Threading.Tasks;
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

    // private static Vector3 m_ItemNormalCol = new Vector3(255.0f, 255.0f, 255.0f); // item默认颜色
    // private static Vector3 m_ItemSelectedCol = new Vector3(0.0f, 125.0f, 255.0f); // item选中颜色

    public override void Awake()
    {
        base.Awake();

    }

    public async void Start()
    {
        Active(false);
        _instance = this;

        comeback?.onClick.AddListener(() =>
        {
            LoginPanel._instance.Active(true);
            Active(false);
        });

        register?.onClick.AddListener(() =>
        {
            string strClassName = "";
            if (className.options.Count > 0) strClassName = className.options[className.value].text;
            NetHelper.Register(account?.text, password?.text, verify?.text, Name?.text, strClassName);
        });

        className?.onValueChanged.AddListener((int i) => 
        {
            Debug.Log("onValueChange: " + i);

            // var Options = className.options[0];
        });

        quit?.onClick.AddListener(UITools.Quit);

        await UniTask.WaitUntil(() => { return GlobalData.classList.Count != 0; });
        List<string> classNameList = new List<string>();
        foreach (var classInf in GlobalData.classList) { classNameList.Add(classInf.Class); }
        className.AddOptions(classNameList);
    }
}
